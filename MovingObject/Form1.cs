using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace MovingObject {
    public partial class Form1 : Form {
        Pen red = new Pen(Color.Red);
        Rectangle rect = new Rectangle(20, 20, 30, 30);
        SolidBrush fillBlue = new SolidBrush(Color.Blue);
        int slideX = 5;
        int slideY = 5; // Menambahkan gerakan vertikal

        Socket serverSocket;
        // Gunakan object untuk lock agar thread-safe
        private readonly object clientLock = new object();
        List<Socket> clientSockets = new List<Socket>();

        public Form1() {
            InitializeComponent();

            // Mengurangi flicker
            this.DoubleBuffered = true;

            // setup timer
            timer1.Interval = 30; // Sedikit lebih cepat
            timer1.Enabled = true;

            // setup server socket
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 11000));
            serverSocket.Listen(10); // Bisa menampung hingga 10 antrian koneksi
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            Console.WriteLine("Server started, waiting for clients...");
        }

        private void AcceptCallback(IAsyncResult ar) {
            try {
                Socket client = serverSocket.EndAccept(ar);
                lock (clientLock) {
                    clientSockets.Add(client);
                }
                Console.WriteLine("Client connected: " + client.RemoteEndPoint);

                // Siap menerima client berikutnya
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Accept Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void timer1_Tick(object sender, EventArgs e) {
            MoveAndBounce();
            Invalidate(); // Memicu Form1_Paint

            // Setiap update posisi → kirim ke semua client
            // MENAMBAHKAN '\n' SEBAGAI PEMISAH PESAN
            string pos = rect.X + "," + rect.Y + "\n";
            byte[] data = Encoding.ASCII.GetBytes(pos);

            // Buat daftar client yang disconnect untuk dihapus nanti
            List<Socket> disconnectedClients = new List<Socket>();

            lock (clientLock) {
                foreach (Socket client in clientSockets) {
                    try {
                        client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), client);
                    } catch (SocketException) {
                        // Jika ada error saat mengirim, tandai client untuk dihapus
                        disconnectedClients.Add(client);
                    }
                }

                // Hapus semua client yang sudah ditandai
                foreach (Socket client in disconnectedClients) {
                    Console.WriteLine("Client disconnected: " + client.RemoteEndPoint);
                    clientSockets.Remove(client);
                }
            }
        }

        private void SendCallback(IAsyncResult ar) {
            try {
                Socket client = (Socket)ar.AsyncState;
                client.EndSend(ar);
            } catch (Exception) {
                // Biarkan, penanganan disconnect sudah ada di perulangan utama
            }
        }

        private void MoveAndBounce() {
            // Gerak horizontal
            if (rect.X <= 0 || (rect.X + rect.Width) >= this.ClientSize.Width) {
                slideX = -slideX;
            }
            // Gerak vertikal
            if (rect.Y <= 0 || (rect.Y + rect.Height) >= this.ClientSize.Height) {
                slideY = -slideY;
            }
            rect.X += slideX;
            rect.Y += slideY;
        }

        private void Form1_Paint(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;
            g.DrawRectangle(red, rect);
            g.FillRectangle(fillBlue, rect);
        }
    }
}
