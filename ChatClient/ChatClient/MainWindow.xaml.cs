using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace ChatClient {
    public partial class MainWindow : Window {
        TcpClient? client;
        NetworkStream? stream;

        public record ChatPacket(string type, string from, string? to, string? text, long ts);

        public MainWindow() {
            InitializeComponent();
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e) {
            try {
                client = new TcpClient();
                await client.ConnectAsync(txtHost.Text, int.Parse(txtPort.Text));
                stream = client.GetStream();

                // kirim join
                var join = new ChatPacket("join", txtUser.Text, null, null, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                await SendPacketAsync(join);

                AddMessage("Connected to server.");
                _ = Task.Run(ReceiveLoop);
            } catch (Exception ex) {
                AddMessage("Error: " + ex.Message);
            }
        }

        private async void btnDisconnect_Click(object sender, RoutedEventArgs e) {
            if (client != null && stream != null) {
                var leave = new ChatPacket("leave", txtUser.Text, null, null, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                await SendPacketAsync(leave);
            }
            client?.Close();
            AddMessage("Disconnected.");
        }

        private async void btnSend_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(txtMessage.Text)) return;
            string text = txtMessage.Text.Trim();
            txtMessage.Clear();

            ChatPacket packet;

            if (text.StartsWith("/w ")) {
                // format: /w user message
                var parts = text.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3) {
                    string toUser = parts[1];
                    string pmText = parts[2];
                    packet = new ChatPacket("pm", txtUser.Text, toUser, pmText, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                } else {
                    AddMessage("Usage: /w <user> <message>");
                    return;
                }
            } else {
                // broadcast biasa
                packet = new ChatPacket("msg", txtUser.Text, null, text, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            }

            await SendPacketAsync(packet);
        }

        private async Task SendPacketAsync(ChatPacket packet) {
            if (stream == null) return;
            string json = JsonSerializer.Serialize(packet);
            byte[] data = Encoding.UTF8.GetBytes(json);
            await stream.WriteAsync(data, 0, data.Length);
        }

        private async Task ReceiveLoop() {
            var buffer = new byte[8192];
            try {
                while (client != null && client.Connected) {
                    int read = await stream!.ReadAsync(buffer, 0, buffer.Length);
                    if (read <= 0) break;
                    var msg = Encoding.UTF8.GetString(buffer, 0, read).Trim();
                    ChatPacket? packet;
                    try { packet = JsonSerializer.Deserialize<ChatPacket>(msg); } catch { packet = null; }
                    if (packet != null) {
                        Dispatcher.Invoke(() => AddMessage($"[{packet.from}] {packet.text}"));
                    }
                }
            } catch (Exception ex) {
                Dispatcher.Invoke(() => AddMessage("Receive error: " + ex.Message));
            }
        }

        private void AddMessage(string msg) {
            lstChat.Items.Add($"{DateTime.Now:T} {msg}");
            lstChat.ScrollIntoView(lstChat.Items[lstChat.Items.Count - 1]);
        }
    }
}
