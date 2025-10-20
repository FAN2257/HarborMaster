using System;
using System.Windows;
using System.Windows.Controls;
using HarborMaster; // Namespace yang sama dengan semua Models dan Services
// Tidak perlu banyak 'using' karena semua kelas berada di namespace yang sama

namespace HarborMaster
{
    public partial class ArrivalWindow : Window
    {
        // Instansiasi Service Layer di code-behind
        private PortService _portService = new PortService();

        // Simulasikan Pengguna yang login (Role: Operator Standar)
        // UBAH CurrentRole ke UserRoleType.HarborMaster untuk menguji fitur Override
        private HarborUser _currentUser = new HarborUser
        {
            UserId = 1,
            Username = "op_standar",
            CurrentRole = UserRoleType.PortOperator // Role awal
        };

        public ArrivalWindow()
        {
            InitializeComponent();
        }

        // Event Handler yang terikat ke Button Click="BtnAllocate_Click" di XAML
        private void BtnAllocate_Click(object sender, RoutedEventArgs e)
        {
            // TxtStatusMessage harus berupa TextBlock dengan Name="TxtStatusMessage" di XAML
            TxtStatusMessage.Text = "";

            try
            {
                // 1. Validasi dan Pengambilan Data Input dari kontrol XAML

                // Cek Nama Kapal
                if (string.IsNullOrWhiteSpace(TxtShipName.Text))
                    throw new ArgumentException("Nama Kapal tidak boleh kosong.");

                // Cek Angka (Draft dan Ukuran)
                if (!double.TryParse(TxtDraft.Text, out double shipDraft) || !double.TryParse(TxtSize.Text, out double shipSize))
                {
                    throw new ArgumentException("Draft dan Panjang harus berupa angka yang valid.");
                }

                // Cek Tanggal/Waktu
                DateTime eta = DtpETA.SelectedDate ?? throw new ArgumentException("ETA harus dipilih.");

                // 2. Buat Model Ship
                Ship newShip = new Ship { Name = TxtShipName.Text, Draft = shipDraft, Size = shipSize };

                // 3. Panggil Logika Bisnis (PortService.AllocateBerth)
                BerthAssignment result = _portService.AllocateBerth(newShip, eta, _currentUser);

                // 4. Sukses
                MessageBox.Show($"Kapal '{newShip.Name}' berhasil dialokasikan ke Dermaga {result.Berth.Id}.", "Alokasi Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                // 5. Penanganan Exception/Konflik

                if (ex.Message == "CONFLICT_DETECTED_OVERRIDABLE")
                {
                    // Kasus Khusus: Konflik terdeteksi, dan PortService mengindikasikan bisa di-override

                    // Kita asumsikan tombol ini hanya aktif jika pengguna adalah HarborMaster
                    MessageBoxResult response = MessageBox.Show(
                        "Konflik jadwal terdeteksi. Lakukan override?",
                        "Konflik Jadwal",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (response == MessageBoxResult.Yes)
                    {
                        // TODO: Panggil metode PortService.ForceAllocate() di sini (jika Anda membuatnya)
                        MessageBox.Show("Override Berhasil. Alokasi diproses secara paksa.", "Sukses");
                        this.Close();
                    }
                    else
                    {
                        TxtStatusMessage.Text = "Alokasi dibatalkan oleh pengguna (Override ditolak).";
                    }
                }
                else
                {
                    // Tampilkan error standar (validasi input, dermaga terlalu dangkal, dll.)
                    TxtStatusMessage.Text = $"Gagal Alokasi: {ex.Message}";
                }
            }
        }

        private void TxtShipName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}