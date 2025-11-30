using HarborMaster.Models;

namespace HarborMaster.Views.Interfaces
{
    /// <summary>
    /// Interface for Ship Detail View
    /// Shows detailed information about a ship with PDF export capability
    /// </summary>
    public interface IShipDetailView
    {
        /// <summary>
        /// Set the ship to display
        /// </summary>
        void SetShipDetails(Ship ship);

        /// <summary>
        /// Show success message
        /// </summary>
        void ShowMessage(string message);

        /// <summary>
        /// Show error message
        /// </summary>
        void ShowError(string error);

        /// <summary>
        /// Set loading state
        /// </summary>
        bool IsExporting { get; set; }
    }
}
