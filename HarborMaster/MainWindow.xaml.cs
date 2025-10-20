using System.Windows;
using System;
using System.Linq;
using HarborMaster; // Namespace yang sama dengan Models dan Services

namespace HarborMaster
{
    public partial class MainWindow : Window
    {
        // Instansiasi Service Layer
        private PortService _portService = new PortService();

        public MainWindow()
        {
            InitializeComponent();
            // Muat data saat aplikasi dimulai
            RefreshSchedule();
        }

        // Event handler untuk tombol "Input Kapal Baru"
        private void OpenArrivalWindow_Click(object sender, RoutedEventArgs e)
        {
            // Pastikan ArrivalWindow ada di namespace yang sama
            ArrivalWindow arrivalWindow = new ArrivalWindow();

            // ShowDialog akan memblokir MainWindow sampai ArrivalWindow ditutup
            arrivalWindow.ShowDialog();

            // Refresh Data setelah input selesai
            RefreshSchedule();
        }

        // Event handler untuk tombol "Refresh Jadwal"
        private void RefreshSchedule_Click(object sender, RoutedEventArgs e)
        {
            RefreshSchedule();
        }

        // Metode inti untuk mengambil dan menampilkan data
        private void RefreshSchedule()
        {
            try
            {
                // Ambil daftar BerthAssignment aktif dari Service Layer
                var currentSchedule = _portService.GetCurrentAssignments().ToList();

                // Binding data langsung ke DataGrid
                DgvSchedule.ItemsSource = currentSchedule;
            }
            catch (Exception ex)
            {
                // Menampilkan error jika gagal terhubung ke data
                MessageBox.Show($"Gagal memuat jadwal: {ex.Message}", "Kesalahan Data", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}