using System.Text.Json;

using MySql.Data.MySqlClient;
using Matthew_Tranmer_NEA.Generic;
using Matthew_Tranmer_NEA_Server.Workers;
using Matthew_Tranmer_NEA.Searching;


namespace Matthew_Tranmer_NEA_Server
{
    internal static class API
    {
        //Time until the token will expire.
        static readonly int token_expiration = 86400;

        //Authenticate the given token.
        public static bool authenticateRequest(MySqlConnection db, string username, string token)
        {
            //Recieve the token and expiry date from the database.
            string cmd_text = "SELECT Token, ExpiryDate FROM tokens WHERE UserID = (SELECT UserID FROM users WHERE Username = @Username)";
            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@Username", username);

            //Make sure another thread isn't reading from the database.
            lock (db)
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    //If no records are returned then the given data must be invalid.
                    if (!reader.Read())
                    {
                        return false;
                    }

                    //Read data from reader.
                    string db_token = reader.GetString(0);
                    long expiry_date = reader.GetInt64(1);

                    //Get current unix time.
                    long unix_time = DateTimeOffset.Now.ToUnixTimeSeconds();

                    //If the token is equal to the token recieved from the database and it isn't expired then the given data is valid.
                    if (db_token == token && unix_time < expiry_date)
                    {
                        return true;
                    }

                    //Data must be invalid.
                    return false;
                }
            }
        }

        //Read the encrypted private keys from the database and send them to the client.
        public static Dictionary<string, string>? downloadPrivateKeys(MySqlConnection db, string username, string token)
        {
            //Make sure token is valid.
            if (!authenticateRequest(db, username, token))
            {
                return new Dictionary<string, string>() { { "error", "Invalid Session Token" } };
            }

            //Read the keys from the database.
            string cmd_text = "SELECT PrivateIDkey, PrivateSignedPreKey FROM privatekeys WHERE UserID = (SELECT UserID FROM users WHERE Username = @Username);";
            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@Username", username);

            //Make sure another thread isn't reading from the database.
            lock (db)
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    //Advance the reader.
                    reader.Read();

                    Dictionary<string, string> response = new Dictionary<string, string>()
                    {
                        { "private_identity_key", reader.GetString(0) },
                        { "private_signed_pre_key", reader.GetString(1) }
                    };

                    return response;
                }
            }
        }

        //Store the clients generated pre key in the database.
        public static Dictionary<string, string>? uploadPreKey(MySqlConnection db, string username, string token, string identitfier, string encrypted_private_key, string encoded_public_key)
        {
            //Make sure token is valid.
            if (!authenticateRequest(db, username, token))
            {
                return new Dictionary<string, string>() { { "error", "Invalid Session Token" } };
            }

            //Store the public pre key into the publicprekeys table.
            string cmd_text = "INSERT INTO publicprekeys (UserID, Identifier, PreKey, HasBeenUsed) VALUES ((SELECT UserID FROM users WHERE Username = @username), @Identifier, @PreKey, 0);";
            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@Identifier", identitfier);
            command.Parameters.AddWithValue("@PreKey", encoded_public_key);
            command.ExecuteNonQuery();

            //Store the encrypted private pre key in the privateprekeys table.
            cmd_text = "INSERT INTO privateprekeys (AssociatedPreKeyID, PreKey) VALUES ((SELECT PreKeyID FROM publicprekeys WHERE Identifier = @Identifier), @PreKey);";
            command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@Identifier", identitfier);
            command.Parameters.AddWithValue("@PreKey", encrypted_private_key);
            command.ExecuteNonQuery();

            return null;
        }

        //Store the clients generated keys in the database after they have signed up.
        public static Dictionary<string, string>? uploadInitialKeys(MySqlConnection db, string username, string token, string privateIDkey, string publicIDkey, string privateSPK, string publicSPK, string preKeySignature)
        {
            //Make sure token is valid.
            if (!authenticateRequest(db, username, token))
            {
                return new Dictionary<string, string>() { { "error", "Invalid Session Token" } };
            }

            //Get the userID from the database.
            string cmd_text = "SELECT UserID FROM users WHERE Username = @Username;";
            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@Username", username);

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
            
            //Store the encrypted private keys in the privatekeys table.
            cmd_text = "INSERT INTO privatekeys (UserID, PrivateIDkey, PrivateSignedPreKey) VALUES (@UserID, @PrivateIDkey, @PrivateSignedPreKey);";
            command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@UserID", UserID);
            command.Parameters.AddWithValue("@PrivateIDkey", privateIDkey);
            command.Parameters.AddWithValue("@PrivateSignedPreKey", privateSPK);
            command.ExecuteNonQuery();

            //Store the public handshake keys in the handshakevalues table.
            cmd_text = "INSERT INTO handshakevalues (UserID, PublicIDkey, PublicSignedPreKey, PreKeySignature) VALUES (@UserID, @PublicIDkey, @PublicSignedPreKey, @PreKeySignature);";
            command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@UserID", UserID);
            command.Parameters.AddWithValue("@PublicIDkey", publicIDkey);
            command.Parameters.AddWithValue("@PublicSignedPreKey", publicSPK);
            command.Parameters.AddWithValue("@PreKeySignature", preKeySignature);
            command.ExecuteNonQuery();

            return null;
        }

        //Generate a session token. Takes the amount of seconds the token is valid for.
        private static (string token, long expiration) createToken(int expiry_length)
        {
            //Random string.
            string token = Guid.NewGuid().ToString();

            //Get current unix time.
            long unix_time = DateTimeOffset.Now.ToUnixTimeSeconds();
            //The time at which the token wont be valid anymore.
            long expiration = unix_time + expiry_length;

            return (token, expiration);
        }

        //Set the management tunnel workers last heartbeat property to the current time so the connection isn't closed by the auditor.
        public static Dictionary<string, string>? managmentHeartbeat(MySqlConnection db, List<ManagmentWorker> managment_tunnel_workers, string username, string token)
        {
            //Make sure token is valid.
            if (!authenticateRequest(db, username, token))
            {
                return new Dictionary<string, string>() { { "error", "Invalid Session Token" } };
            }

            //Get the userID from the database.
            string cmd_text = "SELECT UserID FROM users WHERE Username = @Username;";
            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@Username", username);

            //Make sure another thread isn't reading from the database.
            lock (db)
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();

                    int UserID = reader.GetInt32(0);
                    //Search the open tunnel workers with the userID.
                    int index = BinarySearch.searchTunnelWorker(managment_tunnel_workers, UserID);
                    //If the index is -1 then a worker doesn't exist for that UserID.
                    if (index == -1)
                    {
                        return new Dictionary<string, string>() { { "error", "No Worker Found" } };
                    }

                    //Update the workers last hearbeat to the current time.
                    managment_tunnel_workers[index].last_heartbeat = DateTimeOffset.Now.ToUnixTimeSeconds();
                }
            }
            
            return null;
        }

        //Endpoint to create an account.  
        public static Dictionary<string, string>? createAccount(MySqlConnection db, FriendGraph graph, string username, string authentication_code)
        {
            //Query the database to make sure the username isn't taken.
            string cmd_text = "SELECT EXISTS(SELECT * FROM users WHERE Username = @username);";
            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@username", username);

            //Make sure another thread isn't reading from the database.
            lock (db)
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    //Advances the reader to the next record.
                    reader.Read();

                    //Return an error if the username exists.
                    if (reader.GetInt32(0) > 0)
                    {
                        return new Dictionary<string, string>()
                        {
                            {"error", "The username already exisits." }
                        };
                    }
                }
            }
            
            //Hash the authentication code.
            string authentication_code_hash = Hash.generatePasswordHash(authentication_code, 8);

            //Store the hashed authentication code with the username in the database. 
            cmd_text = "INSERT INTO users (Username, AuthenticationCode) VALUES (@username, @authentication_code);";
            command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@authentication_code", authentication_code_hash);
            command.ExecuteNonQuery();

            graph.createAccount(username);

            //Create a token.
            (string token, long expiration) = createToken(token_expiration);

            //Insert the token into the database;
            cmd_text = "INSERT INTO tokens (UserID, ExpiryDate, Token) VALUES ((SELECT UserID FROM users WHERE Username = @Username), @ExpiryDate, @Token)";
            command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@ExpiryDate", expiration);
            command.Parameters.AddWithValue("@Token", token);
            command.ExecuteNonQuery();

            //Return the session token so the user can store it.
            return new Dictionary<string, string>()
            {
                {"session_token", token }
            };
        }

        //Returns true is there has been a fatal error.
        private static bool createConversations(MySqlConnection db, string username, string recipient_username)
        {
            string cmd_text = "SELECT UserID FROM users WHERE Username = @username OR Username = @recipient_username";
            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@recipient_username", recipient_username);

            int user_ID_1;
            int user_ID_2;

            lock (db)
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read()) return true;
                    user_ID_1 = reader.GetInt32(0);

                    if (!reader.Read()) return true;
                    user_ID_2 = reader.GetInt32(0);
                }
            }

            cmd_text = "INSERT INTO Conversations (Source, Destination) VALUES (@User1, @User2); INSERT INTO Conversations (Source, Destination) VALUES (@User2, @User1);";
            command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@User1", user_ID_1);
            command.Parameters.AddWithValue("@User2", user_ID_2);

            lock (db)
            {
                command.ExecuteNonQuery();
            }

            return false;
        }

        private static void uploadChainKeys(MySqlConnection db, string PrivateChainKey, string PrivateRatchetKey, string PublicRatchetKey, int ConversationID)
        {
            string cmd_text = "INSERT INTO chainkeys (PrivateChainKey, PrivateRatchetKey, PublicRatchetKey, IsNextMessageInAChain, CurrentSequenceCount, ConversationID) " +
                "VALUES (@PrivateChainKey, @PrivateRachetKey, @PublicRachetKey, 1, 1, @ConversationID);";
            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@PrivateChainKey", PrivateChainKey);
            command.Parameters.AddWithValue("@PrivateRachetKey", PrivateRatchetKey);
            command.Parameters.AddWithValue("@PublicRachetKey", PublicRatchetKey);
            command.Parameters.AddWithValue("@ConversationID", ConversationID);
            command.ExecuteNonQuery();
        }

        private static int getConversationID(MySqlConnection db, string source, string destination)
        {
            int conversationID = -1;

            string cmd_text = "SELECT ConversationID FROM conversations WHERE Source = (SELECT UserID FROM users WHERE Username = @Source) AND Destination = (SELECT UserID FROM users WHERE Username = @Destination)";
            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@Source", source);
            command.Parameters.AddWithValue("@Destination", destination);

            lock (db)
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        conversationID = reader.GetInt32(0);
                    }
                }
            }

            return conversationID;
        }

        private static void uploadX3DHhandshake(MySqlConnection db, string PublicEphemeralKey, string Identifier, int ConversationID)
        {
            string cmd_text = "INSERT INTO x3dhhandshake (PublicEphemeralKey, PreKeyID, ConversationID) " +
                "VALUES (@PublicEphemeralKey, (SELECT PreKeyID FROM publicprekeys WHERE Identifier = @Identifier), @ConversationID);";
            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@PublicEphemeralKey", PublicEphemeralKey);
            command.Parameters.AddWithValue("@Identifier", Identifier);
            command.Parameters.AddWithValue("@ConversationID", ConversationID);
            lock (db) command.ExecuteNonQuery();
        }

        private static void updateChainKeys(MySqlConnection db, string PrivateChainKey, string PrivateRatchetKey, string PublicRatchetKey, int ConversationID)
        {
            bool record_exists = false;

            string cmd_text = "SELECT EXISTS(SELECT * FROM chainkeys WHERE ConversationID=@ConversationID);";
            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@ConversationID", ConversationID);

            //Make sure another thread isn't reading from the database.
            lock (db)
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    //Advances the reader to the next record.
                    reader.Read();

                    if (reader.GetInt32(0) > 0)
                    {
                        record_exists = true;
                    }
                }
            }

            if (record_exists)
            {
                cmd_text = "UPDATE chainkeys SET PrivateChainKey=@PrivateChainKey, PrivateRatchetKey=@PrivateRatchetKey, PublicRatchetKey=@PublicRatchetKey, IsNextMessageInAChain=1, CurrentSequenceCount=1 WHERE ConversationID=@ConversationID";
                command = new MySqlCommand(cmd_text, db);
                command.Parameters.AddWithValue("@PrivateChainKey", PrivateChainKey);
                command.Parameters.AddWithValue("@PrivateRatchetKey", PrivateRatchetKey);
                command.Parameters.AddWithValue("@PublicRatchetKey", PublicRatchetKey);
                command.Parameters.AddWithValue("@ConversationID", ConversationID);
                lock (db) command.ExecuteNonQuery();
            }
            else
            {
                uploadChainKeys(db, PrivateChainKey, PrivateRatchetKey, PublicRatchetKey, ConversationID);
            }
        }

        public static Dictionary<string, string>? sendMessage(APIworker worker, MySqlConnection db, string username, string token, string recipient_username, string message_data, Dictionary<string, string> request_data)
        {
            //Make sure token is valid.
            if (!authenticateRequest(db, username, token))
            {
                return new Dictionary<string, string>() { { "error", "Invalid Session Token" } };
            }

            string message_type = request_data["message_type"];

            int sequence_count = 1;
            int conversationID = -1;

            string cmd_text;
            MySqlCommand command;

            switch (message_type)
            {
                case "initial_message":
                    bool error = createConversations(db, username, recipient_username);
                    if (error) throw new Exception("Error Whilst Creating Conversations");

                    conversationID = getConversationID(db, username, recipient_username);

                    uploadChainKeys(db, request_data["encrypted_chain_key"], request_data["encrypted_ratchet_private_key"], request_data["public_ratchet_key"], conversationID);
                    uploadX3DHhandshake(db, request_data["public_ephemeral_key"], request_data["prekey_identity"], conversationID);

                    createChat(db, username, recipient_username);
                    break;

                case "first_in_chain":
                    conversationID = getConversationID(db, username, recipient_username);
                    updateChainKeys(db, request_data["encrypted_chain_key"], request_data["encrypted_ratchet_private_key"], request_data["public_ratchet_key"], conversationID);

                    int recipient_conversationID = getConversationID(db, recipient_username, username);
                    cmd_text = "UPDATE chainkeys SET IsNextMessageInAChain=0 WHERE ConversationID=@ConversationID";
                    command = new MySqlCommand(cmd_text, db);
                    command.Parameters.AddWithValue("@ConversationID", recipient_conversationID);
                    lock (db) command.ExecuteNonQuery();

                    break;

                case "chain_message":
                    sequence_count = Convert.ToInt32(request_data["sequence_count"]);
                    conversationID = getConversationID(db, username, recipient_username);

                    cmd_text = "UPDATE chainkeys SET CurrentSequenceCount=@CurrentSequenceCount WHERE ConversationID=@ConversationID";
                    command = new MySqlCommand(cmd_text, db);
                    command.Parameters.AddWithValue("@CurrentSequenceCount", sequence_count);
                    command.Parameters.AddWithValue("@ConversationID", conversationID);
                    lock (db) command.ExecuteNonQuery();

                    break;
            }

            cmd_text = "INSERT INTO messages (SequenceCount, MessageData, TimeSent, MessageType, ConversationID) " +
                "VALUES (@SequenceCount, @MessageData, CURRENT_TIMESTAMP(), @MessageType, @ConversationID)";
            command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@SequenceCount", sequence_count);
            command.Parameters.AddWithValue("@MessageData", message_data);
            command.Parameters.AddWithValue("@MessageType", message_type);
            command.Parameters.AddWithValue("@ConversationID", conversationID);

            lock (db) command.ExecuteNonQuery();

            Dictionary<string, string> management_request = new Dictionary<string, string>()
            {
                { "URL", "\\managment_tunnel\\message_ready" },
                { "sender", username }
            };

            worker.createManagmentRequest(recipient_username, management_request);

            return null;
        }

        private static Dictionary<string, string> fetchPreKeyBundle(MySqlConnection db, string username)
        {
            string cmd_text = "SELECT publicprekeys.PreKey, publicprekeys.Identifier, handshakevalues.PublicIDkey, handshakevalues.PublicSignedPreKey, handshakevalues.PreKeySignature, publicprekeys.PreKeyID " +
            "FROM publicprekeys INNER JOIN handshakevalues ON publicprekeys.UserID = handshakevalues.UserID " +
            "WHERE publicprekeys.UserID = (SELECT UserID FROM Users WHERE username = @username) AND publicprekeys.HasBeenUsed = 0 LIMIT 1;";

            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@username", username);

            Dictionary<string, string> pre_key_bundle;
            int pre_keyID = -1;

            lock (db)
            {
                //Create a reader to read the query response.
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return new Dictionary<string, string>() { { "error", "Out Of Pre Keys. Please wait for the user to come online again." } };
                    }

                    string public_one_time_prekey = reader.GetString(0);
                    string prekey_identity = reader.GetString(1);
                    string public_reciever_ID = reader.GetString(2);
                    string public_signed_pre_key = reader.GetString(3);
                    string pre_key_signature = reader.GetString(4);
                    pre_keyID = reader.GetInt32(5);

                    pre_key_bundle = new Dictionary<string, string>()
                    {
                        { "public_signed_pre_key", public_signed_pre_key },
                        { "public_reciever_ID", public_reciever_ID },
                        { "public_one_time_prekey", public_one_time_prekey },
                        { "prekey_identity", prekey_identity },
                        { "pre_key_signature", pre_key_signature }
                    };
                }
            }

            cmd_text = "UPDATE publicprekeys SET HasBeenUsed = 1 WHERE PreKeyID = @prekeyID";
            command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@prekeyID", pre_keyID);
            lock (db) command.ExecuteNonQuery();

            return pre_key_bundle;
        }

        public static Dictionary<string, string>? requestMessageSend(APIworker worker, MySqlConnection db, string username, string token, string recipient_username)
        {
            //Make sure token is valid.
            if (!authenticateRequest(db, username, token))
            {
                return new Dictionary<string, string>() { { "error", "Invalid Session Token" } };
            }

            Dictionary<string, string> management_request = new Dictionary<string, string>()
            {
                { "URL", "\\managemnt_tunnel\\message_sending" },
                { "sender", username }
            };

            worker.createManagmentRequest(recipient_username, management_request, false);

            string cmd_text = "SELECT ConversationID FROM conversations WHERE " +
                "(Source = (SELECT UserID FROM users WHERE username = @username) OR Source = (SELECT UserID FROM users WHERE username = @recipient_username)) " +
                "AND (Destination = (SELECT UserID FROM users WHERE username = @username) OR Destination = (SELECT UserID FROM users WHERE username = @recipient_username)) " +
                "LIMIT 1";
            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@recipient_username", recipient_username);

            Dictionary<string, string> response = new Dictionary<string, string>();
            bool is_initial_message = false;

            lock (db)
            {
                //Create a reader to read the query response.
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    //If no conversations exists.
                    if (!reader.Read())
                    {
                        is_initial_message = true;
                    }
                }
            }

            //Message is the initial message.
            if (is_initial_message)
            {
                //Send client pre key bundle.
                response = fetchPreKeyBundle(db, recipient_username);
                response["message_send_type"] = "initial_message";

                return response;
            }

            int ConversationID = getConversationID(db, username, recipient_username);

            cmd_text = "SELECT IsNextMessageInAChain, PrivateChainKey, CurrentSequenceCount FROM chainkeys WHERE ConversationID = @ConversationID";
            command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@ConversationID", ConversationID);

            lock (db)
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    bool contains_in_chain = !reader.IsDBNull(0);

                    string private_chain_key = reader.GetString(1);

                    //Message is in a chain.
                    if (contains_in_chain && reader.GetBoolean(0))
                    {
                        int current_sequence_count = reader.GetInt32(2);

                        response["message_send_type"] = "chain_message";
                        response["private_chain_key"] = private_chain_key;
                        response["sequence_count"] = Convert.ToString(current_sequence_count);

                        return response;
                    }

                    //First message in chain.
                    response["message_send_type"] = "first_in_chain";
                    response["private_chain_key"] = private_chain_key;
                }
            }

            int recipientConversationID = getConversationID(db, recipient_username, username);

            cmd_text = "SELECT PublicRatchetKey FROM chainkeys WHERE ConversationID = @RecipientConversationID";
            command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@RecipientConversationID", recipientConversationID);

            lock (db)
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    response["public_ratchet_key"] = reader.GetString(0);
                }
            }

            return response;
        }

        public static Dictionary<string, string>? recieveMessage(MySqlConnection db, string username, string token, string sender)
        {
            //Make sure token is valid.
            if (!authenticateRequest(db, username, token))
            {
                return new Dictionary<string, string>() { { "error", "Invalid Session Token" } };
            }

            //Where the message is from the sender to the username.
            int conversationID = getConversationID(db, sender, username);

            string cmd_text = "SELECT SequenceCount, MessageData, MessageType, TimeSent, MessageID FROM messages WHERE ConversationID = @ConversationID ORDER BY TimeSent";
            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@ConversationID", conversationID);

            int sequence_count = 0;
            string message_data;
            string message_type;
            DateTime time_sent;
            int message_id;

            lock (db)
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    sequence_count = reader.GetInt32(0);
                    message_data = reader.GetString(1);
                    message_type = reader.GetString(2);
                    time_sent = reader.GetDateTime(3);
                    message_id = reader.GetInt32(4);
                }
            }

            cmd_text = "DELETE FROM messages WHERE MessageID=@MessageID";
            command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@MessageID", message_id);
            lock (db) command.ExecuteNonQuery();

            string encoded_time_sent = Convert.ToBase64String(BitConverter.GetBytes(time_sent.Ticks));

            Dictionary<string, string> response = new Dictionary<string, string>()
            {
                { "message_type", message_type },
                { "encrypted_message", message_data },
                { "time_sent", encoded_time_sent }
            };

            switch (message_type)
            {
                case "initial_message":
                    cmd_text = "SELECT chainkeys.PublicRatchetKey, publicprekeys.Identifier, x3dhhandshake.PublicEphemeralKey, handshakevalues.PublicIDkey, publicprekeys.PreKeyID FROM chainkeys " +
                        "INNER JOIN x3dhhandshake ON x3dhhandshake.ConversationID = chainkeys.ConversationID " +
                        "INNER JOIN publicprekeys ON publicprekeys.PreKeyID = x3dhhandshake.PreKeyID " +
                        "INNER JOIN conversations ON conversations.ConversationID = chainkeys.ConversationID " +
                        "INNER JOIN handshakevalues ON handshakevalues.UserID = conversations.Source " +
                        "WHERE chainkeys.ConversationID = @ConversationID;";

                    command = new MySqlCommand(cmd_text, db);
                    command.Parameters.AddWithValue("@ConversationID", conversationID);

                    int pre_keyID = -1;

                    lock (db)
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            response["public_ratchet_key"] = reader.GetString(0);
                            response["prekey_identity"] = reader.GetString(1);
                            response["public_ephemeral_key"] = reader.GetString(2);
                            response["public_sender_ID"] = reader.GetString(3);
                            pre_keyID = reader.GetInt32(4);
                        }
                    }

                    cmd_text = "DELETE FROM x3dhhandshake WHERE PreKeyID = @prekeyID1; DELETE FROM privateprekeys WHERE AssociatedPreKeyID = @prekeyID2; DELETE FROM publicprekeys WHERE PreKeyID = @prekeyID3;";
                    command = new MySqlCommand(cmd_text, db);
                    command.Parameters.AddWithValue("@prekeyID1", pre_keyID);
                    command.Parameters.AddWithValue("@prekeyID2", pre_keyID);
                    command.Parameters.AddWithValue("@prekeyID3", pre_keyID);
                    lock (db) command.ExecuteNonQuery();

                    createChat(db, username, sender);
                    break;

                case "first_in_chain":
                    int ConversationID = getConversationID(db, username, sender);

                    cmd_text = "SELECT PrivateRatchetKey, PrivateChainKey FROM chainkeys WHERE ConversationID = @ConversationID";
                    command = new MySqlCommand(cmd_text, db);
                    command.Parameters.AddWithValue("@ConversationID", ConversationID);

                    lock (db)
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            response["private_ratchet_key"] = reader.GetString(0);
                            response["private_chain_key"] = reader.GetString(1);
                        }
                    }

                    int senderConversationID = getConversationID(db, sender, username);

                    cmd_text = "SELECT PublicRatchetKey FROM chainkeys WHERE ConversationID = @ConversationID";
                    command = new MySqlCommand(cmd_text, db);
                    command.Parameters.AddWithValue("@ConversationID", senderConversationID);

                    lock (db)
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            response["public_ratchet_key"] = reader.GetString(0);
                        }
                    }
                    break;

                case "chain_message":
                    int reciever_conversationID = getConversationID(db, username, sender);

                    cmd_text = "SELECT PrivateChainKey FROM chainkeys WHERE ConversationID = @ConversationID";
                    command = new MySqlCommand(cmd_text, db);
                    command.Parameters.AddWithValue("@ConversationID", reciever_conversationID);

                    lock (db)
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            response["private_chain_key"] = reader.GetString(0);
                        }
                    }
                    response["sequence_count"] = Convert.ToString(sequence_count);

                    break;
            }
            return response;
        }

        public static Dictionary<string, string>? uploadOldMessage(MySqlConnection db, string username, string token, string sender, string recipient, Dictionary<string, string> request)
        {
            //Make sure token is valid.
            if (!authenticateRequest(db, username, token))
            {
                return new Dictionary<string, string>() { { "error", "Invalid Session Token" } };
            }

            string cmd_text;
            MySqlCommand command;
            int ConversationID;
            DateTime time_sent;

            if (username == recipient)
            {
                ConversationID = getConversationID(db, username, sender);

                switch (request["message_type"])
                {
                    case "initial_message":
                        cmd_text = "INSERT INTO chainkeys (PrivateChainKey, ConversationID) VALUES (@PrivateChainKey, @ConversationID)";
                        command = new MySqlCommand(cmd_text, db);
                        command.Parameters.AddWithValue("@PrivateChainKey", request["private_chain_key"]);
                        command.Parameters.AddWithValue("@ConversationID", ConversationID);
                        lock (db) command.ExecuteNonQuery();

                        uploadPreKey(db, username, token, request["new_pre_key_identifier"], request["new_private_pre_key"], request["new_public_pre_key"]);
                        break;

                    case "first_in_chain":
                        cmd_text = "UPDATE chainkeys SET PrivateChainKey=@PrivateChainKey WHERE ConversationID=@ConversationID";
                        command = new MySqlCommand(cmd_text, db);
                        command.Parameters.AddWithValue("@PrivateChainKey", request["private_chain_key"]);
                        command.Parameters.AddWithValue("@ConversationID", ConversationID);
                        lock (db) command.ExecuteNonQuery();
                        break;
                }

                time_sent = new DateTime(BitConverter.ToInt64(Convert.FromBase64String(request["time_sent"])));
            }
            else
            {
                ConversationID = getConversationID(db, username, recipient);
                time_sent = DateTime.Now;
            }

            

            cmd_text = "INSERT INTO oldmessages (MessageData, TimeSent, Sender, ConversationID) VALUES (@MessageData, @TimeSent, @Sender, @ConversationID)";
            command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@MessageData", request["encrypted_message"]);
            command.Parameters.AddWithValue("@TimeSent", time_sent);
            command.Parameters.AddWithValue("@Sender", sender);
            command.Parameters.AddWithValue("@ConversationID", ConversationID);
            lock (db) command.ExecuteNonQuery();

            return null;
        }

        public static Dictionary<string, string>? downloadOldMessages(MySqlConnection db, string username, string token, string recipient)
        {
            //Make sure token is valid.
            if (!authenticateRequest(db, username, token))
            {
                return new Dictionary<string, string>() { { "error", "Invalid Session Token" } };
            }

            int conversationID = getConversationID(db, username, recipient);

            if (conversationID == -1)
            {
                return null;
            }

            List<Dictionary<string, string>> messages = new List<Dictionary<string, string>>();

            string cmd_text = "SELECT Sender, MessageData FROM oldmessages WHERE ConversationID=@ConversationID ORDER BY TimeSent";
            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@ConversationID",conversationID);
            
            lock (db)
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Dictionary<string, string> message = new Dictionary<string, string>()
                        {
                            { "sender", reader.GetString(0) },
                            { "message_data", reader.GetString(1) }
                        };

                        messages.Add(message);
                    }
                }
            }

            string encoded_messages = JsonSerializer.Serialize(messages);

            return new Dictionary<string, string>()
            {
                { "messages", encoded_messages }
            };
        }

        public static Dictionary<string, string>? downloadPreKeys(MySqlConnection db, string username, string token)
        {
            //Make sure token is valid.
            if (!authenticateRequest(db, username, token))
            {
                return new Dictionary<string, string>() { { "error", "Invalid Session Token" } };
            }

            string cmd_text = "SELECT publicprekeys.Identifier, privateprekeys.PreKey " +
            "FROM publicprekeys INNER JOIN privateprekeys ON privateprekeys.AssociatedPreKeyID=publicprekeys.PreKeyID " +
            "WHERE publicprekeys.UserID = (SELECT UserID FROM Users WHERE Username = @Username)";

            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@Username", username);

            List<string> pre_keys = new List<string>();

            //Make sure another thread isn't reading from the database.
            lock (db)
            {
                //Create a reader to read the query response.
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string identifier = reader.GetString(0);
                        string encrypted_private_value = reader.GetString(1);

                        Dictionary<string, string> pre_key = new Dictionary<string, string>()
                        {
                            { "identifier", identifier },
                            { "private_value", encrypted_private_value }
                        };

                        string serialized_pre_key = JsonSerializer.Serialize(pre_key);
                        pre_keys.Add(serialized_pre_key);
                    }
                }
            }

            string serialized_pre_keys = JsonSerializer.Serialize(pre_keys);
            return new Dictionary<string, string>()
            {
                { "pre_keys", serialized_pre_keys }
            };
        }

        //Generate a token which will be used to identify a logged in user.
        public static Dictionary<string, string>? generateToken(MySqlConnection db, string username, string authentication_code)
        {
            //Get stored authenticationcode hash and the userID from the database.
            string cmd_text = "SELECT Username, AuthenticationCode, UserID FROM users WHERE username = @username";
            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@username", username);

            //Incase authentication is succesful, store the userID.
            string userID = "";

            //Make sure another thread isn't reading from the database.
            lock (db)
            {
                //Create a reader to read the query response.
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    //False if no records are returned.
                    if (!reader.Read())
                    {
                        return new Dictionary<string, string>()
                        {
                            {"error", "The username does not exist." }
                        };
                    }

                    string db_username = reader.GetString(0);
                    if (username != db_username)
                    {
                        return new Dictionary<string, string>()
                        {
                            {"error", "The username does not exist." }
                        };
                    }

                    //Store the authentication code hash from the database.
                    string db_authentication_code = reader.GetString(1);

                    //Verify the given authentication code with the hash from the database.
                    if (!Hash.verifyPasswordHash(db_authentication_code, authentication_code, 8))
                    {
                        return new Dictionary<string, string>()
                        {
                            {"error", "The password is incorrect." }
                        };
                    }

                    //Store the userID needed for the next query.
                    userID = reader.GetString(2);
                }
            }

            //Create a token.
            (string token, long expiration) = createToken(token_expiration);

            //Update the token value and the expiry date.
            cmd_text = "UPDATE tokens SET ExpiryDate = @ExpiryDate, Token = @Token WHERE UserID = @UserID";
            command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@UserID", userID);
            command.Parameters.AddWithValue("@ExpiryDate", expiration);
            command.Parameters.AddWithValue("@Token", token);
            command.ExecuteNonQuery();

            //Return the session token so the user can store it.
            return new Dictionary<string, string>()
            {
                {"session_token", token }
            };
        }

        public static Dictionary<string, string>? sendFriendRequest(MySqlConnection db, APIworker worker, string username, string token, string recipient)
        {
            //Make sure token is valid.
            if (!authenticateRequest(db, username, token))
            {
                return new Dictionary<string, string>() { { "error", "Invalid Session Token" } };
            }

            string cmd_txt = "SELECT UserID FROM users WHERE Username = @recipient";
            MySqlCommand command = new MySqlCommand(cmd_txt, db);
            command.Parameters.AddWithValue("@recipient", recipient);

            lock (db)
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return new Dictionary<string, string>() { { "fatal_error", "The username doesn't exist." } };
                    }
                }
            }

            cmd_txt = "SELECT requestID FROM friendrequests WHERE " + 
                "Sender = (SELECT UserID FROM users WHERE Username = @Sender) AND " +
                "Recipient = (SELECT UserID FROM users WHERE Username = @Recipient) " +
                "LIMIT 1";
            command = new MySqlCommand(cmd_txt, db);
            command.Parameters.AddWithValue("@Sender", username);
            command.Parameters.AddWithValue("@Recipient", recipient);

            lock (db)
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return null;
                    }
                }
            }

            cmd_txt = "INSERT INTO friendrequests (Sender, Recipient) VALUES (" +
                "(SELECT UserID FROM users WHERE Username = @Sender)," +
                "(SELECT UserID FROM users WHERE Username = @Recipient))";
            command = new MySqlCommand(cmd_txt, db);
            command.Parameters.AddWithValue("@Sender", username);
            command.Parameters.AddWithValue("@Recipient", recipient);
            lock (db) command.ExecuteNonQuery();

            Dictionary<string, string> request = new Dictionary<string, string>()
            {
                { "URL", "\\management_tunnel\\friend_request"},
                { "requester", username }
            };

            worker.createManagmentRequest(recipient, request);
            return null;
        }

        static public Dictionary<string, string>? acceptFriendRequest(MySqlConnection db, FriendGraph graph, string username, string token, string requester)
        {
            //Make sure token is valid.
            if (!authenticateRequest(db, username, token))
            {
                return new Dictionary<string, string>() { { "error", "Invalid Session Token" } };
            }

            string cmd_txt = "DELETE FROM friendrequests WHERE Sender = (SELECT UserID FROM users WHERE username = @requester) AND Recipient = (SELECT UserID FROM users WHERE username = @username)";
            MySqlCommand command = new MySqlCommand(cmd_txt, db);
            command.Parameters.AddWithValue("@requester", requester);
            command.Parameters.AddWithValue("@username", username);
            lock (db) command.ExecuteNonQuery();

            graph.addFriend(username, requester);
            graph.addFriend(requester, username);

            return null;
        }

        static public Dictionary<string, string> getFriendsAndRequests(MySqlConnection db, FriendGraph graph, string username, string token)
        {
            //Make sure token is valid.
            if (!authenticateRequest(db, username, token))
            {
                return new Dictionary<string, string>() { { "error", "Invalid Session Token" } };
            }

            string cmd_text = "SELECT users.Username FROM users " +
                "INNER JOIN friendrequests ON users.UserID = friendrequests.Sender " +
                "WHERE friendrequests.Recipient = (SELECT UserID FROM users WHERE Username = @username);";
            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@username", username);

            List<string> requests = new List<string>();

            lock (db)
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        requests.Add(reader.GetString(0));
                    }
                }
            }

            cmd_text = "SELECT users.Username FROM users " +
                "INNER JOIN friendrequests ON users.UserID = friendrequests.Recipient " +
                "WHERE friendrequests.Sender = (SELECT UserID FROM users WHERE Username = @username)";
            command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@username", username);

            List<string> sent_requests = new List<string>();

            lock (db)
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sent_requests.Add(reader.GetString(0));
                    }
                }
            }

            List<string> friends = graph.getFriends(username);

            Dictionary<string, string> response = new Dictionary<string, string>()
            {
                { "requests", JsonSerializer.Serialize(requests) },
                { "friends", JsonSerializer.Serialize(friends) },
                { "sent_requests", JsonSerializer.Serialize(sent_requests) }
            };

            return response;
        }

        static private void createChat(MySqlConnection db, string username, string recipient)
        {
            string cmd_txt = "INSERT INTO openedchats (OwnerID, RecipientID) VALUES (" +
                "(SELECT UserID FROM users WHERE Username = @username)," +
                "(SELECT UserID FROM users WHERE Username = @recipient))";

            MySqlCommand command = new MySqlCommand(cmd_txt, db);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@recipient", recipient);
            lock (db) command.ExecuteNonQuery();
        }

        static public Dictionary<string, string> getOpenedChats(MySqlConnection db, string username, string token)
        {
            //Make sure token is valid.
            if (!authenticateRequest(db, username, token))
            {
                return new Dictionary<string, string>() { { "error", "Invalid Session Token" } };
            }

            string cmd_text = "SELECT users.Username FROM users " +
                "INNER JOIN openedchats ON OpenedChats.RecipientID = users.UserID " +
                "WHERE OwnerID = (SELECT UserID FROM users WHERE username = @username) ";

            MySqlCommand command = new MySqlCommand(cmd_text, db);
            command.Parameters.AddWithValue("@username", username);

            List<Dictionary<string, string>> chats = new List<Dictionary<string, string>>();

            lock (db)
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string recipient_username = reader.GetString(0);

                        Dictionary<string, string> chat = new Dictionary<string, string>()
                        {
                            { "recipient", recipient_username },
                        };

                        chats.Add(chat);
                    }
                }
            }

            Dictionary<string, string> response = new Dictionary<string, string>() 
            {
                { "chats", JsonSerializer.Serialize(chats) }
            };

            return response;
        }
    }
}
