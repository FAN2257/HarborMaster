using HarborMaster.Models;
using HarborMaster.Presenters;
using System.Collections.Generic;

namespace HarborMaster.Views.Interfaces
{
    /// <summary>
    /// Interface for Berth Status view (Operator/HarborMaster)
    /// Shows current status of all berths
    /// </summary>
    public interface IBerthStatusView
    {
        /// <summary>
        /// Set the list of berths to display
        /// </summary>
        void SetBerthsDataSource(List<BerthStatusViewModel> berths);

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
