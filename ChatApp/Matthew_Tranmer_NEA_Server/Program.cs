using System.Net;
using System.Net.Sockets;
using System.Numerics;

using Matthew_Tranmer_NEA_Server.Workers;
using MySql.Data.MySqlClient;

namespace Matthew_Tranmer_NEA_Server
{
    internal class Program
    {
        public static void Display(string? value, ConsoleColor foreground_color)
        {
            ConsoleColor default_color = Console.ForegroundColor;
            Console.ForegroundColor = foreground_color;
            Console.WriteLine(value);
            Console.ForegroundColor = default_color;
        }

        static void Main()
        {
            string database_password;
            string str_private_key;

            //Read database password
            using (StreamReader reader = new StreamReader("databasepassword.txt"))
            {
                string? password = reader.ReadLine();
                if (password == null)
                {
                    throw new Exception("Password Error");
                }
                database_password = password;
            }

            //Read private key
            using (StreamReader reader = new StreamReader("privatekey.txt"))
            {
                string? key = reader.ReadLine();
                if (key == null)
                {
                    throw new Exception("Private Key Error");
                }
                str_private_key = key;
            }

            //Create a socket.
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 9921);

            //Bind the socket to the address and set it to listen mode.
            socket.Bind(ep);
            socket.Listen();

            Display("Server Started, Connecting To Database...", ConsoleColor.Blue);

            //Create a connection to the database.
            using (MySqlConnection db = new MySqlConnection($"server = localhost; userid = matthew; password = {database_password}; database = nea"))
            {
                db.Open();
                Display("Database Connected", ConsoleColor.Blue);

                Display("Creating Friend Graph Storage...", ConsoleColor.Green);
                FriendGraph friend_storage = new FriendGraph(db);
                Display("Friend Graph Storage Created", ConsoleColor.Green);

                //Private key used to sign header messages sent to the client.
                BigInteger private_key = BigInteger.Parse(str_private_key);
                //Create a new worker.
                APIworker worker = new APIworker(db, private_key, friend_storage);

                Display("Initialization Complete! Waiting For Requests...", ConsoleColor.Blue);

                while (true)
                {
                    try
                    {
                        Socket connection = socket.Accept();
                        _ = worker.start(connection);
                    }
                    catch (Exception exeption)
                    {
                        Console.WriteLine(exeption);
                    }
                }
            }
        }
    }
}