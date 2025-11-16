using HarborMaster.Models;
using System;
using System.Collections.Generic;

namespace HarborMaster.Views.Interfaces
{
    /// <summary>
    /// Interface for Submit Docking Request view (ShipOwner)
    /// Form to submit new docking request
    /// </summary>
    public interface ISubmitDockingRequestView
    {
        // Input from user
        int SelectedShipId { get; }
        DateTime RequestedETA { get; }
        DateTime RequestedETD { get; }
        string CargoType { get; }
        string SpecialRequirements { get; }

        // Output to UI
        void SetShipsDataSource(List<Ship> ships);
        void ShowMessage(string message);
        void ShowError(string error);
        bool IsLoading { get; set; }
        void CloseView();
    }
}
