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
            this.requestsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.usernameInput = new System.Windows.Forms.TextBox();
            this.sendRequestButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(185, 53);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(40, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "Friends";
            // 
            // friendsFlowLayoutPanel
            // 
            this.friendsFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.friendsFlowLayoutPanel.AutoScroll = true;
            this.friendsFlowLayoutPanel.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.friendsFlowLayoutPanel.Location = new System.Drawing.Point(0, 52);
            this.friendsFlowLayoutPanel.Name = "friendsFlowLayoutPanel";
            this.friendsFlowLayoutPanel.Size = new System.Drawing.Size(185, 443);
            this.friendsFlowLayoutPanel.TabIndex = 1;
            // 
            // requestsFlowLayoutPanel
            // 
            this.requestsFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.requestsFlowLayoutPanel.AutoScroll = true;
            this.requestsFlowLayoutPanel.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.requestsFlowLayoutPanel.Location = new System.Drawing.Point(195, 52);
            this.requestsFlowLayoutPanel.Name = "requestsFlowLayoutPanel";
            this.requestsFlowLayoutPanel.Size = new System.Drawing.Size(185, 443);
            this.requestsFlowLayoutPanel.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(195, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(185, 53);
            this.panel2.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(29, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 37);
            this.label2.TabIndex = 1;
            this.label2.Text = "Requests";
            // 
            // usernameInput
            // 
            this.usernameInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.usernameInput.Location = new System.Drawing.Point(12, 508);
            this.usernameInput.Name = "usernameInput";
            this.usernameInput.PlaceholderText = "Username";
            this.usernameInput.Size = new System.Drawing.Size(233, 23);
            this.usernameInput.TabIndex = 3;
            // 
            // sendRequestButton
            // 
            this.sendRequestButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sendRequestButton.Location = new System.Drawing.Point(259, 499);
            this.sendRequestButton.Name = "sendRequestButton";
            this.sendRequestButton.Size = new System.Drawing.Size(97, 39);
            this.sendRequestButton.TabIndex = 4;
            this.sendRequestButton.Text = "Send Friend Request";
            this.sendRequestButton.UseVisualStyleBackColor = true;
            this.sendRequestButton.Click += new System.EventHandler(this.sendRequestButton_Click);
            // 
            // ChatSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(379, 542);
            this.Controls.Add(this.sendRequestButton);
            this.Controls.Add(this.usernameInput);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.requestsFlowLayoutPanel);
            this.Controls.Add(this.friendsFlowLayoutPanel);
            this.Controls.Add(this.panel1);
            this.MaximumSize = new System.Drawing.Size(395, 5000);
            this.Name = "ChatSelector";
            this.Text = "ChatSelector";
            this.Load += new System.EventHandler(this.ChatSelector_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Panel panel1;
        private FlowLayoutPanel friendsFlowLayoutPanel;
        private FlowLayoutPanel requestsFlowLayoutPanel;
        private Panel panel2;
        private Label label1;
        private Label label2;
        private TextBox usernameInput;
        private Button sendRequestButton;
    }
}