using System.Text.Json;

namespace NEA_GUI
{
    public partial class ChatSelector : Form
    {
        public string selected_username = ""; 

        public ChatSelector()
        {
            InitializeComponent();
        }

        private void returnUsername(string username)
        {
            selected_username = username;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void addFriend(string username)
        {
            Panel recipent_panel = new Panel();
            recipent_panel.Width = friendsFlowLayoutPanel.Width - 8;
            recipent_panel.Parent = friendsFlowLayoutPanel;
            recipent_panel.BackColor = Color.LightGray;
            recipent_panel.Height = 70;
            recipent_panel.Cursor = Cursors.Hand;

            Label name = new Label();
            name.Text = username;
            name.Parent = recipent_panel;

            Point point = name.Location;
            point.Offset(0, 10);
            name.Location = point;

            name.Width = recipent_panel.Width;
            name.TextAlign = ContentAlignment.MiddleCenter;
            name.AutoSize = false;
            name.Cursor = Cursors.Hand;
            name.AutoEllipsis = true;

            Button create_chat_button = new Button();
            create_chat_button.Text = "Create Chat";
            create_chat_button.Click += (s, e) => returnUsername(username);
            create_chat_button.Parent = recipent_panel;
            create_chat_button.AutoSize = true;

            point = create_chat_button.Location;
            point.Offset(50, 37);
            create_chat_button.Location = point;
        }

        bool has_friends = true;

        private void acceptRequest(string username, object? sender)
        {
            Button? button = sender as Button;
            button?.Parent.Dispose();

            if (requestsRecievedFlowLayoutPanel.Controls.Count == 0)
            {
                setNoFriendRequestRecieved();
            }

            if (!has_friends)
            {
                friendsFlowLayoutPanel.Controls.Clear();
                has_friends = true;
            }

            addFriend(username);

            Dictionary<string, string> request = new Dictionary<string, string>()
            {
                { "URL", "\\api\\friend\\accept_friend_request" },
                { "username", ApplicationValues.username },
                { "token", ApplicationValues.session_token },
                { "requester", username }
            };

            API.apiRequest(request);
        }

        private void addRequest(string username)
        {
            Panel request_panel = new Panel();
            request_panel.Width = requestsRecievedFlowLayoutPanel.Width - 8;
            request_panel.Parent = requestsRecievedFlowLayoutPanel;
            request_panel.BackColor = Color.LightGray;
            request_panel.Height = 70;
            request_panel.Cursor = Cursors.Hand;

            Label name = new Label();
            name.Text = username;
            name.Parent = request_panel;

            Point point = name.Location;
            point.Offset(0, 10);
            name.Location = point;

            name.Width = request_panel.Width;
            name.TextAlign = ContentAlignment.MiddleCenter;
            name.AutoSize = false;
            name.Cursor = Cursors.Hand;
            name.AutoEllipsis = true;

            Button accept_button = new Button();
            accept_button.Text = "Accept Request";
            accept_button.Click += (s, e) => acceptRequest(username, s);
            accept_button.Parent = request_panel;
            accept_button.AutoSize = true;

            point = accept_button.Location;
            point.Offset(40, 37);
            accept_button.Location = point;
        }

        private void addRequestSent(string username)
        {
            Panel request_panel = new Panel();
            request_panel.Width = requestsSentFlowLayoutPanel.Width - 8;
            request_panel.Parent = requestsSentFlowLayoutPanel;
            request_panel.BackColor = Color.LightGray;
            request_panel.Height = 50;

            Label name = new Label();
            name.Text = username;
            name.Parent = request_panel;

            Point point = name.Location;
            point.Offset(0, 10);
            name.Location = point;

            name.Width = request_panel.Width;
            name.TextAlign = ContentAlignment.MiddleCenter;
            name.AutoSize = false;
            name.Cursor = Cursors.Hand;
            name.AutoEllipsis = true;
        }

        private void ChatSelector_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> request = new Dictionary<string, string>()
            {
                { "URL", "\\api\\friend\\get_friends_and_requests" },
                { "username", ApplicationValues.username },
                { "token", ApplicationValues.session_token }
            };

            (Dictionary<string, string>? response, bool fatal_error) = API.apiRequest(request);
            if (fatal_error) return;
            
            List<string>? friends = JsonSerializer.Deserialize<List<string>>(response!["friends"]);
            if (friends != null && friends.Count > 0)
            {
                foreach (string friend in friends)
                {
                    addFriend(friend);
                }
            }
            else
            {
                has_friends = false;
                setNoFriends();
            }

            List<string>? requests = JsonSerializer.Deserialize<List<string>>(response!["requests"]);
            if (requests != null && requests.Count > 0)
            {
                foreach (string friend_request in requests)
                {
                    addRequest(friend_request);
                }
            }
            else
            {
                setNoFriendRequestRecieved();
            }

            List<string>? sent_requests = JsonSerializer.Deserialize<List<string>>(response!["sent_requests"]);
            if (sent_requests != null && sent_requests.Count > 0)
            {
                foreach (string sent_request in sent_requests)
                {
                    addRequestSent(sent_request);
                }
            }
            else
            {
                has_requests_sent = false;
                setNoFriendRequestSent();
            }
        }

        public void setNoFriends()
        {
            Label label = new Label();
            label.Text = "You have no friends";
            label.Height = 40;
            label.Width = friendsFlowLayoutPanel.Width - 6;
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleCenter;

            friendsFlowLayoutPanel.Controls.Add(label);
        }

        private void setNoFriendRequestRecieved()
        {
            Label label = new Label();
            label.Text = "You have no recieved friend requests";
            label.Height = 50;
            label.Width = friendsFlowLayoutPanel.Width - 6;
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleCenter;

            requestsRecievedFlowLayoutPanel.Controls.Add(label);
        }

        private void setNoFriendRequestSent()
        {
            Label label = new Label();
            label.Text = "You have no sent friend requests";
            label.Height = 50;
            label.Width = friendsFlowLayoutPanel.Width - 6;
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleCenter;

            requestsSentFlowLayoutPanel.Controls.Add(label);
            has_requests_sent = false;
        }

        bool has_requests_sent = true;

        private void sendRequestButton_Click(object sender, EventArgs e)
        {
            string recipient = usernameInput.Text;
            usernameInput.Clear();

            Dictionary<string, string> request = new Dictionary<string, string>()
            {
                { "URL", "\\api\\friend\\send_friend_request" },
                { "username", ApplicationValues.username },
                { "token", ApplicationValues.session_token },
                { "recipient", recipient }
            };

            (Dictionary<string, string> _, bool error) = API.apiRequest(request);
            if (!error)
            {
                if (!has_requests_sent)
                {
                    requestsSentFlowLayoutPanel.Controls.Clear();
                    has_requests_sent = true;
                }

                addRequestSent(recipient);
            }
        }
    }
}
