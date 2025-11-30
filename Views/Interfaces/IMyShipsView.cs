using HarborMaster.Models;
using System.Collections.Generic;

namespace HarborMaster.Views.Interfaces
{
    /// <summary>
    /// Interface for My Ships view (ShipOwner)
    /// Shows list of ships owned by the logged-in user
    /// </summary>
    public interface IMyShipsView
    {
        /// <summary>
        /// Set the list of ships to display
        /// </summary>
        void SetShipsDataSource(List<Ship> ships);

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
        /// Get selected ship type filter
        /// </summary>
        string SelectedShipType { get; }

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
