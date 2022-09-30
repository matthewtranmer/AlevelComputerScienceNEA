using System.Text;
using System.Numerics;
using System.Net.Sockets;
using System.Text.Json;
using System.Timers;

using MySql.Data.MySqlClient;
using Matthew_Tranmer_NEA.Networking;
using Matthew_Tranmer_NEA.Searching;

namespace Matthew_Tranmer_NEA_Server.Workers
{
    internal class APIworker
    {
        private List<ManagmentWorker> managment_tunnel_workers = new List<ManagmentWorker>();
        private MySqlConnection db;
        private BigInteger private_key;
        private System.Timers.Timer audit_timer;
        private FriendGraph friend_storage;

        public APIworker(MySqlConnection db, BigInteger private_key, FriendGraph friend_storage)
        {
            this.db = db;
            this.private_key = private_key;
            this.friend_storage = friend_storage;

            //Create a timer which will start the managment tunnel auditor every 30 seconds.
            audit_timer = new System.Timers.Timer(30000);
            audit_timer.Elapsed += managementTunnelAuditor;
            audit_timer.AutoReset = true;
            audit_timer.Enabled = true;
        }

        //Event which will remove the unresponsive management tunnels from the list.
        private void managementTunnelAuditor(object? source, ElapsedEventArgs e)
        {
            Program.Display("Tunnel Audit Started...", ConsoleColor.Cyan);

            const int seconds_till_timeout = 20;
            int removed_worker_count = 0;
            int index = 0;

            //Make sure another thread isn't modifying the managment tunnel workers list.
            lock (managment_tunnel_workers)
            {
                //Loop through all the managment tunnel workers.
                for (int i = 0; i < managment_tunnel_workers.Count; i++)
                {
                    //If the last heartbeat time is too low then remove the managment worker from the list.
                    if (managment_tunnel_workers[i].last_heartbeat + seconds_till_timeout < DateTimeOffset.Now.ToUnixTimeSeconds())
                    {
                        managment_tunnel_workers.RemoveAt(index);
                        removed_worker_count++;
                    }
                    else
                    {
                        index++;
                    }
                }
            }

            Program.Display($"Audit Complete. Workers Removed: {removed_worker_count}, Total Tunnels Open: {managment_tunnel_workers.Count}.", ConsoleColor.Cyan); 
        }

        //Start a new request worker in a new thread.
        async public Task start(Socket raw_connection)
        {
            await Task.Factory.StartNew(() => requestWorker(raw_connection));
        }

        //Method which creates a managment request which is sent to the client if there is a managment tunnel open, otherwise
        //it is buffered in the database until a new tunnel is opened.
        public void createManagmentRequest(string recipient_username, Dictionary<string, string> request, bool buffer_if_offline=true)
        {
            //Get the userID from the database.
            string cmd_text = "SELECT UserID FROM users WHERE Username = @Username;";
            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@Username", recipient_username);

            int UserID;
            //Make sure another thread isn't reading from the database.
            lock (db)
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    UserID = reader.GetInt32(0);
                }
            }

            //Serialize the request using JSON.
            string json_request = JsonSerializer.Serialize(request);

            bool request_sent = false;
            //Make sure another thread isn't modifying the managment tunnel workers list.
            lock (managment_tunnel_workers){
                int worker_index = BinarySearch.searchTunnelWorker(managment_tunnel_workers, UserID);
                
                //If a connection is open then send the request to the user.
                if (worker_index != -1)
                {
                    try
                    {
                        managment_tunnel_workers[worker_index].send(json_request);
                        request_sent = true;
                    }
                    catch 
                    {
                        request_sent = false;
                    }
                }
            }

            //A tunnel worker is not open (or is unresponsive) so buffer the data in the database if need be.
            if (!request_sent && buffer_if_offline)
            {
                cmd_text = "INSERT INTO managementrequests (RequestData, Recipient) VALUES (@RequestData, @Recipient)";
                command = new MySqlCommand(cmd_text, db);
                command.Parameters.AddWithValue("@RequestData", json_request);
                command.Parameters.AddWithValue("@Recipient", UserID);
                command.ExecuteNonQuery();
            } 
        }

        //Creates a sustained connection between the client and the server.
        private Dictionary<string, string>? createManagmentTunnel(Socket raw_connection, EncryptedSocketWrapper socket_wrapper, string username, string token)
        {
            //Make sure the token is valid.
            if (!API.authenticateRequest(db, username, token))
            {
                return new Dictionary<string, string>() { { "error", "Invalid Session Token." } };
            }

            //Read the userID from the database.
            string cmd_text = "SELECT UserID FROM users WHERE Username = @Username";
            MySqlCommand cmd = new MySqlCommand(cmd_text, db);
            cmd.Parameters.AddWithValue("@Username", username);

            ManagmentWorker mng_worker;
            int UserID;
            //Make sure another thread isn't reading from the database.
            lock (db)
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    UserID = reader.GetInt32(0);

                    //Make sure another thread isn't modifying the managment tunnel workers list.
                    lock (managment_tunnel_workers)
                    {
                        //If there is a tunnel worker already then remove it.
                        int index = BinarySearch.searchTunnelWorker(managment_tunnel_workers, UserID);
                        if (index != -1)
                        {
                            managment_tunnel_workers.RemoveAt(index);
                        }

                        //Create a new managment worker and add it to the list.
                        mng_worker = new ManagmentWorker(UserID, raw_connection, socket_wrapper);
                        BinarySearch.addTunnelWorker(managment_tunnel_workers, mng_worker);
                    }
                }
            }

            //Send an OK response to the server.
            Dictionary<string, string> message = new Dictionary<string, string>() { { "message", "OK" } };
            string json_message = JsonSerializer.Serialize(message);
            socket_wrapper.send(Encoding.UTF8.GetBytes(json_message));

            //Read the buffered management requests from the database.
            cmd_text = "SELECT RequestData FROM managementrequests WHERE Recipient = @Recipient";
            cmd = new MySqlCommand(cmd_text, db);
            cmd.Parameters.AddWithValue("@Recipient", UserID);

            List<string> requests = new List<string>();


            //Make sure another thread isn't reading from the database.
            lock (db)
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    //Read all the buffered data and write it to memory.
                    while (reader.Read())
                    {
                        requests.Add(reader.GetString(0));
                    }
                }
            }

            foreach (string request in requests)
            {
                mng_worker.send(request);
            }

            //Delete all the buffed data from the server.
            cmd_text = "DELETE FROM managementrequests WHERE Recipient = @Recipient";
            cmd = new MySqlCommand(cmd_text, db);
            cmd.Parameters.AddWithValue("@Recipient", UserID);
            cmd.ExecuteNonQuery();

            return null;
        }

        //Method that handles the request from the client.
        private void requestWorker(Socket raw_connection)
        {
            Program.Display("Request Accepted", ConsoleColor.DarkYellow);

            //Set timeouts.
            const int timeout = 10000;
            raw_connection.ReceiveTimeout = timeout;
            raw_connection.SendTimeout = timeout;

            //Create encrypted socket wrapper.
            EncryptedSocketWrapper socket_wrapper = new EncryptedSocketWrapper(raw_connection);

            //Recieve encrypted headers.
            byte[] encoded_data = socket_wrapper.recieveSigned(private_key);
            string json_data = Encoding.UTF8.GetString(encoded_data);

            //Deserialize recieved data.
            Dictionary<string, string>? request = JsonSerializer.Deserialize<Dictionary<string, string>>(json_data);

            bool keep_alive = false;
            Dictionary<string, string>? response = null;
            try
            {
                Program.Display(request!["URL"], ConsoleColor.Red);
                switch (request["URL"])
                {
                    //Managment endpoints.
                    case "\\api\\management\\management_tunnel":
                        response = createManagmentTunnel(raw_connection, socket_wrapper, request["username"], request["token"]);
                        if (response == null) keep_alive = true;
                        break;

                    case "\\api\\management\\heartbeat":
                        response = API.managmentHeartbeat(db, managment_tunnel_workers, request["username"], request["token"]);
                        break;

                    case "\\api\\management\\create_account":
                        response = API.createAccount(db, friend_storage, request["username"], request["authentication_code"]);
                        break;

                    case "\\api\\management\\generate_token":
                        response = API.generateToken(db, request["username"], request["authentication_code"]);
                        break;

                    //Secrets endpoints.
                    case "\\api\\secrets\\download_private_keys":
                        response = API.downloadPrivateKeys(db, request["username"], request["token"]);
                        break;

                    case "\\api\\secrets\\upload_initial_keys":
                        response = API.uploadInitialKeys(db, request["username"], request["token"], request["private_identity_key"], request["public_identity_key"], request["private_signed_pre_key"], request["public_signed_pre_key"], request["pre_key_signature"]);
                        break;

                    case "\\api\\secrets\\download_pre_keys":
                        response = API.downloadPreKeys(db, request["username"], request["token"]);
                        break;

                    case "\\api\\secrets\\upload_pre_key":
                        response = API.uploadPreKey(db, request["username"], request["token"], request["identifier"], request["encrypted_private_key"], request["encoded_public_key"]);
                        break;

                    //Friend endpoints.
                    case "\\api\\friend\\send_friend_request":
                        response = API.sendFriendRequest(db, this, request["username"], request["token"], request["recipient"]);
                        break;

                    case "\\api\\friend\\accept_friend_request":
                        response = API.acceptFriendRequest(db, friend_storage, request["username"], request["token"], request["requester"]);
                        break;

                    case "\\api\\friend\\get_friends_and_requests":
                        response = API.getFriendsAndRequests(db, friend_storage, request["username"], request["token"]);
                        break;

                    case "\\api\\friend\\get_opened_chats":
                        response = API.getOpenedChats(db, request["username"], request["token"]);
                        break;

                    //Message endpoints.
                    case "\\api\\message\\request_message_send":
                        response = API.requestMessageSend(this, db, request["username"], request["token"], request["recipient_username"]);
                        break;

                    case "\\api\\message\\send_message":
                        response = API.sendMessage(this, db, request["username"], request["token"], request["recipient_username"], request["encrypted_message"], request);
                        break;

                    case "\\api\\message\\recieve_message":
                        response = API.recieveMessage(db, request["username"], request["token"], request["sender"]);
                        break;

                    case "\\api\\message\\upload_old_message":
                        response = API.uploadOldMessage(db, request["username"], request["token"], request["sender"], request["recipient"], request);
                        break;

                    case "\\api\\message\\download_old_messages":
                        response = API.downloadOldMessages(db, request["username"], request["token"], request["recipient"]);
                        break;
                    
                    default:
                        response = new Dictionary<string, string>()
                        {
                            { "fatal_error", $"The requested endpoint URL ('{request["URL"]}') does not exist." }
                        };
                        break;
                }
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(e);

                response = new Dictionary<string, string>()
                {
                    { "fatal_error", "A required key was not sent in the JSON object." }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                response = new Dictionary<string, string>()
                {
                    { "fatal_error", "A Server Side Error Has Occured." }
                };
            }

            if (!keep_alive)
            {   
                //Serialize and send response.
                string json_response = JsonSerializer.Serialize(response);
                socket_wrapper.send(Encoding.UTF8.GetBytes(json_response));

                //Close connection because it isn't needed anymore.
                raw_connection.Close();
            }
        }
    }
}
