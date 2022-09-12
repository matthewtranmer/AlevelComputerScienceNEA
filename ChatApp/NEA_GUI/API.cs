using System.Text;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

using Matthew_Tranmer_NEA.Generic;
using Matthew_Tranmer_NEA.Networking;

namespace NEA_GUI
{
    internal static class API
    {
        private static IPEndPoint end_point = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 9921);

        private static Dictionary<string, string>? sendRequest(Socket socket, Dictionary<string, string> request)
        {
            //Create the public key.
            Coordinate signature_pk_coord = new Coordinate(BigInteger.Parse("89630596470571539848842129232432250117878455304252638950051962197460885296971"), BigInteger.Parse("81151631255409626093048500128742932208733034782939337963036891136896518229386"));
            EllipticCurvePoint public_key = new EllipticCurvePoint(signature_pk_coord, PreDefinedCurves.nist256);

            //Create an encrypted socket wrapper over the socket.
            EncryptedSocketWrapper socket_wrapper = new EncryptedSocketWrapper(socket);

            //Serialize the request body and send it encrypted to the server.
            string json_request = JsonSerializer.Serialize(request);
            socket_wrapper.sendSigned(Encoding.UTF8.GetBytes(json_request), public_key);

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
                //Create a socket.
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.ReceiveTimeout = 1000;
                socket.SendTimeout = 1000;

                //Connect the socket to the server.
                socket.Connect(end_point);

                Dictionary<string, string>? response = sendRequest(socket, request);

                //If the server returns a fatal error, then show an error popup.
                if (response != null && response.ContainsKey("fatal_error"))
                {
                    Functions.showError(response["fatal_error"] + "\nSource: Server");
                    return (response, true);
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

        public static (Dictionary<string, string>? response, Socket? socket) createManagmentTunnel()
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

            Dictionary<string, string>? response = sendRequest(socket, request);
            return (response, socket);
        }
    }
}
