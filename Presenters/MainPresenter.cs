using HarborMaster.Models;
using HarborMaster.Services;
using HarborMaster.Views.Interfaces; // <-- Menggunakan interface
using System;
using System.Threading.Tasks;

namespace HarborMaster.Presenters
{
    public class MainPresenter
    {
        private readonly IMainView _view;
        private readonly PortService _portService;
        private readonly NotificationService _notificationService;

        // Simpan user yang sedang login
        private User _currentUser;

        public MainPresenter(IMainView view)
        {
            _view = view;
            _portService = new PortService();
            _notificationService = new NotificationService();
        }

        /// <summary>
        /// Dipanggil oleh View saat Form 'Main' pertama kali dimuat.
        /// </summary>
        public async Task LoadInitialDataAsync(User user)
        {
            _currentUser = user;

            // 1. Set info user di UI
            _view.SetCurrentUser(user.FullName, user.CanOverrideAllocation());

            // 2. Muat data jadwal dan dermaga
            await LoadScheduleAsync();
            await LoadBerthsAsync();
        }

        public async Task LoadScheduleAsync()
        {
            var schedule = await _portService.GetActiveScheduleAsync();
            _view.SetScheduleDataSource(schedule);
        }

        public async Task LoadBerthsAsync()
        {
            var berths = await _portService.GetAllBerthsAsync();
            _view.SetBerthDataSource(berths);
        }

        /// <summary>
        /// Dipanggil oleh View saat tombol 'Alokasi' diklik.
        /// </summary>
        public async Task AllocateBerthAsync()
        {
            try
            {
                // 1. Ambil input dari View
                int shipId = _view.SelectedShipId;
                DateTime eta = _view.SelectedETA;
                DateTime etd = _view.SelectedETD;

                // 2. Panggil service untuk alokasi
                string errorMessage = await _portService.TryAllocateBerth(shipId, eta, etd, _currentUser);

                if (string.IsNullOrEmpty(errorMessage))
                {
                    // 3. Sukses
                    _notificationService.ShowMessage("Alokasi Berhasil!");
                    await LoadScheduleAsync(); // Refresh data jadwal
                }
                else
                {
                    // 4. Gagal (konflik, dll)
                    _notificationService.ShowError(errorMessage);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Error: {ex.Message}");
            }
        }
    }
}