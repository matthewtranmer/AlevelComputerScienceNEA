using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NEA_GUI
{
    public partial class ChatSelector : Form
    {
        public string selected_username = ""; 

        public ChatSelector()
        {
            InitializeComponent();
        }

        private void setUsername(string username)
        {
            selected_username = username;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Chat_panel_Click(object? sender, EventArgs e)
        {
            Panel? panel = (Panel?)sender;
            string? username = panel?.GetChildAtPoint(new Point(85, 10)).Text;

            if (username == null)
            {
                throw new Exception();
            }

            setUsername(username);
        }

        private void Username_label_Click(object? sender, EventArgs e)
        {
            Label? label = (Label?)sender;
            string? username = label?.Text;
            if (username == null)
            {
                throw new Exception();
            }

            setUsername(username);
        }

        private void add_recipient(string username)
        {
            Panel recipent_panel = new Panel();
            recipent_panel.Width = Recipients.Width - 8;
            recipent_panel.Parent = Recipients;
            recipent_panel.BackColor = Color.LightGray;
            recipent_panel.Height = 90;

            recipent_panel.Click += Chat_panel_Click;

            Label name = new Label();
            name.Text = username;
            name.Parent = recipent_panel;
            name.Location = new Point(85, 10);
            name.Width = 200;
            name.AutoEllipsis = true;

            name.Click += Username_label_Click;

            PictureBox profile_picture = new PictureBox();
            profile_picture.Width = profile_picture.Height = 75;
            profile_picture.Parent = recipent_panel;
            profile_picture.ImageLocation = "C:\\Users\\Matthew\\source\\repos\\NEA_GUI\\default_profile.jpg";
            profile_picture.Location = new Point(5, 5);


        }

        private void ChatSelector_Load(object sender, EventArgs e)
        {
            string[] friends = new string[] { "Matthew", "Mick", "Dave", "Jane Martin", "James Jones", "Marissa Jones" };

            for (int i = 0; i< friends.Length; i++)
            {
                add_recipient(friends[i]);
            }
        }
    }
}
