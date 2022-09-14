
namespace RobotecOPC_Client
{
    partial class Form1
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
            this.UsernametextBox = new System.Windows.Forms.TextBox();
            this.PasswordtextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.IPtextBox = new System.Windows.Forms.TextBox();
            this.PorttextBox = new System.Windows.Forms.TextBox();
            this.StatusConnect = new System.Windows.Forms.Label();
            this.NodeReadValueTextBox = new System.Windows.Forms.TextBox();
            this.NodeNameReadTextBox = new System.Windows.Forms.TextBox();
            this.ReadNodeButton = new System.Windows.Forms.Button();
            this.WriteNodeButton = new System.Windows.Forms.Button();
            this.NodeWriteValueTextBox = new System.Windows.Forms.TextBox();
            this.NodeNameWriteTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // UsernametextBox
            // 
            this.UsernametextBox.Location = new System.Drawing.Point(12, 35);
            this.UsernametextBox.Name = "UsernametextBox";
            this.UsernametextBox.Size = new System.Drawing.Size(100, 23);
            this.UsernametextBox.TabIndex = 0;
            this.UsernametextBox.Text = "Username";
            // 
            // PasswordtextBox
            // 
            this.PasswordtextBox.Location = new System.Drawing.Point(118, 35);
            this.PasswordtextBox.Name = "PasswordtextBox";
            this.PasswordtextBox.Size = new System.Drawing.Size(100, 23);
            this.PasswordtextBox.TabIndex = 1;
            this.PasswordtextBox.Text = "Password";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(225, 35);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Connect";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ConnectButton);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(225, 64);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Dissconect";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.DisconnectButton);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 107);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Read nodes";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 166);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Write nodes";
            // 
            // IPtextBox
            // 
            this.IPtextBox.Location = new System.Drawing.Point(13, 64);
            this.IPtextBox.Name = "IPtextBox";
            this.IPtextBox.Size = new System.Drawing.Size(100, 23);
            this.IPtextBox.TabIndex = 12;
            this.IPtextBox.Text = "IP";
            // 
            // PorttextBox
            // 
            this.PorttextBox.Location = new System.Drawing.Point(119, 64);
            this.PorttextBox.Name = "PorttextBox";
            this.PorttextBox.Size = new System.Drawing.Size(100, 23);
            this.PorttextBox.TabIndex = 13;
            this.PorttextBox.Text = "Port";
            // 
            // StatusConnect
            // 
            this.StatusConnect.AutoSize = true;
            this.StatusConnect.Location = new System.Drawing.Point(12, 9);
            this.StatusConnect.Name = "StatusConnect";
            this.StatusConnect.Size = new System.Drawing.Size(112, 15);
            this.StatusConnect.TabIndex = 17;
            this.StatusConnect.Text = "Connect status: null";
            // 
            // NodeReadValueTextBox
            // 
            this.NodeReadValueTextBox.Location = new System.Drawing.Point(119, 125);
            this.NodeReadValueTextBox.Name = "NodeReadValueTextBox";
            this.NodeReadValueTextBox.Size = new System.Drawing.Size(100, 23);
            this.NodeReadValueTextBox.TabIndex = 20;
            // 
            // NodeNameReadTextBox
            // 
            this.NodeNameReadTextBox.Location = new System.Drawing.Point(13, 125);
            this.NodeNameReadTextBox.Name = "NodeNameReadTextBox";
            this.NodeNameReadTextBox.Size = new System.Drawing.Size(100, 23);
            this.NodeNameReadTextBox.TabIndex = 19;
            // 
            // ReadNodeButton
            // 
            this.ReadNodeButton.Location = new System.Drawing.Point(225, 125);
            this.ReadNodeButton.Name = "ReadNodeButton";
            this.ReadNodeButton.Size = new System.Drawing.Size(75, 23);
            this.ReadNodeButton.TabIndex = 21;
            this.ReadNodeButton.Text = "Read";
            this.ReadNodeButton.UseVisualStyleBackColor = true;
            this.ReadNodeButton.Click += new System.EventHandler(this.ReadNodeButton_Click);
            // 
            // WriteNodeButton
            // 
            this.WriteNodeButton.Location = new System.Drawing.Point(225, 184);
            this.WriteNodeButton.Name = "WriteNodeButton";
            this.WriteNodeButton.Size = new System.Drawing.Size(75, 23);
            this.WriteNodeButton.TabIndex = 24;
            this.WriteNodeButton.Text = "Write";
            this.WriteNodeButton.UseVisualStyleBackColor = true;
            this.WriteNodeButton.Click += new System.EventHandler(this.WriteNodeButton_Click);
            // 
            // NodeWriteValueTextBox
            // 
            this.NodeWriteValueTextBox.Location = new System.Drawing.Point(119, 184);
            this.NodeWriteValueTextBox.Name = "NodeWriteValueTextBox";
            this.NodeWriteValueTextBox.Size = new System.Drawing.Size(100, 23);
            this.NodeWriteValueTextBox.TabIndex = 23;
            // 
            // NodeNameWriteTextBox
            // 
            this.NodeNameWriteTextBox.Location = new System.Drawing.Point(13, 184);
            this.NodeNameWriteTextBox.Name = "NodeNameWriteTextBox";
            this.NodeNameWriteTextBox.Size = new System.Drawing.Size(100, 23);
            this.NodeNameWriteTextBox.TabIndex = 22;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(313, 225);
            this.Controls.Add(this.WriteNodeButton);
            this.Controls.Add(this.NodeWriteValueTextBox);
            this.Controls.Add(this.NodeNameWriteTextBox);
            this.Controls.Add(this.ReadNodeButton);
            this.Controls.Add(this.NodeReadValueTextBox);
            this.Controls.Add(this.NodeNameReadTextBox);
            this.Controls.Add(this.StatusConnect);
            this.Controls.Add(this.PorttextBox);
            this.Controls.Add(this.IPtextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.PasswordtextBox);
            this.Controls.Add(this.UsernametextBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox UsernametextBox;
        private System.Windows.Forms.TextBox PasswordtextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox IPtextBox;
        private System.Windows.Forms.TextBox PorttextBox;
        private System.Windows.Forms.Label StatusConnect;
        private System.Windows.Forms.TextBox NodeReadValueTextBox;
        private System.Windows.Forms.TextBox NodeNameReadTextBox;
        private System.Windows.Forms.Button ReadNodeButton;
        private System.Windows.Forms.Button WriteNodeButton;
        private System.Windows.Forms.TextBox NodeWriteValueTextBox;
        private System.Windows.Forms.TextBox NodeNameWriteTextBox;
    }
}

