using HarborMaster.Models; // <-- Tambahkan ini
using HarborMaster.Views;
using System;
using System.Windows.Forms;

namespace HarborMaster
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowViewAsDialog();

            if (loginWindow.DialogResult == DialogResult.OK)
            {
                // Ambil user yang berhasil login
                User loggedInUser = loginWindow.LoggedInUser;

                // JALANKAN MAINWINDOW (INI BAGIAN BARUNYA):
                // Berikan user tersebut ke MainWindow
                Application.Run(new MainWindow(loggedInUser));
            }
            // Jika login ditutup, aplikasi akan berakhir.
        }
    }
}