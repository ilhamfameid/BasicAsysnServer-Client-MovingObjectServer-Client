using System.Net;
using System.Net.Sockets;

namespace Receiver_File_Server {
    public partial class Form1 : Form {
        private List<Socket> listSockets;
        private static readonly object fileLock = new object();
        private TcpListener? tcpListener;
        private Thread? thdListener;

        public Form1() {
            InitializeComponent();
            listSockets = new List<Socket>();
        }

        private void Form1_Load(object sender, EventArgs e) {
            lbConnections.Items.Add("Server siap. Klik 'Start Server' untuk memulai.");
        }

        // Tombol Start sekarang hanya bertugas memulai thread, sangat cepat dan tidak akan freeze.
        private void btnStartServer_Click(object sender, EventArgs e) {
            // Langsung mulai thread baru untuk semua pekerjaan server
            thdListener = new Thread(new ThreadStart(StartServerListener));
            thdListener.IsBackground = true;
            thdListener.Start();

            // Tombol langsung dinonaktifkan untuk memberikan feedback instan
            btnStartServer.Enabled = false;
            btnStartServer.Text = "Starting...";
        }

        // Method ini berisi SEMUA logika untuk memulai dan menjalankan server.
        // Ini berjalan sepenuhnya di background.
        public void StartServerListener() {
            try {
                // 1. Mencari IP di background thread
                var ip = Dns.GetHostAddresses(Dns.GetHostName())
                               .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);

                // 2. Mengirim perintah ke UI thread untuk menampilkan IP
                this.Invoke((MethodInvoker)delegate {
                    if (ip != null)
                        txtIpAddress.Text = ip.ToString();
                    else
                        txtIpAddress.Text = "No IPv4 address found!";
                });

                // 3. Memulai TCP Listener
                tcpListener = new TcpListener(IPAddress.Any, 5000);
                tcpListener.Start();

                // 4. Mengirim perintah ke UI thread untuk update status
                this.Invoke((MethodInvoker)delegate {
                    lbConnections.Items.Clear();
                    lbConnections.Items.Add("Server started, listening on port 5000...");
                    btnStartServer.Text = "Server Running";
                });

                // 5. Loop untuk menerima koneksi klien
                while (true) {
                    Socket handlerSocket = tcpListener.AcceptSocket();
                    this.Invoke((MethodInvoker)delegate {
                        lbConnections.Items.Add($"Client {handlerSocket.RemoteEndPoint} connected.");
                    });

                    // Setiap klien ditangani oleh thread-nya sendiri
                    Thread clientHandlerThread = new Thread(new ParameterizedThreadStart(HandleClient));
                    clientHandlerThread.IsBackground = true;
                    clientHandlerThread.Start(handlerSocket);
                }
            } catch (SocketException) {
                // Terjadi jika server dihentikan paksa
                this.Invoke((MethodInvoker)delegate { lbConnections.Items.Add("Server stopped."); });
            } catch (Exception ex) {
                // Menampilkan error jika gagal memulai server
                MessageBox.Show("Gagal memulai server: " + ex.Message);
                this.Invoke((MethodInvoker)delegate {
                    btnStartServer.Enabled = true;
                    btnStartServer.Text = "Start Server";
                });
            }
        }

        // Method untuk menangani setiap klien yang terhubung (dulu bernama handlerThread)
        public void HandleClient(object? socketObject) {
            if (socketObject is not Socket handlerSocket) return;

            try {
                listSockets.Add(handlerSocket);
                using (var networkStream = new NetworkStream(handlerSocket)) {
                    // ... (logika menerima file tetap sama) ...
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    string directory = "C:\\Temp";
                    Directory.CreateDirectory(directory);
                    string filePath = Path.Combine(directory, $"upload_{Guid.NewGuid()}.tmp");

                    using (var fileStream = File.Create(filePath)) {
                        while ((bytesRead = networkStream.Read(buffer, 0, buffer.Length)) > 0) {
                            fileStream.Write(buffer, 0, bytesRead);
                        }
                    }

                    this.Invoke((MethodInvoker)delegate {
                        lbConnections.Items.Add($"File from {handlerSocket.RemoteEndPoint} received.");
                    });
                }
            } catch (Exception) { /* Handle client disconnects silently */ } finally {
                this.Invoke((MethodInvoker)delegate {
                    lbConnections.Items.Add($"Client {handlerSocket.RemoteEndPoint} disconnected.");
                });
                listSockets.Remove(handlerSocket);
                handlerSocket.Close();
            }
        }
    }
}