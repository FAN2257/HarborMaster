using System;
using System.Collections.Generic;

namespace HarborMaster
{
    // --- PENAMBAHAN ENUM YANG HILANG (Dibutuhkan oleh PortService dan ArrivalWindow) ---
    public enum UserRoleType
    {
        HarborMaster = 1,      // Role dengan izin Override
        PortOperator = 2,      // Role standar untuk input data
        AdminStaff = 3         // Role untuk urusan administrasi/laporan
    }
    // ---------------------------------------------------------------------------------

    // Kelas ini berfungsi sebagai Model Pengguna (User) yang sedang aktif login
    public class HarborUser
    {
        // Properti yang dibutuhkan oleh ArrivalWindow.xaml.cs:
        public int UserId { get; set; }        // Dulu: HarborMasterID (Diubah ke int untuk konvensi)
        public string Username { get; set; }   // Dulu: Name (Diubah untuk kejelasan)
        public UserRoleType CurrentRole { get; set; } // Dulu: Role (Diubah ke Enum)

        // Properti lain dari definisimu:
        public string Contact { get; set; }

        // --- CATATAN: Metode di bawah ini SEBAIKNYA dipindahkan ke PortService.cs ---
        // Jika Anda tetap ingin menyimpannya di sini, hapus body-nya untuk menghilangkan ketergantungan pada Berth.cs
        /*
        public void CreateAssignment(Berth berth, Ship ship) { } 
        public void MonitorTraffic() { } 
        public void GenerateReport() { }
        */
        // -----------------------------------------------------------------------------
    }
}