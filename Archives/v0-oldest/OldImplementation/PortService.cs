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

            // Berikan ID ke kapal baru jika belum ada
            if (string.IsNullOrEmpty(ship.ShipID))
            {
                ship.ShipID = (_nextShipId++).ToString();
            }

            // Loop untuk mencari dermaga yang cocok
            foreach (var berth in _berths)
            {
                // **A. Cek Kapasitas berdasarkan Tonnage (sebagai pengganti Draft):**
                // Anggap setiap dermaga punya batas tonnage berdasarkan MaxLength * 100
                decimal maxTonnage = (decimal)(berth.MaxLength * 100); 
                if (ship.Tonnage > maxTonnage)
                {
                    continue; // Dermaga ini terlalu kecil untuk kapal ini
                }

                // **B. Cek Konflik Waktu (Schedule Collision):**
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
                    AssignmentID = Guid.NewGuid().ToString(),
                    Ship = ship,
                    Berth = berth,
                    ETA = eta,
                    ETD = etd,
                    Status = "Scheduled",
                    HarborUser = user
                };
                _assignments.Add(newAssignment);
                return newAssignment; // Mengembalikan hasil alokasi
            }

            // Jika loop selesai tanpa menemukan dermaga yang bebas konflik
            throw new Exception("ALOKASI GAGAL: Tidak ditemukan dermaga yang sesuai atau semua jadwal bentrok.");
        }

        // Metode untuk tampilan Dashboard (MainForm akan memanggil ini)
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
                AssignmentID = Guid.NewGuid().ToString(),
                Ship = ship,
                Berth = berth,
                ETA = eta,
                ETD = etd,
                Status = "FORCED_SCHEDULED"
            };
            _assignments.Add(forcedAssignment);
            return forcedAssignment;
        }

        public void ReleaseBerth(Berth berth, Ship ship)
        {
            // Logika pelepasan dermaga:

            // 1. Perbarui Status Kapal
            ship.UpdateStatus("Departed");

            // 2. Perbarui Status Dermaga (jika Anda ingin mengelolanya secara langsung)
            berth.Status = "Available";

            // 3. Update assignment status
            var assignment = _assignments.FirstOrDefault(a => a.Ship.ShipID == ship.ShipID && a.Berth.Id == berth.Id);
            if (assignment != null)
            {
                assignment.Status = "Completed";
            }
        }

        // === TAMBAHAN: Methods dari HarborMasterController ===

        /// <summary>
        /// Create new berth assignment manually (from HarborMasterController)
        /// </summary>
        public BerthAssignment CreateAssignment(Ship ship, Berth berth, DateTime arrival, DateTime departure, HarborUser user)
        {
            var assignment = new BerthAssignment
            {
                AssignmentID = Guid.NewGuid().ToString(),
                Ship = ship,
                Berth = berth,
                ETA = arrival,
                ETD = departure,
                Status = "Scheduled",
                HarborUser = user
            };

            _assignments.Add(assignment);
            return assignment;
        }

        /// <summary>
        /// Monitor traffic for all ships (from HarborMasterController)
        /// </summary>
        public void MonitorTraffic(List<Ship> ships)
        {
            foreach (var ship in ships)
            {
                Console.WriteLine($"{ship.Name} is currently {ship.Status}");
            }
        }

        /// <summary>
        /// Generate harbor traffic report (from HarborMasterController)
        /// </summary>
        public string GenerateReport(List<BerthAssignment> assignments = null)
        {
            // Use current assignments if none provided
            assignments = assignments ?? _assignments.ToList();

            var report = new StringBuilder();
            report.AppendLine("=== Harbor Traffic Report ===");
            report.AppendLine($"Generated: {DateTime.Now:dd/MM/yyyy HH:mm}");
            report.AppendLine();

            if (!assignments.Any())
            {
                report.AppendLine("No assignments found.");
                return report.ToString();
            }

            foreach (var assignment in assignments)
            {
                report.AppendLine(assignment.GetAssignmentInfo());
            }

            report.AppendLine();
            report.AppendLine($"Total Assignments: {assignments.Count}");
            report.AppendLine($"Active: {assignments.Count(a => a.Status == "Scheduled")}");
            report.AppendLine($"Completed: {assignments.Count(a => a.Status == "Completed")}");

            return report.ToString();
        }

        /// <summary>
        /// Get all available berths
        /// </summary>
        public IEnumerable<Berth> GetAvailableBerths()
        {
            return _berths.Where(b => b.CheckAvailability());
        }

        /// <summary>
        /// Get berth by ID
        /// </summary>
        public Berth GetBerth(string berthId)
        {
            return _berths.FirstOrDefault(b => b.Id == berthId);
        }

        /// <summary>
        /// Get all berths
        /// </summary>
        public IEnumerable<Berth> GetAllBerths()
        {
            return _berths.AsReadOnly();
        }

        // Add backward compatibility method
        public decimal CalculateCost(Ship ship)
        {
            return ship.CalculateDockingFee();
        }
        
        public decimal CalculateCost()
        {
            return 0; // Default cost
        }

        public string GetServiceDescription()
        {
            return "Port Service";
        }

        public void RequestService(Ship ship)
        {
            // Default implementation
        }
    }
}