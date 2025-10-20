using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HarborMaster
{
    // KELAS INI BERFUNGSI SEBAGAI LOGIKA BISNIS, BUKAN MODEL DATA
    public class PortService
    {
        // 1. SIMULASI DATABASE: List statis untuk menyimpan jadwal dan data dermaga
        private static List<BerthAssignment> _assignments = new List<BerthAssignment>();
        private static List<Berth> _berths = new List<Berth>
        { 
            // Tambahkan Dermaga dengan properti fisik yang benar
            new Berth { Id = "A1", Name = "Dermaga Utara", MaxDraft = 12.0, MaxLength = 250 },
            new Berth { Id = "B2", Name = "Dermaga Selatan", MaxDraft = 15.0, MaxLength = 300 },
        };
        private static int _nextShipId = 1;

        // 2. METODE WAJIB (Memperbaiki error CS0106: AllocateBerth)
        public BerthAssignment AllocateBerth(Ship ship, DateTime eta, HarborUser user)
        {
            // Tentukan ETD (Estimated Time of Departure) untuk cek konflik
            // Contoh: Kapal akan sandar selama 48 jam (dapat disesuaikan)
            DateTime etd = eta.AddHours(48);

            // Berikan ID ke kapal baru
            ship.ShipID = (_nextShipId++).ToString();

            // Loop untuk mencari dermaga yang cocok
            foreach (var berth in _berths)
            {
                // **A. Cek Fisik (Draft):** Aturan anti-tabrakan/kerusakan dasar
                if (ship.Draft > berth.MaxDraft)
                {
                    continue; // Dermaga ini terlalu dangkal, coba dermaga berikutnya.
                }

                // **B. Cek Konflik Waktu (Sand Collision):**
                bool isConflict = _assignments
                    .Where(a => a.Berth.Id == berth.Id && a.Status == "Scheduled") // Hanya cek dermaga yang sama & status aktif
                    .Any(a =>
                        (eta < a.ETD && etd > a.ETA) // Logika tumpang tindih waktu
                    );

                if (isConflict)
                {
                    // Jika ada konflik, periksa role pengguna yang sedang login
                    if (user.CurrentRole == UserRoleType.HarborMaster)
                    {
                        // Melemparkan exception spesifik untuk di-catch dan di-override di UI
                        throw new Exception("CONFLICT_DETECTED_OVERRIDABLE");
                    }
                    // Jika bukan HarborMaster, lewati dermaga ini (coba yang lain)
                    continue;
                }

                // 3. Alokasi Sukses Ditemukan
                BerthAssignment newAssignment = new BerthAssignment
                {
                    Ship = ship,
                    Berth = berth,
                    ETA = eta,
                    ETD = etd,
                    Status = "Scheduled"
                };
                _assignments.Add(newAssignment);
                return newAssignment; // Mengembalikan hasil alokasi
            }

            // Jika loop selesai tanpa menemukan dermaga yang bebas konflik
            throw new Exception("ALOKASI GAGAL: Tidak ditemukan dermaga yang sesuai atau semua jadwal bentrok.");
        }

        // Metode untuk tampilan Dashboard (MainWindow.xaml.cs akan memanggil ini)
        public IEnumerable<BerthAssignment> GetCurrentAssignments()
        {
            return _assignments.Where(a => a.Status == "Scheduled").ToList();
        }

        // Metode tambahan yang mungkin diperlukan untuk fitur override
        public BerthAssignment ForceAllocateBerth(Ship ship, DateTime eta, Berth berth)
        {
            // Logika yang sama dengan AllocateBerth, tetapi mengabaikan pengecekan konflik
            DateTime etd = eta.AddHours(48);
            BerthAssignment forcedAssignment = new BerthAssignment
            {
                Ship = ship,
                Berth = berth,
                ETA = eta,
                ETD = etd,
                Status = "FORCED_SCHEDULED"
            };
            _assignments.Add(forcedAssignment);
            return forcedAssignment;
        }

        // File: PortService.cs

        public void ReleaseBerth(Berth berth, Ship ship)
        {
            // Logika pelepasan dermaga:

            // 1. Perbarui Status Kapal
            ship.UpdateStatus("Departed");

            // 2. Perbarui Status Dermaga (jika Anda ingin mengelolanya secara langsung)
            berth.Status = "Available";

            // Catatan: Anda juga bisa menghapus assignment ini dari List<BerthAssignment> _assignments
            // atau mengubah statusnya menjadi "Completed" di PortService.
        }
    }
}