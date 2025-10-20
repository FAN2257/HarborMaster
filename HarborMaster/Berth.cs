using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarborMaster
{
    public class Berth
    {
        // 1. GANTI NAMA PROPERTI (Memperbaiki error CS0106: Berth does not contain a definition for Id)
        public string Id { get; set; } // Diubah dari BerthID -> Id

        public string Name { get; set; }

        // 2. PROPERTI FISIK (WAJIB untuk Cek Konflik/Draft)
        public double MaxDraft { get; set; } // Kedalaman maksimum yang bisa ditampung
        public double MaxLength { get; set; } // Panjang maksimum (Menggantikan Capacity string)

        // Properti yang disesuaikan
        public string Location { get; set; }
        public string Status { get; set; } // Tetap dipertahankan

        // Catatan Penting: Hapus logika AssignShip dan ReleaseShip. 
        // Logika kompleks waktu (BerthAssignment) dan konflik harus berada di PortService.

        public bool CheckAvailability()
        {
            // Fungsi ini masih bisa digunakan untuk cek status fisik
            return Status == "Available";
        }

        // Catatan: Semua metode Assign/Release/CreateAssignment harus berada di PortService.cs!
    }
}