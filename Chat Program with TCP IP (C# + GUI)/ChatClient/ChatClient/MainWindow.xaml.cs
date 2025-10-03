using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ChatClient {
    public partial class MainWindow : Window {
        private TcpClient client;
        private StreamWriter writer;
        private StreamReader reader;
        private ObservableCollection<string> messages = new ObservableCollection<string>();
        private ObservableCollection<string> users = new ObservableCollection<string>();
        private bool isTyping = false;

        public MainWindow() {
            InitializeComponent();
            ChatList.ItemsSource = messages;
            UserList.ItemsSource = users;
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e) {
            try {
                string ip = IpInput.Text.Trim();
                int port = int.Parse(PortInput.Text.Trim());

                client = new TcpClient(ip, port);

                writer = new StreamWriter(client.GetStream(), Encoding.UTF8) { AutoFlush = true };
                reader = new StreamReader(client.GetStream(), Encoding.UTF8);

                await writer.WriteLineAsync(NameInput.Text.Trim());

                messages.Add("[System] Connected to server.");
                _ = Task.Run(ReceiveMessages);
            } catch (Exception ex) {
                messages.Add("[System] Error: " + ex.Message);
            }
        }

        private async Task ReceiveMessages() {
            try {
                while (true) {
                    string message = await reader.ReadLineAsync();
                    if (message == null) break;

                    Application.Current.Dispatcher.Invoke(() => {
                        if (message.StartsWith("[TYPING")) {
                            string typingUser = message.Split(' ')[1];
                            if (typingUser != NameInput.Text.Trim())
                                TypingIndicator.Text = $"{typingUser} is typing...";
                        } else if (message.StartsWith("[NOTYPING")) {
                            TypingIndicator.Text = "";
                        } else if (message.StartsWith("[USERLIST")) {
                            users.Clear();
                            string list = message.Substring(10, message.Length - 11);
                            foreach (var u in list.Split(',', StringSplitOptions.RemoveEmptyEntries))
                                users.Add(u);
                        } else {
                            messages.Add(message);
                        }
                    });
                }
            } catch {
                Application.Current.Dispatcher.Invoke(() =>
                    messages.Add("[System] Disconnected from server."));
            }
        }

        private async Task SendMessage() {
            if (writer != null && !string.IsNullOrWhiteSpace(MessageInput.Text)) {
                await writer.WriteLineAsync(MessageInput.Text.Trim());
                MessageInput.Text = "";
                await writer.WriteLineAsync($"/notyping {NameInput.Text}");
                isTyping = false;
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e) {
            await SendMessage();
        }

        private async void MessageInput_KeyDown(object sender, KeyEventArgs e) {
            if (!isTyping && writer != null) {
                await writer.WriteLineAsync($"/typing {NameInput.Text}");
                isTyping = true;
            }
            if (e.Key == Key.Enter) {
                await SendMessage();
            }
        }

        private async void MessageInput_GotFocus(object sender, RoutedEventArgs e) {
            if (writer != null && !isTyping) {
                await writer.WriteLineAsync($"/typing {NameInput.Text}");
                isTyping = true;
            }
        }

        private async void MessageInput_LostFocus(object sender, RoutedEventArgs e) {
            if (writer != null && isTyping) {
                await writer.WriteLineAsync($"/notyping {NameInput.Text}");
                isTyping = false;
            }
        }

        private async void DisconnectButton_Click(object sender, RoutedEventArgs e) {
            if (writer != null) {
                await writer.WriteLineAsync($"/leave {NameInput.Text}");
                client.Close();
                messages.Add("[System] Disconnected.");
            }
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e) {
            try {
                string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ChatHistory");
                Directory.CreateDirectory(folder);

                string fileName = $"history_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string filePath = Path.Combine(folder, fileName);

                File.WriteAllLines(filePath, messages);
                messages.Add($"[System] History saved to {filePath}");
            } catch (Exception ex) {
                messages.Add("[System] Error saving history: " + ex.Message);
            }
        }
    }
}
