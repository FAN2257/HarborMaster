using HarborMaster.Models;
using HarborMaster.Repositories; // <-- Menggunakan Repositori
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarborMaster.Services
{
    public class PortService
    {
        // Repositori yang dibutuhkan oleh layanan ini
        private readonly BerthRepository _berthRepository;
        private readonly BerthAssignmentRepository _assignmentRepository;
        private readonly ShipRepository _shipRepository; // Jika perlu

        public PortService()
        {
            _berthRepository = new BerthRepository();
            _assignmentRepository = new BerthAssignmentRepository();
            _shipRepository = new ShipRepository();
        }

        /// <summary>
        /// Logika bisnis utama untuk alokasi dermaga
        /// </summary>
        /// <returns>String pesan error, atau string kosong jika sukses.</returns>
        public async Task<string> TryAllocateBerth(int shipId, DateTime eta, DateTime etd, User user)
        {
            // 1. Validasi Waktu
            if (eta >= etd)
            {
                return "Waktu keberangkatan harus setelah waktu kedatangan.";
            }

            // 2. Ambil data kapal dari DB
            Ship ship = await _shipRepository.GetByIdAsync(shipId);
            if (ship == null)
            {
                return "Data kapal tidak ditemukan.";
            }

            // 3. Cari Dermaga yang Cocok (secara fisik) dari DB
            List<Berth> suitableBerths = await _berthRepository.GetPhysicallySuitableBerths(ship.LengthOverall, ship.Draft);

            if (!suitableBerths.Any())
            {
                return "Tidak ada dermaga yang cocok secara fisik (panjang/draft).";
            }

            // 4. Cari Slot Waktu yang Tersedia
            Berth availableBerth = null;
            foreach (var berth in suitableBerths)
            {
                // Cek konflik di database
                bool isCollision = await _assignmentRepository.HasScheduleCollision(berth.Id, eta, etd);

                if (!isCollision)
                {
                    availableBerth = berth;
                    break; // Ditemukan dermaga yang cocok dan kosong!
                }
            }

            // 5. Proses Alokasi
            if (availableBerth != null)
            {
                // Sukses! Buat entri jadwal baru
                BerthAssignment newAssignment = new BerthAssignment
                {
                    ShipId = ship.Id,
                    BerthId = availableBerth.Id,
                    ETA = eta,
                    ETD = etd,
                    Status = "Scheduled"
                };

                // Simpan ke database
                await _assignmentRepository.InsertAsync(newAssignment);
                return string.Empty; // Sukses
            }

            // 6. Gagal (Konflik)
            if (user.CanOverrideAllocation())
            {
                return "Semua dermaga yang cocok penuh. (Override Tersedia)";
            }

            return "Gagal: Semua dermaga yang cocok penuh pada jadwal tersebut.";
        }

        /// <summary>
        /// Mengambil semua jadwal yang sedang aktif dari database.
        /// </summary>
        public async Task<List<BerthAssignment>> GetActiveScheduleAsync()
        {
            return await _assignmentRepository.GetActiveSchedule();
        }

        /// <summary>
        /// Mengambil semua data dermaga dari database.
        /// </summary>
        public async Task<List<Berth>> GetAllBerthsAsync()
        {
            return await _berthRepository.GetAllAsync();
        }

        /// <summary>
        /// Process ship arrival registration - Create/get ship, allocate berth
        /// Returns: Success message with berth name, or error message
        /// </summary>
        public async Task<ShipArrivalResult> ProcessShipArrival(
            string shipName,
            string imoNumber,
            double length,
            double draft,
            string shipType,
            DateTime eta,
            DateTime etd,
            int? ownerId = null)
        {
            var result = new ShipArrivalResult();

            try
            {
                // 1. Validate time
                if (eta >= etd)
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "ETD harus setelah ETA.";
                    return result;
                }

                // 2. Check if ship already exists (by IMO or name)
                Ship? existingShip = null;

                if (!string.IsNullOrWhiteSpace(imoNumber))
                {
                    var allShips = await _shipRepository.GetAllAsync();
                    existingShip = allShips.FirstOrDefault(s =>
                        s.ImoNumber?.Equals(imoNumber, StringComparison.OrdinalIgnoreCase) == true);
                }

                Ship ship;
                if (existingShip != null)
                {
                    // Ship exists, use existing data
                    ship = existingShip;
                    result.IsNewShip = false;
                }
                else
                {
                    // Ship doesn't exist, create new one
                    ship = new Ship
                    {
                        Name = shipName,
                        ImoNumber = imoNumber,
                        LengthOverall = length,
                        Draft = draft,
                        ShipType = shipType,
                        OwnerId = ownerId // Set owner ID if provided (for Ship Owner role)
                    };

                    // Validate dimensions manually
                    try
                    {
                        ship.ValidateDimensions();
                    }
                    catch (ArgumentException ex)
                    {
                        // Validation error
                        result.IsSuccess = false;
                        result.ErrorMessage = $"Validasi gagal: {ex.Message}\n\nNilai yang diterima - Panjang: {length}m, Draft: {draft}m";
                        return result;
                    }

                    await _shipRepository.InsertAsync(ship);
                    result.IsNewShip = true;

                    // Fetch the inserted ship to get ID
                    var allShips = await _shipRepository.GetAllAsync();
                    ship = allShips.FirstOrDefault(s => s.Name == shipName) ?? ship;
                }

                // 3. Find suitable berth
                List<Berth> suitableBerths = await _berthRepository.GetPhysicallySuitableBerths(length, draft);

                if (!suitableBerths.Any())
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = $"Tidak ada dermaga yang cocok untuk kapal dengan panjang {length}m dan draft {draft}m.";
                    return result;
                }

                // 4. Validate docking duration using POLYMORPHISM
                int requestedDays = (int)Math.Ceiling((etd - eta).TotalDays);
                int maxAllowedDays = ship.GetMaxDockingDuration();

                if (requestedDays > maxAllowedDays)
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = $"Durasi berlabuh ({requestedDays} hari) melebihi maksimum untuk tipe kapal {ship.ShipType} ({maxAllowedDays} hari).";
                    return result;
                }

                // 5. Find available berth using POLYMORPHISM
                // ship.CanDockAt() checks physical constraints polymorphically
                Berth? allocatedBerth = null;
                foreach (var berth in suitableBerths)
                {
                    // Use POLYMORPHISM: ship.CanDockAt()
                    if (!ship.CanDockAt(berth))
                        continue;

                    bool hasCollision = await _assignmentRepository.HasScheduleCollision(berth.Id, eta, etd);

                    if (!hasCollision)
                    {
                        allocatedBerth = berth;
                        break;
                    }
                }

                if (allocatedBerth == null)
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "Semua dermaga yang cocok penuh pada jadwal tersebut.";
                    return result;
                }

                // 6. Create berth assignment with priority info
                var assignment = new BerthAssignment
                {
                    ShipId = ship.Id,
                    BerthId = allocatedBerth.Id,
                    ETA = eta,
                    ETD = etd,
                    Status = "Scheduled",
                    ActualArrivalTime = null, // Will be set when ship actually arrives
                    ActualDepartureTime = null // Will be set when ship departs
                };

                await _assignmentRepository.InsertAsync(assignment);

                // 7. Get required services using POLYMORPHISM
                var requiredServices = ship.GetRequiredServices();
                int priorityLevel = ship.GetPriorityLevel();

                // 8. Build success result
                result.IsSuccess = true;
                result.AllocatedBerth = allocatedBerth;
                result.Ship = ship;
                result.Assignment = assignment;
                result.RequiredServices = requiredServices;
                result.PriorityLevel = priorityLevel;
                result.SuccessMessage = $"Kapal '{shipName}' berhasil dialokasikan ke {allocatedBerth.BerthName}!\n" +
                                      $"Prioritas: {priorityLevel} | Layanan: {string.Join(", ", requiredServices)}";

                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = $"Error: {ex.Message}";
                return result;
            }
        }
    }

    /// <summary>
    /// Result object for ship arrival processing
    /// Enhanced with polymorphic data from Ship class
    /// </summary>
    public class ShipArrivalResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;
        public bool IsNewShip { get; set; } // True if ship was newly created
        public Ship? Ship { get; set; }
        public Berth? AllocatedBerth { get; set; }
        public BerthAssignment? Assignment { get; set; }

        // POLYMORPHISM: Data derived from virtual methods
        public List<string> RequiredServices { get; set; } = new List<string>();
        public int PriorityLevel { get; set; }
    }
}