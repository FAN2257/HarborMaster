using System;
using Postgrest.Models;
using Postgrest.Attributes;

namespace HarborMaster.Models
{
    /// <summary>
    /// DockingRequest model represents a ship owner's request to dock at the harbor.
    /// Workflow: Ship Owner creates request → Operator approves/rejects → BerthAssignment created if approved
    /// </summary>
    [Table("docking_requests")]
    public class DockingRequest : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        /// <summary>
        /// The ship requesting docking
        /// </summary>
        [Column("ship_id")]
        public int ShipId { get; set; }

        [Reference(typeof(Ship))]
        public Ship Ship { get; set; }

        /// <summary>
        /// The owner of the ship (user who submitted the request)
        /// </summary>
        [Column("owner_id")]
        public int OwnerId { get; set; }

        /// <summary>
        /// Requested Estimated Time of Arrival
        /// </summary>
        [Column("requested_eta")]
        public DateTime RequestedETA { get; set; }

        /// <summary>
        /// Requested Estimated Time of Departure
        /// </summary>
        [Column("requested_etd")]
        public DateTime RequestedETD { get; set; }

        /// <summary>
        /// Type of cargo (optional)
        /// Examples: "Containers", "Crude Oil", "Coal", "Passengers"
        /// </summary>
        [Column("cargo_type")]
        public string? CargoType { get; set; }

        /// <summary>
        /// Special requirements or notes from ship owner (optional)
        /// Examples: "Need crane service", "Hazmat handling required", "VIP passengers"
        /// </summary>
        [Column("special_requirements")]
        public string? SpecialRequirements { get; set; }

        /// <summary>
        /// Request status
        /// Values: "Pending", "Approved", "Rejected", "Cancelled"
        /// </summary>
        [Column("status")]
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// When the request was created
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// User ID of the operator/harbor master who processed the request (nullable)
        /// </summary>
        [Column("processed_by")]
        public int? ProcessedBy { get; set; }

        /// <summary>
        /// When the request was processed (approved/rejected)
        /// </summary>
        [Column("processed_at")]
        public DateTime? ProcessedAt { get; set; }

        /// <summary>
        /// Reason for rejection (only filled if status = "Rejected")
        /// </summary>
        [Column("rejection_reason")]
        public string? RejectionReason { get; set; }

        /// <summary>
        /// Link to the BerthAssignment created after approval (nullable)
        /// </summary>
        [Column("berth_assignment_id")]
        public int? BerthAssignmentId { get; set; }

        // ===== HELPER METHODS =====

        /// <summary>
        /// Calculate how many days requested for docking
        /// </summary>
        public int GetRequestedDays()
        {
            return (RequestedETD - RequestedETA).Days;
        }

        /// <summary>
        /// Check if request is still pending
        /// </summary>
        public bool IsPending()
        {
            return Status == "Pending";
        }

        /// <summary>
        /// Check if request was approved
        /// </summary>
        public bool IsApproved()
        {
            return Status == "Approved";
        }

        /// <summary>
        /// Check if request was rejected
        /// </summary>
        public bool IsRejected()
        {
            return Status == "Rejected";
        }

        /// <summary>
        /// Check if request was cancelled by owner
        /// </summary>
        public bool IsCancelled()
        {
            return Status == "Cancelled";
        }

        /// <summary>
        /// Get display-friendly status with color code
        /// </summary>
        public string GetStatusDisplay()
        {
            return Status switch
            {
                "Pending" => "⏳ Menunggu Persetujuan",
                "Approved" => "✅ Disetujui",
                "Rejected" => "❌ Ditolak",
                "Cancelled" => "🚫 Dibatalkan",
                _ => Status
            };
        }

        /// <summary>
        /// Get formatted date range display
        /// </summary>
        public string GetDateRangeDisplay()
        {
            return $"{RequestedETA:dd/MM/yyyy HH:mm} - {RequestedETD:dd/MM/yyyy HH:mm} ({GetRequestedDays()} hari)";
        }

        /// <summary>
        /// Validate request data before submission
        /// Returns error message (empty string if valid)
        /// </summary>
        public string Validate()
        {
            if (ShipId <= 0)
                return "Ship ID tidak valid";

            if (OwnerId <= 0)
                return "Owner ID tidak valid";

            if (RequestedETA >= RequestedETD)
                return "ETA harus lebih awal dari ETD";

            if (RequestedETA < DateTime.Now.AddHours(-1))
                return "ETA tidak boleh di masa lalu";

            var requestedDays = GetRequestedDays();
            if (requestedDays < 1)
                return "Durasi docking minimal 1 hari";

            if (requestedDays > 90)
                return "Durasi docking maksimal 90 hari";

            return string.Empty; // Valid
        }
    }
}
