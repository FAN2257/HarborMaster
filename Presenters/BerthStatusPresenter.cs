using HarborMaster.Models;
using HarborMaster.Repositories;
using HarborMaster.Views.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarborMaster.Presenters
{
    // View model untuk menggabungkan data
    public class BerthStatusViewModel
    {
        public int BerthId { get; set; }
        public string BerthName { get; set; }
        public string BerthStatus { get; set; }
        public int? AssignmentId { get; set; }
        public string ShipName { get; set; } = "-";
        public DateTime? ETA { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ActualArrivalTime { get; set; }
        public string AssignmentStatus { get; set; } = "Available";
    }


    /// <summary>
    /// Presenter for Berth Status view (Operator/HarborMaster)
    /// </summary>
    public class BerthStatusPresenter
    {
        private readonly IBerthStatusView _view;
        private readonly BerthRepository _berthRepo;
        private readonly BerthAssignmentRepository _assignmentRepo;
        private readonly ShipRepository _shipRepo;

        public BerthStatusPresenter(IBerthStatusView view)
        {
            _view = view;
            _berthRepo = new BerthRepository();
            _assignmentRepo = new BerthAssignmentRepository();
            _shipRepo = new ShipRepository();
        }

        /// <summary>
        /// Load all berths and their current assignment status
        /// </summary>
        public async Task LoadBerthsAsync()
        {
            try
            {
                _view.IsLoading = true;

                // 1. Ambil semua data yang relevan secara paralel
                var berthsTask = _berthRepo.GetAllAsync();
                var assignmentsTask = _assignmentRepo.GetAllAsync(); // Ambil semua untuk join
                var shipsTask = _shipRepo.GetAllAsync();

                await Task.WhenAll(berthsTask, assignmentsTask, shipsTask);

                var berths = berthsTask.Result;
                var assignments = assignmentsTask.Result;
                var ships = shipsTask.Result;

                var viewModelList = new List<BerthStatusViewModel>();

                // 2. Buat view model dari setiap dermaga
                foreach (var berth in berths)
                {
                    var vm = new BerthStatusViewModel
                    {
                        BerthId = berth.Id,
                        BerthName = berth.BerthName,
                        BerthStatus = berth.Status,
                    };

                    // Cari assignment yang sedang aktif di dermaga ini
                    var activeAssignment = assignments.FirstOrDefault(a =>
                        a.BerthId == berth.Id &&
                        (a.Status == "Scheduled" || a.Status == "Arrived" || a.Status == "Docked" || a.Status == "Berlabuh"));

                    if (activeAssignment != null)
                    {
                        var ship = ships.FirstOrDefault(s => s.Id == activeAssignment.ShipId);
                        vm.AssignmentId = activeAssignment.Id;
                        vm.ShipName = ship?.Name ?? "Unknown Ship";
                        vm.ETA = activeAssignment.ETA;
                        vm.ETD = activeAssignment.ETD;
                        vm.ActualArrivalTime = activeAssignment.ActualArrivalTime;
                        vm.AssignmentStatus = activeAssignment.Status;
                    }
                    
                    viewModelList.Add(vm);
                }

                // 3. Kirim data yang sudah digabung ke View
                _view.SetBerthsDataSource(viewModelList);

                if (viewModelList.Count == 0)
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
        
        public async Task MarkAsArrivedAsync(int assignmentId)
        {
            try
            {
                _view.IsLoading = true;
                var assignment = await _assignmentRepo.GetByIdAsync(assignmentId);
                if (assignment == null)
                {
                    _view.ShowError("Jadwal tidak ditemukan.");
                    return;
                }

                assignment.Status = "Arrived";
                assignment.ActualArrivalTime = DateTime.Now;
                await _assignmentRepo.UpdateAsync(assignment);

                var berth = await _berthRepo.GetByIdAsync(assignment.BerthId);
                if (berth != null)
                {
                    berth.Status = "Occupied";
                    await _berthRepo.UpdateAsync(berth);
                }

                _view.ShowMessage("Status kapal berhasil diubah menjadi 'Arrived'.");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Gagal mencatat kedatangan: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
                await LoadBerthsAsync(); // Muat ulang data
            }
        }

        public async Task MarkAsDepartedAsync(int assignmentId)
        {
            try
            {
                _view.IsLoading = true;
                var assignment = await _assignmentRepo.GetByIdAsync(assignmentId);
                if (assignment == null)
                {
                    _view.ShowError("Jadwal tidak ditemukan.");
                    return;
                }

                assignment.Status = "Departed";
                assignment.ActualDepartureTime = DateTime.Now;
                await _assignmentRepo.UpdateAsync(assignment);

                var berth = await _berthRepo.GetByIdAsync(assignment.BerthId);
                if (berth != null)
                {
                    berth.Status = "Available";
                    await _berthRepo.UpdateAsync(berth);
                }
                
                _view.ShowMessage("Status kapal berhasil diubah menjadi 'Departed'.");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Gagal mencatat keberangkatan: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
                await LoadBerthsAsync(); // Muat ulang data
            }
        }
    }
}
