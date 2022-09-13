using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Matthew_Tranmer_NEA_Server
{
    class FriendGraph
    {
        class GraphNode
        {
            public string username;
            public List<GraphNode> friends = new List<GraphNode>();

            public GraphNode(string username)
            {
                this.username = username;
            }
        }

        MySqlConnection db;
        List<GraphNode> nodes = new List<GraphNode>();

        public FriendGraph(MySqlConnection db)
        {
            Console.WriteLine("Creating Graph");
            this.db = db;

            string cmd_txt = "SELECT Username FROM users WHERE UserID = (SELECT UserID FROM friendgraphnodes)";
            MySqlCommand command = new MySqlCommand(cmd_txt);

            lock (db)
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        GraphNode node = new GraphNode(reader.GetString(0));
                        nodes.Add(node);
                    }
                }
            }

            foreach (var node in nodes)
            {
                cmd_txt = "SELECT Username FROM users WHERE UserID = (SELECT FriendNodeID FROM friendgraphlinks WHERE NodeID = (SELECT UserID FROM users WHERE Username = @username))";
                command = new MySqlCommand(cmd_txt);
                command.Parameters.AddWithValue("@username", node.username);

                lock (db)
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            addFriend(node.username, reader.GetString(0), false);
                        }
                    }
                }
            }
        }

        public void createAccount(string username)
        {
            GraphNode new_user = new GraphNode(username);

            string cmd_txt = "INSERT INTO friendgraphnodes (UserID) VALUES (SELECT UserID FROM users WHERE username = @username)";
            MySqlCommand command = new MySqlCommand(cmd_txt);
            command.Parameters.AddWithValue("@username", username);
            command.ExecuteNonQueryAsync();

            nodes.Add(new_user);
        }

        public void addFriend(string username, string friend, bool insert_into_database = true)
        {
            GraphNode? user_node = null;
            GraphNode? friend_node = null;

            foreach (GraphNode node in nodes)
            {
                if (user_node != null && friend_node != null) break;
                if (node.username == username) user_node = node;
                else if (node.username == friend) friend_node = node;
            }

            if (insert_into_database)
            {
                string cmd_txt = "INSERT INTO friendgraphlinks (NodeID, FriendNodeID) VALUES ( " +
                "SELECT NodeID FROM friendgraphnodes WHERE UserID = (SELECT UserID FROM users WHERE username = @username), " +
                "SELECT NodeID FROM friendgraphnodes WHERE UserID = (SELECT UserID FROM users WHERE username = @friend))";
                MySqlCommand command = new MySqlCommand(cmd_txt);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@friend", friend);
                command.ExecuteNonQueryAsync();
            }

            user_node.friends.Add(friend_node);
        }

        public List<string> getFriends(string username)
        {
            List<string> friends = new List<string>();
            GraphNode? user_node = null;

            foreach (GraphNode node in nodes)
            {
                if (node.username == username)
                {
                    user_node = node;
                    break;
                }
            }

            if (user_node.friends.Count > 0)
            {
                foreach (GraphNode node in user_node.friends)
                {
                    friends.Add(node.username);
                }

            }

            return friends;
        }

        public void removeFriend(string username, string friend)
        {
            GraphNode? user_node = null;
            GraphNode? friend_node = null;

            foreach (GraphNode node in nodes)
            {
                if (user_node == null && friend_node == null) break;
                if (node.username == username) user_node = node;
                else if (node.username == friend) friend_node = node;
            }

            user_node.friends.Remove(friend_node);
        }
    }
    
}
