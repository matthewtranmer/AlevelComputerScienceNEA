namespace NEA_GUI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.sortByDropdown = new System.Windows.Forms.ComboBox();
            this.NewChatBtn = new System.Windows.Forms.Button();
            this.messageBox = new System.Windows.Forms.ListBox();
            this.Send_Button = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Active_Chat_Username = new System.Windows.Forms.Label();
            this.ChatsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.left_panel = new System.Windows.Forms.Panel();
            this.Input_Box = new System.Windows.Forms.TextBox();
            this.notificationFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.clearAllButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.information_label = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.left_panel.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(214, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1002, 71);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(842, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "Chat App";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.DimGray;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.sortByDropdown);
            this.panel3.Controls.Add(this.NewChatBtn);
            this.panel3.Location = new System.Drawing.Point(2, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(220, 68);
            this.panel3.TabIndex = 0;
            // 
            // sortByDropdown
            // 
            this.sortByDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sortByDropdown.FormattingEnabled = true;
            this.sortByDropdown.Location = new System.Drawing.Point(11, 40);
            this.sortByDropdown.Name = "sortByDropdown";
            this.sortByDropdown.Size = new System.Drawing.Size(195, 23);
            this.sortByDropdown.TabIndex = 2;
            // 
            // NewChatBtn
            // 
            this.NewChatBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.NewChatBtn.Location = new System.Drawing.Point(11, 5);
            this.NewChatBtn.Name = "NewChatBtn";
            this.NewChatBtn.Size = new System.Drawing.Size(195, 29);
            this.NewChatBtn.TabIndex = 1;
            this.NewChatBtn.Text = "Create New Chat";
            this.NewChatBtn.UseVisualStyleBackColor = true;
            this.NewChatBtn.Click += new System.EventHandler(this.NewChatBtnClick);
            // 
            // messageBox
            // 
            this.messageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageBox.FormattingEnabled = true;
            this.messageBox.HorizontalScrollbar = true;
            this.messageBox.ItemHeight = 15;
            this.messageBox.Location = new System.Drawing.Point(226, 136);
            this.messageBox.Name = "messageBox";
            this.messageBox.Size = new System.Drawing.Size(781, 454);
            this.messageBox.TabIndex = 0;
            // 
            // Send_Button
            // 
            this.Send_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Send_Button.Enabled = false;
            this.Send_Button.Location = new System.Drawing.Point(887, 617);
            this.Send_Button.Name = "Send_Button";
            this.Send_Button.Size = new System.Drawing.Size(116, 56);
            this.Send_Button.TabIndex = 4;
            this.Send_Button.Text = "Send";
            this.Send_Button.UseVisualStyleBackColor = true;
            this.Send_Button.Click += new System.EventHandler(this.sendButtonClick);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.LightGray;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.Active_Chat_Username);
            this.panel2.Location = new System.Drawing.Point(226, 83);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(781, 47);
            this.panel2.TabIndex = 5;
            // 
            // Active_Chat_Username
            // 
            this.Active_Chat_Username.AutoSize = true;
            this.Active_Chat_Username.Location = new System.Drawing.Point(3, 16);
            this.Active_Chat_Username.Name = "Active_Chat_Username";
            this.Active_Chat_Username.Size = new System.Drawing.Size(0, 15);
            this.Active_Chat_Username.TabIndex = 0;
            // 
            // ChatsPanel
            // 
            this.ChatsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ChatsPanel.BackColor = System.Drawing.Color.DimGray;
            this.ChatsPanel.Location = new System.Drawing.Point(0, 71);
            this.ChatsPanel.Name = "ChatsPanel";
            this.ChatsPanel.Size = new System.Drawing.Size(222, 608);
            this.ChatsPanel.TabIndex = 1;
            // 
            // left_panel
            // 
            this.left_panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.left_panel.BackColor = System.Drawing.Color.Black;
            this.left_panel.Controls.Add(this.panel3);
            this.left_panel.Controls.Add(this.ChatsPanel);
            this.left_panel.Location = new System.Drawing.Point(-2, 0);
            this.left_panel.Name = "left_panel";
            this.left_panel.Size = new System.Drawing.Size(222, 679);
            this.left_panel.TabIndex = 6;
            // 
            // Input_Box
            // 
            this.Input_Box.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Input_Box.Enabled = false;
            this.Input_Box.Location = new System.Drawing.Point(226, 617);
            this.Input_Box.Multiline = true;
            this.Input_Box.Name = "Input_Box";
            this.Input_Box.Size = new System.Drawing.Size(655, 56);
            this.Input_Box.TabIndex = 7;
            this.Input_Box.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputBoxKeyDown);
            // 
            // notificationFlowLayoutPanel
            // 
            this.notificationFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.notificationFlowLayoutPanel.AutoScroll = true;
            this.notificationFlowLayoutPanel.BackColor = System.Drawing.Color.DimGray;
            this.notificationFlowLayoutPanel.Location = new System.Drawing.Point(1013, 130);
            this.notificationFlowLayoutPanel.Name = "notificationFlowLayoutPanel";
            this.notificationFlowLayoutPanel.Size = new System.Drawing.Size(203, 549);
            this.notificationFlowLayoutPanel.TabIndex = 8;
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.BackColor = System.Drawing.Color.DimGray;
            this.panel4.Controls.Add(this.clearAllButton);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Location = new System.Drawing.Point(1013, 71);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(203, 59);
            this.panel4.TabIndex = 9;
            // 
            // clearAllButton
            // 
            this.clearAllButton.Location = new System.Drawing.Point(63, 29);
            this.clearAllButton.Name = "clearAllButton";
            this.clearAllButton.Size = new System.Drawing.Size(77, 22);
            this.clearAllButton.TabIndex = 1;
            this.clearAllButton.Text = "Clear All";
            this.clearAllButton.UseVisualStyleBackColor = true;
            this.clearAllButton.Click += new System.EventHandler(this.clearAllButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(43, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 25);
            this.label2.TabIndex = 0;
            this.label2.Text = "Notifications";
            // 
            // information_label
            // 
            this.information_label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.information_label.Location = new System.Drawing.Point(230, 596);
            this.information_label.Name = "information_label";
            this.information_label.Size = new System.Drawing.Size(777, 18);
            this.information_label.TabIndex = 10;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(1216, 678);
            this.Controls.Add(this.information_label);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.notificationFlowLayoutPanel);
            this.Controls.Add(this.messageBox);
            this.Controls.Add(this.Input_Box);
            this.Controls.Add(this.left_panel);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.Send_Button);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(600, 598);
            this.Name = "MainForm";
            this.Text = "Chat App";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.left_panel.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Panel panel1;
        private Label label1;
        private Button NewChatBtn;
        private Button Send_Button;
        private Panel panel2;
        private Label Active_Chat_Username;
        private Panel panel3;
        private FlowLayoutPanel ChatsPanel;
        private Panel left_panel;
        private TextBox Input_Box;
        private ListBox messageBox;
        private FlowLayoutPanel notificationFlowLayoutPanel;
        private Panel panel4;
        private Label label2;
        private ComboBox sortByDropdown;
        private Label information_label;
        private Button clearAllButton;
    }
}