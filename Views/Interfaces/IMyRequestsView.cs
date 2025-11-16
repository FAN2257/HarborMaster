using HarborMaster.Models;
using System.Collections.Generic;

namespace HarborMaster.Views.Interfaces
{
    /// <summary>
    /// Interface for My Requests view (ShipOwner)
    /// Shows list of docking requests submitted by the logged-in user
    /// </summary>
    public interface IMyRequestsView
    {
        /// <summary>
        /// Set the list of requests to display
        /// </summary>
        void SetRequestsDataSource(List<DockingRequest> requests);

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
        /// Close the view
        /// </summary>
        void CloseView();
    }
}
