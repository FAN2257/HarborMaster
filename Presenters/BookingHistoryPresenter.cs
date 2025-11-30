using HarborMaster.Models;
using HarborMaster.Repositories;
using HarborMaster.Views.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarborMaster.Presenters
{
    public class BookingHistoryPresenter
    {
        private readonly IBookingHistoryView _view;
        private readonly DockingRequestRepository _requestRepo;
        private readonly InvoiceRepository _invoiceRepo; // Tambahan
        private readonly User _currentUser;
        private List<DockingRequest> _allBookings;

        public BookingHistoryPresenter(IBookingHistoryView view, User currentUser)
        {
            _view = view;
            _currentUser = currentUser;
            _requestRepo = new DockingRequestRepository();
            _invoiceRepo = new InvoiceRepository(); // Tambahan
            _allBookings = new List<DockingRequest>();
        }

        // Properti untuk mengecek role HarborMaster
        public bool IsHarborMaster => _currentUser.Role == UserRole.HarborMaster;

        public async Task LoadBookingsAsync()
        {
            try
            {
                _view.IsLoading = true;

                // Logika Pembedaan Role
                if (_currentUser.Role == UserRole.ShipOwner)
                {
                    // ShipOwner: Hanya melihat booking miliknya
                    var shipRepo = new ShipRepository();
                    var userShips = await shipRepo.GetShipsByOwnerIdAsync(_currentUser.Id);
                    var userShipIds = userShips.Select(s => s.Id).ToList();
                    
                    var allRequests = await _requestRepo.GetAllAsync();
                    _allBookings = allRequests
                        .Where(r => userShipIds.Contains(r.ShipId))
                        .OrderByDescending(r => r.CreatedAt)
                        .ToList();
                }
                else // Operator & HarborMaster
                {
                    // Operator & HarborMaster: Melihat semua booking
                    var allRequests = await _requestRepo.GetAllAsync();
                    _allBookings = allRequests.OrderByDescending(r => r.CreatedAt).ToList();
                }

                ApplySearchAndFilter();

                if (_allBookings.Count == 0)
                {
                    _view.ShowMessage("Tidak ada riwayat pemesanan docking yang ditemukan.");
                }
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error loading bookings: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        public void ApplySearchAndFilter()
        {
            try
            {
                var searchTerm = _view.SearchTerm?.Trim().ToLower() ?? "";
                var selectedStatus = _view.SelectedStatus;

                var filteredBookings = new List<DockingRequest>(_allBookings);

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    filteredBookings = filteredBookings
                        .Where(r => r.Id.ToString().Contains(searchTerm) || 
                                   (r.Ship != null && r.Ship.Name.ToLower().Contains(searchTerm)))
                        .ToList();
                }

                if (!string.IsNullOrWhiteSpace(selectedStatus) && selectedStatus != "All Status")
                {
                    filteredBookings = filteredBookings
                        .Where(r => r.Status == selectedStatus)
                        .ToList();
                }

                _view.SetBookingsDataSource(filteredBookings);
                _view.UpdateResultCount(filteredBookings.Count, _allBookings.Count);
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error filtering bookings: {ex.Message}");
            }
        }
        
        // Method baru untuk update invoice
        public async Task UpdateInvoiceAsync(int requestId, decimal newAmount, string notes)
        {
            try
            {
                _view.IsLoading = true;
                var invoice = await _invoiceRepo.GetByDockingRequestId(requestId);
                if (invoice == null)
                {
                    _view.ShowError("Invoice tidak ditemukan untuk permintaan ini.");
                    return;
                }

                invoice.TotalAmount = newAmount;
                invoice.Notes = string.IsNullOrWhiteSpace(notes) ? invoice.Notes : notes; // Update notes jika diisi

                await _invoiceRepo.UpdateAsync(invoice);
                _view.ShowMessage("Invoice berhasil diperbarui.");
                
                // Muat ulang data untuk menampilkan perubahan
                await LoadBookingsAsync();
            }
            catch (Exception ex)
            {
                _view.ShowError($"Gagal memperbarui invoice: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }
    }
}
