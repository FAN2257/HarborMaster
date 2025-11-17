using HarborMaster.Models;

namespace HarborMaster.Views.Interfaces
{
    public interface IAddShipDialog
    {
        // Input properties - Ship Data
        string ShipName { get; }
        string ImoNumber { get; }
        decimal ShipLength { get; }
        decimal ShipDraft { get; }
        string ShipType { get; }
        int? OwnerId { get; } // Owner ID for Ship Owner role (nullable for backward compatibility)

        // Input properties - Arrival Schedule
        DateTime ETA { get; }
        int DurationDays { get; } // Durasi berlabuh dalam hari
        DateTime ETD { get; } // Auto-calculated dari ETA + duration

        // Output properties
        string ErrorMessage { set; }
        string SuccessMessage { set; }
        bool IsLoading { set; }

        // Methods
        void CloseDialog();
        void ShowDialogWindow();
        void HandleSuccess(string message);
    }
}
