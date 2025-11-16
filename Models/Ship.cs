using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarborMaster.Models;
using Postgrest.Models;
using Postgrest.Attributes;

namespace HarborMaster.Models
{
    /// <summary>
    /// Ship model with OOP principles applied:
    /// 1. ENCAPSULATION: Private fields + validation in setters
    /// 2. POLYMORPHISM: Virtual methods for type-specific behavior
    /// 3. INHERITANCE: Can be extended by derived classes
    /// Still maintains Supabase compatibility with [Table] and [Column] attributes
    /// </summary>
    [Table("ships")]
    public class Ship : BaseModel
    {
        // ===== ENCAPSULATION: Private backing fields =====
        private string _name = string.Empty;
        private string _imoNumber = string.Empty;
        private double _lengthOverall;
        private double _draft;
        private string _shipType = string.Empty;

        [PrimaryKey("id", false)]
        public int Id { get; set; }

        /// <summary>
        /// Owner of this ship (User ID)
        /// Foreign key to users table
        /// Nullable for backward compatibility with existing ships
        /// </summary>
        [Column("owner_id")]
        public int? OwnerId { get; set; }

        /// <summary>
        /// Ship name with validation (ENCAPSULATION)
        /// </summary>
        [Column("name")]
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Nama kapal tidak boleh kosong");
                if (value.Length > 100)
                    throw new ArgumentException("Nama kapal maksimal 100 karakter");
                _name = value.Trim();
            }
        }

        /// <summary>
        /// IMO Number with validation (ENCAPSULATION)
        /// </summary>
        [Column("imo_number")]
        public string ImoNumber
        {
            get => _imoNumber;
            set
            {
                // IMO optional, tapi kalau diisi harus valid
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var cleaned = value.Trim().ToUpper();
                    _imoNumber = cleaned;
                }
                else
                {
                    _imoNumber = value ?? string.Empty;
                }
            }
        }

        /// <summary>
        /// Ship length (ENCAPSULATION with validation moved to service layer)
        /// Validation is done in service/presenter to avoid issues with database deserialization
        /// </summary>
        [Column("length_overall")]
        public double LengthOverall
        {
            get => _lengthOverall;
            set => _lengthOverall = value;
        }

        /// <summary>
        /// Ship draft (ENCAPSULATION with validation moved to service layer)
        /// Validation is done in service/presenter to avoid issues with database deserialization
        /// </summary>
        [Column("draft")]
        public double Draft
        {
            get => _draft;
            set => _draft = value;
        }

        /// <summary>
        /// Validate ship dimensions (call this manually when needed)
        /// </summary>
        public void ValidateDimensions()
        {
            if (LengthOverall <= 0)
                throw new ArgumentException("Panjang kapal harus lebih dari 0");
            if (LengthOverall > 500)
                throw new ArgumentException("Panjang kapal tidak boleh lebih dari 500 meter");
            if (Draft <= 0)
                throw new ArgumentException("Draft kapal harus lebih dari 0");
            if (Draft > 30)
                throw new ArgumentException("Draft kapal tidak boleh lebih dari 30 meter");
        }

        /// <summary>
        /// Ship type with validation (ENCAPSULATION)
        /// </summary>
        [Column("ship_type")]
        public string ShipType
        {
            get => _shipType;
            set
            {
                var allowedTypes = new[] { "Container", "Tanker", "Bulk Carrier", "General Cargo", "Passenger" };
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tipe kapal tidak boleh kosong");

                // Case-insensitive check
                var matchedType = allowedTypes.FirstOrDefault(t =>
                    t.Equals(value, StringComparison.OrdinalIgnoreCase));

                if (matchedType == null)
                    throw new ArgumentException($"Tipe kapal harus salah satu dari: {string.Join(", ", allowedTypes)}");

                _shipType = matchedType; // Use the matched type with correct casing
            }
        }

        // ===== POLYMORPHISM: Virtual methods untuk behavior khusus per tipe =====

        /// <summary>
        /// Calculate special fee based on ship type (POLYMORPHISM)
        /// Override di derived class untuk behavior khusus
        /// </summary>
        public virtual decimal CalculateSpecialFee()
        {
            // Base implementation: fee tergantung tipe kapal
            return ShipType switch
            {
                "Container" => 500m,      // Extra handling untuk container
                "Tanker" => 1000m,         // Hazmat handling
                "Bulk Carrier" => 300m,    // Bulk loading equipment
                "General Cargo" => 200m,   // Standard cargo handling
                "Passenger" => 800m,       // Passenger terminal services
                _ => 0m
            };
        }

        /// <summary>
        /// Get priority level for berth allocation (POLYMORPHISM)
        /// 1 = Highest, 5 = Lowest
        /// </summary>
        public virtual int GetPriorityLevel()
        {
            return ShipType switch
            {
                "Tanker" => 1,          // Highest priority (safety)
                "Passenger" => 1,       // Highest priority (passenger welfare)
                "Container" => 2,       // High priority (economy)
                "Bulk Carrier" => 3,    // Medium
                "General Cargo" => 4,   // Lower
                _ => 3
            };
        }

        /// <summary>
        /// Get required services for this ship (POLYMORPHISM)
        /// </summary>
        public virtual List<string> GetRequiredServices()
        {
            var services = new List<string> { "Basic Docking", "Security" };

            switch (ShipType)
            {
                case "Container":
                    services.AddRange(new[] { "Crane Service", "Container Yard" });
                    break;
                case "Tanker":
                    services.AddRange(new[] { "Hazmat Team", "Fire Safety", "Spill Prevention" });
                    break;
                case "Bulk Carrier":
                    services.AddRange(new[] { "Conveyor Belt", "Bulk Loading Equipment" });
                    break;
                case "Passenger":
                    services.AddRange(new[] { "Passenger Terminal", "Customs", "Immigration" });
                    break;
                case "General Cargo":
                    services.Add("General Cargo Handling");
                    break;
            }

            return services;
        }

        /// <summary>
        /// Get maximum allowed docking duration (POLYMORPHISM)
        /// </summary>
        public virtual int GetMaxDockingDuration()
        {
            return ShipType switch
            {
                "Passenger" => 3,       // Short stay (cruise ships)
                "Tanker" => 14,         // Safety limitation
                "Container" => 21,      // Quick turnaround
                "Bulk Carrier" => 30,
                "General Cargo" => 30,
                _ => 30
            };
        }

        /// <summary>
        /// Validate if ship can dock at berth (POLYMORPHISM)
        /// </summary>
        public virtual bool CanDockAt(Berth berth)
        {
            if (berth == null) return false;

            bool lengthOk = this.LengthOverall <= berth.MaxLength;
            bool draftOk = this.Draft <= berth.MaxDraft;
            bool available = berth.IsAvailableForDocking();

            return lengthOk && draftOk && available;
        }

        /// <summary>
        /// Get ship display info (POLYMORPHISM)
        /// </summary>
        public virtual string GetDisplayInfo()
        {
            return $"{Name} ({ShipType}) - {LengthOverall}m x {Draft}m";
        }
    }
}
