namespace NEA_GUI
{
    partial class ChatSelector
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.friendsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.requestsRecievedFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.usernameInput = new System.Windows.Forms.TextBox();
            this.sendRequestButton = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.requestsSentFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(211, 71);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(36, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 46);
            this.label1.TabIndex = 0;
            this.label1.Text = "Friends";
            // 
            // friendsFlowLayoutPanel
            // 
            this.friendsFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.friendsFlowLayoutPanel.AutoScroll = true;
            this.friendsFlowLayoutPanel.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.friendsFlowLayoutPanel.Location = new System.Drawing.Point(0, 69);
            this.friendsFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.friendsFlowLayoutPanel.Name = "friendsFlowLayoutPanel";
            this.friendsFlowLayoutPanel.Size = new System.Drawing.Size(211, 591);
            this.friendsFlowLayoutPanel.TabIndex = 1;
            // 
            // requestsRecievedFlowLayoutPanel
            // 
            this.requestsRecievedFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.requestsRecievedFlowLayoutPanel.AutoScroll = true;
            this.requestsRecievedFlowLayoutPanel.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.requestsRecievedFlowLayoutPanel.Location = new System.Drawing.Point(223, 69);
            this.requestsRecievedFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.requestsRecievedFlowLayoutPanel.Name = "requestsRecievedFlowLayoutPanel";
            this.requestsRecievedFlowLayoutPanel.Size = new System.Drawing.Size(211, 591);
            this.requestsRecievedFlowLayoutPanel.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(223, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(211, 71);
            this.panel2.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(51, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 62);
            this.label2.TabIndex = 1;
            this.label2.Text = "Requests\r\nRecieved";
            // 
            // usernameInput
            // 
            this.usernameInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.usernameInput.Location = new System.Drawing.Point(14, 677);
            this.usernameInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.usernameInput.Name = "usernameInput";
            this.usernameInput.PlaceholderText = "Username";
            this.usernameInput.Size = new System.Drawing.Size(492, 27);
            this.usernameInput.TabIndex = 3;
            // 
            // sendRequestButton
            // 
            this.sendRequestButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sendRequestButton.Location = new System.Drawing.Point(522, 665);
            this.sendRequestButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.sendRequestButton.Name = "sendRequestButton";
            this.sendRequestButton.Size = new System.Drawing.Size(111, 52);
            this.sendRequestButton.TabIndex = 4;
            this.sendRequestButton.Text = "Send Friend Request";
            this.sendRequestButton.UseVisualStyleBackColor = true;
            this.sendRequestButton.Click += new System.EventHandler(this.sendRequestButton_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel3.Controls.Add(this.label3);
            this.panel3.Location = new System.Drawing.Point(446, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(211, 71);
            this.panel3.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(46, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 62);
            this.label3.TabIndex = 1;
            this.label3.Text = "Requests \r\nSent";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // requestsSentFlowLayoutPanel
            // 
            this.requestsSentFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.requestsSentFlowLayoutPanel.AutoScroll = true;
            this.requestsSentFlowLayoutPanel.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.requestsSentFlowLayoutPanel.Location = new System.Drawing.Point(446, 69);
            this.requestsSentFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.requestsSentFlowLayoutPanel.Name = "requestsSentFlowLayoutPanel";
            this.requestsSentFlowLayoutPanel.Size = new System.Drawing.Size(211, 588);
            this.requestsSentFlowLayoutPanel.TabIndex = 3;
            // 
            // ChatSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(657, 723);
            this.Controls.Add(this.requestsSentFlowLayoutPanel);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.sendRequestButton);
            this.Controls.Add(this.usernameInput);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.requestsRecievedFlowLayoutPanel);
            this.Controls.Add(this.friendsFlowLayoutPanel);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ChatSelector";
            this.Text = "ChatSelector";
            this.Load += new System.EventHandler(this.ChatSelector_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Panel panel1;
        private FlowLayoutPanel friendsFlowLayoutPanel;
        private FlowLayoutPanel requestsRecievedFlowLayoutPanel;
        private Panel panel2;
        private Label label1;
        private Label label2;
        private TextBox usernameInput;
        private Button sendRequestButton;
        private Panel panel3;
        private Label label3;
        private FlowLayoutPanel requestsSentFlowLayoutPanel;
    }
}