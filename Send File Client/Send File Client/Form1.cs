using System.Net.Sockets;

namespace Send_File_Client {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        // Event handler untuk tombol "Browse..."
        // Fungsi: Membuka dialog untuk memilih file dan menampilkan path-nya di TextBox.
        private void btnBrowse_Click(object sender, EventArgs e) {
            // Pastikan Anda memiliki komponen 'openFileDialog' di form Anda
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                // Pastikan Anda memiliki TextBox bernama 'tbFilename' di form Anda
                tbFilename.Text = openFileDialog.FileName;
            }
        }

        // Event handler untuk tombol "Send"
        // Fungsi: Mengirim file yang dipilih ke server secara bertahap (streaming).
        private async void btnSend_Click(object sender, EventArgs e) {
            // Pastikan Anda memiliki TextBox 'tbFilename' dan 'tbServer' di form Anda
            if (string.IsNullOrWhiteSpace(tbFilename.Text) || string.IsNullOrWhiteSpace(tbServer.Text)) {
                MessageBox.Show("Silakan pilih file dan masukkan alamat IP server terlebih dahulu.", "Informasi Tidak Lengkap", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(tbFilename.Text)) {
                MessageBox.Show("File yang dipilih tidak ditemukan. Silakan periksa kembali.", "File Tidak Ada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Pastikan Anda memiliki tombol 'btnSend' di form Anda
            btnSend.Enabled = false; // Nonaktifkan tombol selama proses pengiriman

            try {
                // Menggunakan 'using' untuk memastikan koneksi ditutup secara otomatis
                using (var client = new TcpClient()) {
                    // Menghubungkan ke server dengan IP dari tbServer dan port 8080
                    await client.ConnectAsync(tbServer.Text, 5000);

                    // Mendapatkan stream jaringan untuk mengirim data
                    using (NetworkStream networkStream = client.GetStream())
                    // Membuka file yang akan dikirim
                    using (FileStream fileStream = File.OpenRead(tbFilename.Text)) {
                        // Membuat buffer kecil (misal: 8KB) untuk membaca file per bagian.
                        // Ini menjaga penggunaan RAM tetap rendah.
                        byte[] buffer = new byte[8192];
                        int bytesRead;

                        // Memulai loop untuk membaca file bagian per bagian
                        while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0) {
                            // Mengirim bagian file yang baru saja dibaca ke server
                            await networkStream.WriteAsync(buffer, 0, bytesRead);
                        }
                    } // FileStream dan NetworkStream akan ditutup di sini
                } // Koneksi TcpClient akan ditutup di sini

                MessageBox.Show("File berhasil dikirim!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (SocketException sockEx) {
                MessageBox.Show($"Tidak dapat terhubung ke server: {sockEx.Message}\nPastikan alamat IP server benar dan server sedang berjalan.", "Kesalahan Koneksi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (Exception ex) {
                MessageBox.Show($"Terjadi kesalahan: {ex.Message}", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } finally {
                // Apapun yang terjadi (sukses atau gagal), aktifkan kembali tombol 'Send'
                btnSend.Enabled = true;
            }
        }

        // Method kosong ini tidak diperlukan dan bisa dihapus dengan aman
        // menggunakan cara yang dijelaskan sebelumnya (via menu Events di Properties)
        private void label1_Click(object sender, EventArgs e) {
            // Tidak ada aksi yang diperlukan di sini
        }
    }
}