using System.Net.Sockets;

namespace Send_File_Client {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e) {
            openFileDialog.ShowDialog();
            tbFilename.Text = openFileDialog.FileName;
        }

        private void btnSend_Click(object sender, EventArgs e) {
            try {
                // Buka file dan baca isinya ke dalam buffer
                using (Stream fileStream = File.OpenRead(tbFilename.Text)) {
                    byte[] fileBuffer = new byte[fileStream.Length];
                    fileStream.Read(fileBuffer, 0, (int)fileStream.Length);

                    // Buka koneksi TCP/IP dan kirim data
                    TcpClient clientSocket = new TcpClient(tbServer.Text, 8080);
                    using (NetworkStream networkStream = clientSocket.GetStream()) {
                        networkStream.Write(fileBuffer, 0, fileBuffer.Length);
                    }
                    clientSocket.Close();
                }
                MessageBox.Show("File sent successfully!");
            } catch (Exception ex) {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}