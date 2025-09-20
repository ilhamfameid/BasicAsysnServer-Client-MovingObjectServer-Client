using System.Net;
using System.Net.Sockets;

namespace Receiver_File_Server {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            // Kode baru yang secara spesifik mencari alamat IPv4
            IPHostEntry IPHost = Dns.GetHostEntry(Dns.GetHostName());
            string myIP = "";
            foreach (IPAddress ip in IPHost.AddressList) {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                    myIP = ip.ToString();
                    break;
                }
            }
            lblStatus.Text = "My IP address is " + myIP;

            // Memulai thread untuk mendengarkan koneksi
            Thread thdListener = new Thread(new ThreadStart(listenerThread));
            thdListener.IsBackground = true; // Agar thread mati saat aplikasi ditutup
            thdListener.Start();
        }

        public void listenerThread() {
            try {
                // Gunakan IPAddress.Any untuk mendengarkan dari semua network interface
                TcpListener tcpListener = new TcpListener(IPAddress.Any, 8080);
                tcpListener.Start();

                while (true) {
                    Socket handlerSocket = tcpListener.AcceptSocket();
                    if (handlerSocket.Connected) {
                        // Update UI secara aman dari thread yang berbeda
                        this.Invoke((MethodInvoker)delegate {
                            lbConnections.Items.Add(handlerSocket.RemoteEndPoint.ToString() + " connected.");
                        });

                        // Membuat thread baru untuk menangani koneksi ini
                        // dan MELEWATKAN socket sebagai parameter untuk menghindari race condition.
                        Thread thdHandler = new Thread(new ParameterizedThreadStart(handlerThread));
                        thdHandler.IsBackground = true;
                        thdHandler.Start(handlerSocket);
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show("Listener Thread Error: " + ex.Message);
            }
        }

        // Metode ini sekarang menerima parameter socket secara langsung
        public void handlerThread(object clientSocket) {
            Socket handlerSocket = (Socket)clientSocket; // Cast parameter ke tipe Socket

            try {
                using (NetworkStream networkStream = new NetworkStream(handlerSocket)) {
                    int blockSize = 1024;
                    byte[] dataByte = new byte[blockSize];

                    string userDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string filePath = Path.Combine(userDocumentsPath, "upload.txt");

                    // Mengunci file agar tidak diakses oleh dua thread bersamaan
                    lock (this) {
                        using (FileStream fileStream = File.OpenWrite(filePath)) {
                            int bytesRead;
                            // Membaca data dari stream sampai koneksi ditutup (bytesRead == 0)
                            while ((bytesRead = networkStream.Read(dataByte, 0, blockSize)) > 0) {
                                fileStream.Write(dataByte, 0, bytesRead);
                            }
                        }
                    }

                    this.Invoke((MethodInvoker)delegate {
                        lbConnections.Items.Add("File Written to " + filePath);
                    });
                }
            } catch (Exception ex) {
                // Menampilkan error jika terjadi masalah saat menangani klien
                this.Invoke((MethodInvoker)delegate {
                    lbConnections.Items.Add("Error with " + handlerSocket.RemoteEndPoint.ToString() + ": " + ex.Message);
                });
            } finally {
                // Pastikan socket selalu ditutup
                handlerSocket.Close();
            }
        }

        private void label1_Click(object sender, EventArgs e) {
            // Event handler ini bisa dibiarkan kosong atau dihapus jika tidak digunakan
        }
    }
}