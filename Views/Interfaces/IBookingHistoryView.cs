using HarborMaster.Models;
using System.Collections.Generic;

namespace HarborMaster.Views.Interfaces
{
    /// <summary>
    /// Interface for Booking History View
    /// Shows list of all docking requests made by the ship owner
    /// </summary>
    public interface IBookingHistoryView
    {
        /// <summary>
        /// Set the list of docking requests to display
        /// </summary>
        void SetBookingsDataSource(List<DockingRequest> bookings);

        /// <summary>
        /// Show message to user
        /// </summary>
        void ShowMessage(string message);

        /// <summary>
        /// Show error message
        /// </summary>
        void ShowError(string error);

        /// <summary>
        /// Set loading state
        /// </summary>
        bool IsLoading { get; set; }

        /// <summary>
        /// Get search term from search box
        /// </summary>
        string SearchTerm { get; }

        /// <summary>
        /// Get selected status filter
        /// </summary>
        string SelectedStatus { get; }

        /// <summary>
        /// Update result count display
        /// </summary>
        void UpdateResultCount(int visibleCount, int totalCount);

        /// <summary>
        /// Close the view
        /// </summary>
        void CloseView();
    }
}
