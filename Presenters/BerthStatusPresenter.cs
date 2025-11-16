using HarborMaster.Models;
using HarborMaster.Repositories;
using HarborMaster.Views.Interfaces;
using System;
using System.Threading.Tasks;

namespace HarborMaster.Presenters
{
    /// <summary>
    /// Presenter for Berth Status view (Operator/HarborMaster)
    /// </summary>
    public class BerthStatusPresenter
    {
        private readonly IBerthStatusView _view;
        private readonly BerthRepository _berthRepo;

        public BerthStatusPresenter(IBerthStatusView view)
        {
            _view = view;
            _berthRepo = new BerthRepository();
        }

        /// <summary>
        /// Load all berths and their current status
        /// </summary>
        public async Task LoadBerthsAsync()
        {
            try
            {
                _view.IsLoading = true;

                var berths = await _berthRepo.GetAllAsync();

                _view.SetBerthsDataSource(berths);

                if (berths.Count == 0)
                {
                    _view.ShowMessage("Tidak ada data dermaga.");
                }
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error loading berths: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }
    }
}
