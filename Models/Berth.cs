using System;
using Postgrest.Attributes;
using Postgrest.Models;

namespace HarborMaster.Models
{
    [Table("berths")]
    public class Berth : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }
        
        [Column("berth_name")]
        public string BerthName { get; set; }
        
        [Column("location")]
        public string Location { get; set; }
        
        [Column("max_length")]
        public double MaxLength { get; set; }
        
        [Column("max_draft")]
        public double MaxDraft { get; set; }

        /// <summary>
        /// Berth status: Available, Occupied, Maintenance, Damaged
        /// Replaces the old is_available boolean for more granular control
        /// </summary>
        [Column("status")]
        public string Status { get; set; } = "Available";

        [Column("base_rate_per_day")]
        public decimal BaseRatePerDay { get; set; }

        // ===== HELPER METHODS =====

        /// <summary>
        /// Check if berth is available for docking
        /// </summary>
        public bool IsAvailableForDocking()
        {
            return Status == "Available";
        }

        /// <summary>
        /// Check if berth is currently occupied
        /// </summary>
        public bool IsOccupied()
        {
            return Status == "Occupied";
        }

        /// <summary>
        /// Check if berth is under maintenance or damaged
        /// </summary>
        public bool IsUnderMaintenance()
        {
            return Status == "Maintenance" || Status == "Damaged";
        }

        /// <summary>
        /// Get display-friendly status with icon
        /// </summary>
        public string GetStatusDisplay()
        {
            return Status switch
            {
                "Available" => "✅ Tersedia",
                "Occupied" => "🚢 Terisi",
                "Maintenance" => "🔧 Maintenance",
                "Damaged" => "⚠️ Rusak",
                _ => Status
            };
        }
    }
}
