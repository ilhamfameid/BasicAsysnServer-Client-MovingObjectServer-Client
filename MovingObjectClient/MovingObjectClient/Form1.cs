using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace MovingObjectClient {
    // Objek untuk menyimpan state saat menerima data secara async
    public class StateObject {
        public Socket workSocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }

    public partial class Form1 : Form {
        Rectangle rect = new Rectangle(20, 20, 30, 30);
        SolidBrush fillBlue = new SolidBrush(Color.Blue);
        Pen red = new Pen(Color.Red);

        Socket clientSocket;

        public Form1() {
            InitializeComponent();

            // Mengurangi flicker
            this.DoubleBuffered = true;

            // Coba terhubung ke server saat form dibuka
            ConnectToServer();
        }

        private void ConnectToServer() {
            try {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // Ganti "127.0.0.1" dengan IP Address server jika beda komputer
                clientSocket.BeginConnect("127.0.0.1", 11000, new AsyncCallback(ConnectCallback), null);
            } catch (Exception ex) {
                MessageBox.Show("Connection Error: " + ex.Message);
            }
        }

        private void ConnectCallback(IAsyncResult ar) {
            try {
                clientSocket.EndConnect(ar);
                Console.WriteLine("Connected to server: " + clientSocket.RemoteEndPoint);

                // Mulai menerima data dari server
                Receive();
            } catch (Exception ex) {
                MessageBox.Show("Callback Error: " + ex.Message);
            }
        }

        private void Receive() {
            try {
                StateObject state = new StateObject();
                state.workSocket = clientSocket;

                // Mulai proses menerima data secara asynchronous
                clientSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(OnReceive), state);
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }


        private void OnReceive(IAsyncResult ar) {
            try {
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;

                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0) {
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    ProcessReceivedData(state); // Proses data yang mungkin sudah lengkap
                }

                // Lanjutkan menerima data lagi
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(OnReceive), state);
            } catch (Exception) {
                // Jika error, kemungkinan server mati
                clientSocket.Close();
            }
        }

        private void ProcessReceivedData(StateObject state) {
            string content = state.sb.ToString();
            // Cari pesan lengkap yang diakhiri dengan '\n'
            int newlineIndex;
            while ((newlineIndex = content.IndexOf('\n')) > -1) {
                string message = content.Substring(0, newlineIndex);
                // Hapus pesan yang sudah diproses dari buffer StringBuilder
                content = content.Substring(newlineIndex + 1);

                // Parsing "x,y"
                string[] parts = message.Split(',');
                if (parts.Length == 2) {
                    try {
                        int x = int.Parse(parts[0]);
                        int y = int.Parse(parts[1]);

                        // Update posisi rect (cross-thread safe)
                        this.Invoke((MethodInvoker)delegate {
                            rect.X = x;
                            rect.Y = y;
                            Invalidate(); // gambar ulang form
                        });
                    } catch (FormatException) { /* Abaikan jika format salah */ }
                }
            }
            // Simpan sisa data yang belum lengkap untuk diproses di OnReceive berikutnya
            state.sb.Clear();
            state.sb.Append(content);
        }

        private void Form1_Paint(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;
            g.DrawRectangle(red, rect);
            g.FillRectangle(fillBlue, rect);
        }
    }
}
