using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ChatServer {
    public record ChatPacket(string type, string from, string? to, string? text, long ts);

    class Program {
        static readonly ConcurrentDictionary<TcpClient, string> clients = new();

        static async Task Main(string[] args) {
            int port = args.Length > 0 && int.TryParse(args[0], out var p) ? p : 9000;
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"Server started on port {port}. Waiting for clients...");

            while (true) {
                var tcp = await listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(tcp);
            }
        }

        static async Task HandleClientAsync(TcpClient tcp) {
            var ep = tcp.Client.RemoteEndPoint;
            Console.WriteLine($"Client connected: {ep}");
            var ns = tcp.GetStream();
            var buffer = new byte[8192];

            try {
                int read = await ns.ReadAsync(buffer, 0, buffer.Length);
                if (read <= 0) { tcp.Close(); return; }
                var initial = Encoding.UTF8.GetString(buffer, 0, read).Trim();
                var packet = JsonSerializer.Deserialize<ChatPacket>(initial);
                if (packet is null || packet.type != "join" || string.IsNullOrWhiteSpace(packet.from)) {
                    tcp.Close();
                    return;
                }

                clients[tcp] = packet.from;
                Console.WriteLine($"{packet.from} joined.");
                await BroadcastSystemAsync($"{packet.from} has joined the chat.");

                while (true) {
                    read = await ns.ReadAsync(buffer, 0, buffer.Length);
                    if (read <= 0) break;

                    var msg = Encoding.UTF8.GetString(buffer, 0, read).Trim();
                    ChatPacket? p;
                    try { p = JsonSerializer.Deserialize<ChatPacket>(msg); } catch { p = null; }
                    if (p is null) continue;

                    if (p.type == "msg") {
                        Console.WriteLine($"[{p.from}] {p.text}");
                        await BroadcastAsync(p);
                    } else if (p.type == "pm" && !string.IsNullOrEmpty(p.to)) {
                        Console.WriteLine($"[PM] {p.from} -> {p.to}: {p.text}");
                        await SendPrivateAsync(p);
                    } else if (p.type == "leave") {
                        break;
                    }
                }
            } catch { } finally {
                if (clients.TryRemove(tcp, out var user)) {
                    Console.WriteLine($"{user} left.");
                    await BroadcastSystemAsync($"{user} has left the chat.");
                }
                tcp.Close();
            }
        }

        static async Task BroadcastAsync(ChatPacket packet) {
            string json = JsonSerializer.Serialize(packet);
            byte[] data = Encoding.UTF8.GetBytes(json);

            foreach (var kvp in clients) {
                try { await kvp.Key.GetStream().WriteAsync(data, 0, data.Length); } catch { }
            }
        }

        static async Task BroadcastSystemAsync(string message) {
            var sysPacket = new ChatPacket("sys", "server", null, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            await BroadcastAsync(sysPacket);
        }

        static async Task SendPrivateAsync(ChatPacket packet) {
            string json = JsonSerializer.Serialize(packet);
            byte[] data = Encoding.UTF8.GetBytes(json);

            foreach (var kvp in clients) {
                if (kvp.Value == packet.to || kvp.Value == packet.from) // kirim ke penerima + pengirim
                {
                    try { await kvp.Key.GetStream().WriteAsync(data, 0, data.Length); } catch { }
                }
            }
        }
    }
}
