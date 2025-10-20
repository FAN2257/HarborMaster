using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarborMaster
{
    public class Ship
    {
        public string ShipID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Flag { get; set; }

        // --- PENAMBAHAN PROPERTI UNTUK VALIDASI FISIK (WAJIB) ---
        public double Draft { get; set; } // Kedalaman (Dibutuhkan untuk cek Berth.MaxDraft)
        public double Size { get; set; }  // Panjang atau Tonase (Dibutuhkan untuk validasi ukuran)
        // --------------------------------------------------------

        // Waktu untuk perencanaan jadwal (Dibutuhkan untuk input di ArrivalWindow)
        public DateTime ScheduledETA { get; set; }
        public DateTime ScheduledETD { get; set; }

        // Waktu aktual kapal
        public DateTime ArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }

        public string Status { get; set; }

        public void Arrive()
        {
            Status = "Arrived";
            ArrivalTime = DateTime.Now;
        }

        public void Depart()
        {
            Status = "Departed";
            DepartureTime = DateTime.Now;
        }

        public void UpdateStatus(string newStatus)
        {
            Status = newStatus;
        }
    }
}