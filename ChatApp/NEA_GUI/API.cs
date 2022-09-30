using System.Text;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Timers;

using Matthew_Tranmer_NEA.Generic;
using Matthew_Tranmer_NEA.Networking;

namespace NEA_GUI
{
    internal static class API
    {
        //10.144.197.214
        private static IPEndPoint end_point = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 9921);
        //private static IPEndPoint end_point = new IPEndPoint(new IPAddress(new byte[] { 10, 144, 197, 214 }), 9921);
        private static System.Timers.Timer keep_alive_timer = new System.Timers.Timer(10500);
        
        private static EncryptedSocketWrapper? wrapper = null;

        private static void keepAliveTimeout(object? s, EventArgs a)
        {
            wrapper = null;
        }

        private static Dictionary<string, string>? sendRequest(EncryptedSocketWrapper socket_wrapper, Dictionary<string, string> request)
        {
            //Serialize the request body and send it encrypted to the server.
            string json_request = JsonSerializer.Serialize(request);
            socket_wrapper.send(Encoding.UTF8.GetBytes(json_request));

            //Recieve the JSON response body.
            string json_response = Encoding.UTF8.GetString(socket_wrapper.recieve());

            //Deserialize the response body into a dictionary. 
            Dictionary<string, string>? response = JsonSerializer.Deserialize<Dictionary<string, string>>(json_response);
            return response;
        }

        //Creates a request to the server.
        public static (Dictionary<string, string>? response_body, bool fatal_error) apiRequest(Dictionary<string, string> request)
        {
            try
            {
                lock (keep_alive_timer)
                {
                    if (wrapper == null)
                    {
                        //Create a socket.
                        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        const int timeout = 5000;
                        socket.ReceiveTimeout = timeout;
                        socket.SendTimeout = timeout;

                        //Connect the socket to the server.
                        socket.Connect(end_point);

                        //Create an encrypted socket wrapper over the socket.
                        wrapper = new EncryptedSocketWrapper(socket, ApplicationValues.server_pk);

                        keep_alive_timer.Stop();
                        keep_alive_timer.Start();
                    }
                    else
                    {
                        keep_alive_timer.Stop();
                        keep_alive_timer.Start();
                    }
                }

                Dictionary<string, string>? response = sendRequest(wrapper, request);

                //If the server returns a fatal error, then show an error popup.
                if (response != null && response.ContainsKey("fatal_error"))
                {
                    Functions.showError(response["fatal_error"] + "\nSource: Server");
                    return (response, true);
                }

                if (response != null && response.ContainsKey("error") && response["error"] == "Invalid Session Token")
                {
                    Functions.showError("The session has expired, the application will now restart.");
                    Functions.fatalRestart();
                }

                //Return the response body.
                return (response, false);
            }
            catch (Exception exeption)
            {
                Functions.showError(exeption.Message);
                return (null, true);
            }
        }

        public static (Dictionary<string, string>? response, EncryptedSocketWrapper socket_wrapper) createManagmentTunnel()
        {
            //Create a socket.
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SendTimeout = 1000;

            //Connect the socket to the server.
            socket.Connect(end_point);

            Dictionary<string, string> request = new Dictionary<string, string>()
            {
                { "URL", "\\api\\management\\management_tunnel" },
                { "username", ApplicationValues.username },
                { "token",  ApplicationValues.session_token },
            };

            //Create an encrypted socket wrapper over the socket.
            EncryptedSocketWrapper socket_wrapper = new EncryptedSocketWrapper(socket, ApplicationValues.server_pk);

            Dictionary<string, string>? response = sendRequest(socket_wrapper, request);
            return (response, socket_wrapper);
        }
    }
}
