namespace Send_File_Client
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
            btnBrowse = new Button();
            btnSend = new Button();
            tbFilename = new TextBox();
            tbServer = new TextBox();
            openFileDialog = new OpenFileDialog();
            lbFile = new Label();
            lbServer = new Label();
            textBox1 = new TextBox();
            groupBox1 = new GroupBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(325, 55);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(94, 29);
            btnBrowse.TabIndex = 0;
            btnBrowse.Text = "Browse";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(328, 100);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(94, 29);
            btnSend.TabIndex = 1;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // tbFilename
            // 
            tbFilename.Location = new Point(76, 57);
            tbFilename.Name = "tbFilename";
            tbFilename.Size = new Size(243, 27);
            tbFilename.TabIndex = 2;
            // 
            // tbServer
            // 
            tbServer.Location = new Point(76, 100);
            tbServer.Name = "tbServer";
            tbServer.Size = new Size(243, 27);
            tbServer.TabIndex = 3;
            tbServer.Text = "127.0.0.1";
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "openFileDialog1";
            // 
            // lbFile
            // 
            lbFile.AutoSize = true;
            lbFile.Location = new Point(38, 60);
            lbFile.Name = "lbFile";
            lbFile.Size = new Size(32, 20);
            lbFile.TabIndex = 4;
            lbFile.Text = "File";
            lbFile.Click += label1_Click;
            // 
            // lbServer
            // 
            lbServer.AutoSize = true;
            lbServer.Location = new Point(20, 103);
            lbServer.Name = "lbServer";
            lbServer.Size = new Size(50, 20);
            lbServer.TabIndex = 5;
            lbServer.Text = "Server";
            // 
            // textBox1
            // 
            textBox1.BackColor = SystemColors.ScrollBar;
            textBox1.Location = new Point(0, 0);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(428, 27);
            textBox1.TabIndex = 6;
            textBox1.Text = "TCP Simple Test";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(textBox1);
            groupBox1.Controls.Add(btnSend);
            groupBox1.Controls.Add(tbServer);
            groupBox1.Controls.Add(lbServer);
            groupBox1.Controls.Add(lbFile);
            groupBox1.Controls.Add(tbFilename);
            groupBox1.Controls.Add(btnBrowse);
            groupBox1.Location = new Point(208, 130);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(428, 163);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
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

        private Button btnBrowse;
        private Button btnSend;
        private TextBox tbFilename;
        private TextBox tbServer;
        private OpenFileDialog openFileDialog;
        private Label lbFile;
        private Label lbServer;
        private TextBox textBox1;
        private GroupBox groupBox1;
    }
}
