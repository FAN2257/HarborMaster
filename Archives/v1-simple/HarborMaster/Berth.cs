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
        public string BerthID 
        { 
        get => Id; 
       set => Id = value; 
     } // For backward compatibility

        public string Name { get; set; }

        // 2. PROPERTI FISIK (WAJIB untuk Cek Konflik/Draft)
        public double MaxDraft { get; set; } // Kedalaman maksimum yang bisa ditampung
        public double MaxLength { get; set; } // Panjang maksimum (Menggantikan Capacity string)
        public decimal Capacity { get; set; } // For backward compatibility

    // Properti yang disesuaikan
    public string Location { get; set; }
        public string Status { get; set; } = "Available"; // Tetap dipertahankan

      public bool CheckAvailability()
     {
     // Fungsi ini masih bisa digunakan untuk cek status fisik
            return Status == "Available";
        }

        // Method required by demo code
        public void AssignShip(Ship ship)
        {
            if (Status != "Available")
            {
throw new InvalidOperationException($"Berth {Id} is not available");
          }
        
        // Basic capacity check using tonnage
            if (MaxLength > 0 && ship.Tonnage > (decimal)(MaxLength * 100))
   {
       throw new InvalidOperationException($"Ship too large for berth {Id}");
      }
      
            Status = "Occupied";
        }
        
 public void ReleaseShip()
        {
     Status = "Available";
        }

        // Catatan: Semua metode Assign/Release/CreateAssignment yang kompleks harus berada di PortService.cs!
    }
}