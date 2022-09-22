using System.Net;
using System.Net.Sockets;
using System.Numerics;

using Matthew_Tranmer_NEA_Server.Workers;
using MySql.Data.MySqlClient;

namespace Matthew_Tranmer_NEA_Server
{
    internal class Program
    {
        //Private Key: 47563461030232379838018606969981225219051095905568807231097714174869457590584
        //Public Key: (89630596470571539848842129232432250117878455304252638950051962197460885296971, 81151631255409626093048500128742932208733034782939337963036891136896518229386)

        public static void Display(string? value, ConsoleColor foreground_color)
        {
            ConsoleColor default_color = Console.ForegroundColor;
            Console.ForegroundColor = foreground_color;
            Console.WriteLine(value);
            Console.ForegroundColor = default_color;
        }

        static void Main()
        {
            //Create a socket.
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 9921);

            //IPEndPoint ep = new IPEndPoint(new IPAddress(new byte[] { 10, 144, 197, 214 }), 9921);


            //Bind the socket to the address and set it to listen mode.
            socket.Bind(ep);
            socket.Listen();

            Display("Server Started, Connecting To Database...", ConsoleColor.Blue);

            //Create a connection to the database.
            using (MySqlConnection db = new MySqlConnection("server = localhost; userid = matthew; password = matthew; database = nea"))
            {
                db.Open();
                Display("Database Connected", ConsoleColor.Blue);

                Display("Creating Friend Graph Storage...", ConsoleColor.Green);
                FriendGraph friend_storage = new FriendGraph(db);
                Display("Friend Graph Storage Created", ConsoleColor.Green);

                //Private key used to sign header messages sent to the client.
                BigInteger private_key = BigInteger.Parse("47563461030232379838018606969981225219051095905568807231097714174869457590584");
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