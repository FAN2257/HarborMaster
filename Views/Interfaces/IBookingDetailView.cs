using HarborMaster.Models;

namespace HarborMaster.Views.Interfaces
{
    /// <summary>
    /// Interface for Booking Detail View
    /// Shows detailed information about a docking request including invoice
    /// </summary>
    public interface IBookingDetailView
    {
        /// <summary>
        /// Set the booking details to display
        /// </summary>
        void SetBookingDetails(DockingRequest request, Ship ship, Berth? berth, BerthAssignment? assignment, Invoice? invoice);

        /// <summary>
        /// Show success message
        /// </summary>
        void ShowMessage(string message);

        /// <summary>
        /// Show error message
        /// </summary>
        void ShowError(string error);

        /// <summary>
        /// Set loading/exporting state
        /// </summary>
        bool IsExporting { get; set; }
    }
}
