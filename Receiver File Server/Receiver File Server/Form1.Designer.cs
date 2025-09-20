using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
namespace Receiver_File_Server {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            lblStatus = new Label();
            lbConnections = new ListBox();
            groupBox1 = new GroupBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(28, 36);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(123, 20);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "My IP Address is :";
            lblStatus.Click += label1_Click;
            // 
            // lbConnections
            // 
            lbConnections.FormattingEnabled = true;
            lbConnections.Location = new Point(28, 60);
            lbConnections.Margin = new Padding(3, 4, 3, 4);
            lbConnections.Name = "lbConnections";
            lbConnections.Size = new Size(426, 224);
            lbConnections.TabIndex = 1;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lbConnections);
            groupBox1.Controls.Add(lblStatus);
            groupBox1.Location = new Point(235, 106);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(472, 303);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 562);
            Controls.Add(groupBox1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ListBox lbConnections;
        private GroupBox groupBox1;
    }
}

