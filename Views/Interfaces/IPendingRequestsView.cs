using HarborMaster.Models;
using System.Collections.Generic;

namespace HarborMaster.Views.Interfaces
{
    /// <summary>
    /// Interface for Pending Requests view (Operator/HarborMaster)
    /// Shows list of pending docking requests that need approval
    /// </summary>
    public interface IPendingRequestsView
    {
        /// <summary>
        /// Set the list of pending requests to display
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

        /// <summary>
        /// Refresh the requests list
        /// </summary>
        void RefreshData();
    }
}
