using System.Text;
using System.Net.Sockets;

using Matthew_Tranmer_NEA.Networking;

namespace Matthew_Tranmer_NEA_Server.Workers
{
    internal class ManagmentWorker : IDisposable
    {
        public int UserID { get; }
        public long last_heartbeat;

        Socket raw_socket;
        EncryptedSocketWrapper socket_wrapper;

        public ManagmentWorker(int UserID, Socket raw_socket, EncryptedSocketWrapper socket_wrapper)
        {
            this.raw_socket = raw_socket;
            this.socket_wrapper = socket_wrapper;
            this.UserID = UserID;

            last_heartbeat = DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        public void send(string message)
        {
            socket_wrapper.send(Encoding.UTF8.GetBytes(message));
        }

        public void Dispose()
        {
            raw_socket.Close();
            raw_socket.Dispose();
        }
    }
}
