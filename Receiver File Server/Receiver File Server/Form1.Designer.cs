namespace Receiver_File_Server
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
        private void InitializeComponent() {
            txtIpAddress = new Label();
            groupBox1 = new GroupBox();
            lbConnections = new ListBox();
            btnStartServer = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // txtIpAddress
            // 
            txtIpAddress.AutoSize = true;
            txtIpAddress.Location = new Point(16, 41);
            txtIpAddress.Name = "txtIpAddress";
            txtIpAddress.Size = new Size(123, 20);
            txtIpAddress.TabIndex = 0;
            txtIpAddress.Text = "My IP Address is :";
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.ControlLight;
            groupBox1.Controls.Add(lbConnections);
            groupBox1.Controls.Add(btnStartServer);
            groupBox1.Controls.Add(txtIpAddress);
            groupBox1.Location = new Point(245, 68);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(332, 318);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            // 
            // lbConnections
            // 
            lbConnections.FormattingEnabled = true;
            lbConnections.Location = new Point(16, 109);
            lbConnections.Name = "lbConnections";
            lbConnections.Size = new Size(289, 184);
            lbConnections.TabIndex = 3;
            // 
            // btnStartServer
            // 
            btnStartServer.Location = new Point(16, 74);
            btnStartServer.Name = "btnStartServer";
            btnStartServer.Size = new Size(94, 29);
            btnStartServer.TabIndex = 2;
            btnStartServer.Text = "Start";
            btnStartServer.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(groupBox1);
            Name = "Form1";
            Text = "Form1";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label txtIpAddress;
        private GroupBox groupBox1;
        private Button btnStartServer;
        private ListBox lbConnections;
    }
}
