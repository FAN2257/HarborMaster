using HarborMaster.Models;
using HarborMaster.Repositories;
using HarborMaster.Views.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborMaster.Presenters
{
    /// <summary>
    /// Presenter for My Ships view (ShipOwner)
    /// </summary>
    public class MyShipsPresenter
    {
        private readonly IMyShipsView _view;
        private readonly ShipRepository _shipRepo;
        private readonly User _currentUser;

        public MyShipsPresenter(IMyShipsView view, User currentUser)
        {
            _view = view;
            _currentUser = currentUser;
            _shipRepo = new ShipRepository();
        }

        /// <summary>
        /// Load ships owned by current user
        /// </summary>
        public async Task LoadMyShipsAsync()
        {
            try
            {
                _view.IsLoading = true;

                // Get all ships
                var allShips = await _shipRepo.GetAllAsync();

                // Filter ships owned by current user
                var myShips = allShips.FindAll(s => s.OwnerId == _currentUser.Id);

                _view.SetShipsDataSource(myShips);

                if (myShips.Count == 0)
                {
                    _view.ShowMessage("Anda belum memiliki kapal terdaftar.");
                }
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error loading ships: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }
    }
}
