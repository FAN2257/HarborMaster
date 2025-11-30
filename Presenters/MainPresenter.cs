using HarborMaster.Models;
﻿using HarborMaster.Repositories;
﻿using HarborMaster.Services;
﻿using HarborMaster.Views.Interfaces;
﻿using System;
﻿using System.Collections.Generic;
﻿using System.Linq;
﻿using System.Threading.Tasks;
﻿
﻿namespace HarborMaster.Presenters
﻿{
﻿    public class MainPresenter
﻿    {
﻿        private readonly IMainView _view;
﻿        private readonly PortService _portService;
﻿        private readonly NotificationService _notificationService;
﻿        private readonly BerthAssignmentRepository _assignmentRepo;
﻿        private readonly ShipRepository _shipRepo;
﻿        private readonly BerthRepository _berthRepo;
﻿
﻿        private User _currentUser;
﻿
﻿        public MainPresenter(IMainView view)
﻿        {
﻿            _view = view;
﻿            _portService = new PortService();
﻿            _notificationService = new NotificationService();
﻿            _assignmentRepo = new BerthAssignmentRepository();
﻿            _shipRepo = new ShipRepository();
﻿            _berthRepo = new BerthRepository();
﻿        }
﻿
﻿        public async Task LoadInitialDataAsync(User user)
﻿        {
﻿            _currentUser = user;
﻿            _view.SetCurrentUser(user.FullName, user.CanOverrideAllocation());
﻿            await LoadScheduleAsync();
﻿        }
﻿
﻿        public async Task LoadScheduleAsync()
﻿        {
﻿            var allAssignments = await _assignmentRepo.GetAllAsync();
﻿            var allShips = await _shipRepo.GetAllAsync();
﻿            var allBerths = await _berthRepo.GetAllAsync();
﻿
﻿            var relevantAssignments = allAssignments.Where(a => 
﻿                a.Status == "Scheduled" || 
﻿                a.Status == "Arrived" || 
﻿                a.Status == "Docked" ||
﻿                a.Status == "Berlabuh" ||
﻿                (a.Status == "Departed" && a.ActualDepartureTime.HasValue && a.ActualDepartureTime.Value > DateTime.Now.AddDays(-2))
﻿            ).ToList();
﻿
﻿            var scheduleViewModel = relevantAssignments.Select(a => {
﻿                var ship = allShips.FirstOrDefault(s => s.Id == a.ShipId);
﻿                var berth = allBerths.FirstOrDefault(b => b.Id == a.BerthId);
﻿                return new DashboardScheduleViewModel
﻿                {
﻿                    ShipName = ship?.Name ?? "N/A",
﻿                    BerthName = berth?.BerthName ?? "N/A",
﻿                    ETA = a.ETA,
﻿                    ETD = a.ETD,
﻿                    Status = a.Status,
﻿                    ActualArrivalTime = a.ActualArrivalTime
﻿                };
﻿            }).OrderBy(s => s.ETA).ToList();
﻿
﻿            _view.SetScheduleDataSource(scheduleViewModel);
﻿            _view.SetBerthDataSource(allBerths);
﻿        }
﻿
﻿        public async Task AllocateBerthAsync()
﻿        {
﻿            try
﻿            {
﻿                int shipId = _view.SelectedShipId;
﻿                DateTime eta = _view.SelectedETA;
﻿                DateTime etd = _view.SelectedETD;
﻿
﻿                string errorMessage = await _portService.TryAllocateBerth(shipId, eta, etd, _currentUser);
﻿
﻿                if (string.IsNullOrEmpty(errorMessage))
﻿                {
﻿                    _notificationService.ShowMessage("Alokasi Berhasil!");
﻿                    await LoadScheduleAsync(); 
﻿                }
﻿                else
﻿                {
﻿                    _notificationService.ShowError(errorMessage);
﻿                }
﻿            }
﻿            catch (Exception ex)
﻿            {
﻿                _notificationService.ShowError($"Error: {ex.Message}");
﻿            }
﻿        }
﻿    }
﻿}
﻿