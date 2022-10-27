namespace NEA_GUI
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.loading_icon = new System.Windows.Forms.PictureBox();
            this.sign_up_label = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.error_label = new System.Windows.Forms.Label();
            this.Submit = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Password_Input = new System.Windows.Forms.TextBox();
            this.Username_Input = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.loading_icon)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(1333, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "Chat App";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(-3, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(691, 71);
            this.panel1.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(541, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 37);
            this.label2.TabIndex = 1;
            this.label2.Text = "Chat App";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.loading_icon);
            this.panel2.Controls.Add(this.sign_up_label);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.error_label);
            this.panel2.Controls.Add(this.Submit);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.Password_Input);
            this.panel2.Controls.Add(this.Username_Input);
            this.panel2.Location = new System.Drawing.Point(12, 84);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(662, 391);
            this.panel2.TabIndex = 3;
            // 
            // loading_icon
            // 
            this.loading_icon.Image = ((System.Drawing.Image)(resources.GetObject("loading_icon.Image")));
            this.loading_icon.Location = new System.Drawing.Point(314, 296);
            this.loading_icon.Name = "loading_icon";
            this.loading_icon.Size = new System.Drawing.Size(32, 32);
            this.loading_icon.TabIndex = 15;
            this.loading_icon.TabStop = false;
            this.loading_icon.Visible = false;
            // 
            // sign_up_label
            // 
            this.sign_up_label.AutoSize = true;
            this.sign_up_label.Cursor = System.Windows.Forms.Cursors.Hand;
            this.sign_up_label.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point);
            this.sign_up_label.ForeColor = System.Drawing.Color.Blue;
            this.sign_up_label.Location = new System.Drawing.Point(276, 267);
            this.sign_up_label.Name = "sign_up_label";
            this.sign_up_label.Size = new System.Drawing.Size(48, 15);
            this.sign_up_label.TabIndex = 9;
            this.sign_up_label.Text = "Sign Up";
            this.sign_up_label.Click += new System.EventHandler(this.signUpLabelClick);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(142, 267);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(137, 15);
            this.label6.TabIndex = 8;
            this.label6.Text = "Don\'t Have An Account?";
            // 
            // error_label
            // 
            this.error_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.error_label.ForeColor = System.Drawing.Color.Red;
            this.error_label.Location = new System.Drawing.Point(15, 341);
            this.error_label.Name = "error_label";
            this.error_label.Size = new System.Drawing.Size(630, 39);
            this.error_label.TabIndex = 7;
            this.error_label.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Submit
            // 
            this.Submit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Submit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Submit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Submit.Location = new System.Drawing.Point(283, 200);
            this.Submit.Name = "Submit";
            this.Submit.Size = new System.Drawing.Size(99, 38);
            this.Submit.TabIndex = 5;
            this.Submit.Text = "Submit";
            this.Submit.UseVisualStyleBackColor = false;
            this.Submit.Click += new System.EventHandler(this.submitClick);
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(142, 138);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 25);
            this.label5.TabIndex = 4;
            this.label5.Text = "Password";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(136, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 25);
            this.label4.TabIndex = 3;
            this.label4.Text = "Username";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(283, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 50);
            this.label3.TabIndex = 2;
            this.label3.Text = "Login";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // Password_Input
            // 
            this.Password_Input.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Password_Input.Location = new System.Drawing.Point(246, 140);
            this.Password_Input.Name = "Password_Input";
            this.Password_Input.PasswordChar = '*';
            this.Password_Input.Size = new System.Drawing.Size(274, 23);
            this.Password_Input.TabIndex = 1;
            // 
            // Username_Input
            // 
            this.Username_Input.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Username_Input.Location = new System.Drawing.Point(246, 95);
            this.Username_Input.Name = "Username_Input";
            this.Username_Input.Size = new System.Drawing.Size(274, 23);
            this.Username_Input.TabIndex = 0;
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 487);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(450, 500);
            this.Name = "Login";
            this.Text = "Login";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.loading_icon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Label label1;
        private Panel panel1;
        private Label label2;
        private Panel panel2;
        private Button Submit;
        private Label label5;
        private Label label4;
        private Label label3;
        private TextBox Password_Input;
        private TextBox Username_Input;
        private Label error_label;
        private Label sign_up_label;
        private Label label6;
        private PictureBox loading_icon;
    }
}