using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatServer {
    class Program {
        static Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();
        static List<string> history = new List<string>();

        static async Task Main(string[] args) {
            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            Console.WriteLine("Server started on port 5000");

            while (true) {
                var client = await server.AcceptTcpClientAsync();
                _ = HandleClient(client);
            }
        }

        static async Task HandleClient(TcpClient client) {
            using var reader = new StreamReader(client.GetStream(), Encoding.UTF8);
            var writer = new StreamWriter(client.GetStream(), Encoding.UTF8) { AutoFlush = true };

            string username = "";
            try {
                // Username handshake
                username = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(username) || clients.ContainsKey(username)) {
                    await writer.WriteLineAsync("[SERVER] Username invalid or already taken.");
                    client.Close();
                    return;
                }

                clients[username] = client;
                Console.WriteLine($"{username} joined.");
                await Broadcast($"[SERVER] {username} joined the chat.");
                await SendUserList();

                while (true) {
                    string data = await reader.ReadLineAsync();
                    if (data == null) break;

                    if (data.StartsWith("/typing")) {
                        await Broadcast($"[TYPING {username}]", username);
                    } else if (data.StartsWith("/notyping")) {
                        await Broadcast($"[NOTYPING {username}]", username);
                    } else if (data.StartsWith("/leave")) {
                        break;
                    } else if (data.StartsWith("/w")) {
                        // Private message
                        string[] parts = data.Split(' ', 3);
                        if (parts.Length >= 3) {
                            string targetUser = parts[1];
                            string pm = $"[PM from {username}] {parts[2]}";
                            if (clients.ContainsKey(targetUser)) {
                                await SendToUser(targetUser, pm);
                                await SendToUser(username, $"[PM to {targetUser}] {parts[2]}");
                            } else {
                                await SendToUser(username, $"[SERVER] User {targetUser} not found.");
                            }
                        }
                    } else {
                        history.Add($"{username}: {data}");
                        await Broadcast($"{username}: {data}");
                    }
                }
            } catch {
                // Ignore errors
            } finally {
                if (!string.IsNullOrWhiteSpace(username) && clients.ContainsKey(username)) {
                    clients.Remove(username);
                    Console.WriteLine($"{username} left.");
                    await Broadcast($"[SERVER] {username} left the chat.");
                    await SendUserList();
                }
                client.Close();
            }
        }

        static async Task Broadcast(string message, string? excludeUser = null) {
            var disconnected = new List<string>();
            foreach (var kvp in clients) {
                if (kvp.Key == excludeUser) continue;
                try {
                    var writer = new StreamWriter(kvp.Value.GetStream(), Encoding.UTF8) { AutoFlush = true };
                    await writer.WriteLineAsync(message);
                } catch {
                    disconnected.Add(kvp.Key);
                }
            }
            foreach (var user in disconnected) clients.Remove(user);
        }

        static async Task SendToUser(string user, string message) {
            if (clients.TryGetValue(user, out var client)) {
                try {
                    var writer = new StreamWriter(client.GetStream(), Encoding.UTF8) { AutoFlush = true };
                    await writer.WriteLineAsync(message);
                } catch { }
            }
        }

        static async Task SendUserList() {
            string users = string.Join(",", clients.Keys);
            await Broadcast($"[USERLIST {users}]");
        }
    }
}
