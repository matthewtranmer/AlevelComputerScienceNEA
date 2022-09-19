using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Matthew_Tranmer_NEA.Networking;
using Matthew_Tranmer_NEA.Generic;
using System.Timers;



namespace NEA_GUI
{
    public partial class MainForm : Form
    {

        public string current_recipient = "";

        public MainForm()
        {
            //Create Tunnel.
            createManagementTunnel();

            //Start Threads.
            Task.Factory.StartNew(managmentTunnelWorker);
            Task.Factory.StartNew(createHeartbeat);

            InitializeComponent();

            Text += $" - {ApplicationValues.username}";  

            //Initialize dropdownbox.
            sortByDropdown.Items.Add("Last Message");
            sortByDropdown.Items.Add("Oldest Message");
            sortByDropdown.Items.Add("A to Z");
            sortByDropdown.Items.Add("Z to A");
            sortByDropdown.SelectedIndex = 0;
        }

        //Thread which sends a heartbeat request to the server every 10 seconds.
        //The heartbeat notifies the server to keep the management tunnel open.
        private void createHeartbeat()
        {
            try
            {
                while (true)
                {
                    //Wait 10 seconds.
                    Thread.Sleep(10000);

                    Dictionary<string, string> heartbeat_request = new Dictionary<string, string>()
                    {
                        { "URL", "\\api\\management\\heartbeat" },
                        { "username", ApplicationValues.username },
                        { "token", ApplicationValues.session_token }
                    };

                    //Send heartbeat to sever.
                    (Dictionary<string, string>? response, bool fatal_error) = API.apiRequest(heartbeat_request);
                    //Server couldn't handle request.
                    if (fatal_error) throw new Exception("Fatal error with heartbeat");

                    //The token is incorrect so make user login again.
                    if (response != null && response.ContainsKey("error"))
                    {
                        Functions.showError("Session Has Expired");
                        Functions.fatalRestart();
                    }
                }
            }
            catch
            {
                //There has been an error with the heartbeat so restart.
                Functions.showError("Heartbeat Error. The Application Will Now Restart.");
                Functions.fatalRestart();
            }
        }

        private void disableMessageSend(object? source, ElapsedEventArgs e)
        {
            if (!message_recieved)
            {
                Send_Button.Enabled = true;
            }
        }

        bool message_recieved = false;

        //Thread which recieves requests from the server via the management tunnel.
        private void managmentTunnelWorker()
        {
            try
            {
                //Create encrypted socket wrapper to recieve encrypted messages from the server.
                EncryptedSocketWrapper wrapper = new EncryptedSocketWrapper(ApplicationValues.managment_tunnel);

                while (true)
                {
                    //Wait until the server sends a request.
                    string json_data = Encoding.UTF8.GetString(wrapper.recieve());
                    Dictionary<string, string>? request = JsonSerializer.Deserialize<Dictionary<string, string>>(json_data);

                    //Decode request.
                    switch (request!["URL"])
                    {
                        case "\\managment_tunnel\\message_ready":
                            Messaging.messageReady(this, request);
                            
                            if (current_recipient == request["sender"])
                            {
                                Invoke(() => information_label.Text = $"{current_recipient} is idle");
                                message_recieved = true;
                                Invoke(() => Send_Button.Enabled = true);
                            }
                                
                            break;

                        case "\\managemnt_tunnel\\message_sending":
                            if (current_recipient == request["sender"])
                            {
                                Invoke(() => Send_Button.Enabled = false);
                                Invoke(() => information_label.Text = $"{current_recipient} is sending a message");
                                
                                System.Timers.Timer timer = new System.Timers.Timer(3000);
                                timer.Elapsed += disableMessageSend;
                                timer.Enabled = true;

                                message_recieved = false;
                            }
                            
                            break;

                        case "\\management_tunnel\\friend_request":
                            Invoke(() => createNotification($"{request["requester"]} sent you a friend request"));
                            break;

                        default:
                            Functions.showError("URL was not found");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                //There has been an error with the management tunnel so restart.
                Functions.showError("There was an error with the tunnel worker. The application will now restart.");
                Functions.showError(e.ToString());
                Functions.fatalRestart();
            }
        }

        //Creates a sustained connection between the server and client which the server uses to send messages to the client.
        private void createManagementTunnel()
        {
            //Make a request to the server to create a sustained connection.
            (Dictionary<string, string>? response, Socket? socket) = API.createManagmentTunnel();

            if (response != null && response.ContainsKey("error"))
            {
                Functions.showError(response["error"]);
                Functions.fatalRestart();
                
                return;
            }

            //Set managment_tunnel variable so it can be used elsewhere.
            ApplicationValues.managment_tunnel = socket!;
        }

        private void clearNotification(object? sender, EventArgs args)
        {
            Button? sender_button = sender as Button;
            sender_button?.Parent.Dispose();
        }

        public void createNotification(string message)
        {
            Panel notification_panel = new Panel();
            notification_panel.Width = notificationFlowLayoutPanel.Width - 7;
            notification_panel.Height = 80;
            notification_panel.BackColor = Color.White;

            Label label = new Label();
            label.AutoEllipsis = true;
            label.Text = message;
            label.AutoSize = false;
            label.Width = notification_panel.Width;
            label.Height = 50;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Font = new Font(label.Font, FontStyle.Bold);

            notification_panel.Controls.Add(label);

            Button clear_button = new Button();
            clear_button.Text = "Clear";
            clear_button.Location = new Point(60, 50);
            clear_button.Click += clearNotification;
            notification_panel.Controls.Add(clear_button);

            notificationFlowLayoutPanel.Controls.Add(notification_panel);
            notificationFlowLayoutPanel.Controls.SetChildIndex(notification_panel, 0);
        }

        public void createMessageBox(string message, string username)
        {
            if(messageBox.Items.Count != 0)
            {
                string start_text = messageBox.Items[messageBox.Items.Count - 1].ToString().Substring(0, username.Length);
                if (start_text != username)
                {
                    messageBox.Items.Add($"");
                }
            }
            messageBox.Items.Add($"{username}: {message}");
        }

        private List<Dictionary<string, string>>? getOldMessages(string username)
        {
            Dictionary<string, string> request = new Dictionary<string, string>()
            {
                { "URL", "\\api\\message\\download_old_messages" },
                { "username", ApplicationValues.username },
                { "token", ApplicationValues.session_token },
                { "recipient", username },
            };

            (Dictionary<string, string>? response, bool fatal_error) = API.apiRequest(request);
            if (fatal_error) return null;
            if (response == null) return null;

            List<Dictionary<string, string>>? messages = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(response["messages"]);
            return messages;
        }

        private async void chatPanelClick(object? sender, EventArgs e)
        {
            Panel? panel = (Panel?)sender;
            string username = panel!.GetChildAtPoint(new Point(4, 5)).Text;

            current_recipient = username;
            Active_Chat_Username.Text = username;
            information_label.Text = "Loading Chat...";
            messageBox.Items.Clear();

            List<Dictionary<string, string>>? messages = await Task.Factory.StartNew(() => getOldMessages(username));

            if (messages != null && messages.Count > 0)
            {
                foreach (var message in messages)
                {
                    string encrypted_message = message["message_data"];
                    string decrypted_message = Encryption.decrypt(Convert.FromBase64String(encrypted_message), ApplicationValues.encryption_key);

                    createMessageBox(decrypted_message, message["sender"]);
                }
            }

            information_label.Text = $"{current_recipient} is idle";
        }

        private void usernameLabelClick(object? sender, EventArgs e)
        {
            Label label = (Label)sender!;
            chatPanelClick(label.Parent, e);
        }

        private async void sendButtonClick(object? sender, EventArgs e)
        {
            Send_Button.Enabled = false;
            string recipent = current_recipient;
            string message = Input_Box.Text;
            Input_Box.Clear();

            createMessageBox(message, ApplicationValues.username);
            
            await Task.Factory.StartNew(() => Messaging.sendMessage(message, recipent));
            Send_Button.Enabled = true;
        }

        private void addChat(string username)
        {
            Panel chat_panel = new Panel();
            chat_panel.Width = ChatsPanel.Width - 4;
            chat_panel.Parent = ChatsPanel;
            chat_panel.BackColor = Color.LightGray;
            chat_panel.Height = 112;
            chat_panel.Location = new Point(chat_panel.Location.X + 12, chat_panel.Location.Y);

            chat_panel.Click += chatPanelClick;

            Label name = new Label();
            Random random = new Random();
            name.Text = username;
            name.Parent = chat_panel;
            name.Location = new Point(4, 5);
            name.Width = 200;
            name.AutoEllipsis = true;

            name.Click += usernameLabelClick;

            RichTextBox last_message = new RichTextBox();
            last_message.Text = "Last Message";
            last_message.Parent = chat_panel;
            last_message.Width = 200;
            last_message.Height = 46;
            last_message.Location = new Point(5, 28);
            last_message.BorderStyle = BorderStyle.FixedSingle;
            last_message.Enabled = false;
            last_message.BackColor = chat_panel.BackColor;
            last_message.BorderStyle = BorderStyle.None;
            last_message.ScrollBars = RichTextBoxScrollBars.None;

            Button delete_chat = new Button();
            delete_chat.Text = "Delete Chat";
            delete_chat.Width = 204;
            delete_chat.Height = 25;
            delete_chat.Parent = chat_panel;
            delete_chat.BackColor = Color.WhiteSmoke;
            delete_chat.Location = new Point(4, 80);

            delete_chat.Click += deleteChatClick;
        }

        private void NewChatBtnClick(object sender, EventArgs e)
        {
            ChatSelector chatSelector = new ChatSelector();
            DialogResult result = chatSelector.ShowDialog();

            if (result == DialogResult.OK)
            {
                addChat(chatSelector.selected_username);
            }
        }

        private void deleteChatClick(object? sender, EventArgs e)
        {
            Button? button = (Button?)sender;
            Panel? panel = (Panel?)button?.Parent;
        }

        private void inputBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && Send_Button.Enabled)
            {
                sendButtonClick(sender, e);
                e.SuppressKeyPress = true;
            }
        }

        private void clearAllButton_Click(object sender, EventArgs e)
        {
            notificationFlowLayoutPanel.Controls.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}