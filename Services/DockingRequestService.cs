using HarborMaster.Models;
using HarborMaster.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborMaster.Services
{
    /// <summary>
    /// Service for handling docking request workflow
    /// ShipOwner submits → Operator approves/rejects → BerthAssignment created
    /// </summary>
    public class DockingRequestService
    {
        private readonly DockingRequestRepository _requestRepo;
        private readonly ShipRepository _shipRepo;
        private readonly PortService _portService;
        private readonly BerthAssignmentRepository _assignmentRepo;
        private readonly UserRepository _userRepo;

        public DockingRequestService()
        {
            _requestRepo = new DockingRequestRepository();
            _shipRepo = new ShipRepository();
            _portService = new PortService();
            _assignmentRepo = new BerthAssignmentRepository();
            _userRepo = new UserRepository();
        }

        /// <summary>
        /// Submit a new docking request (ShipOwner)
        /// Returns: empty string if success, error message if failed
        /// </summary>
        public async Task<string> SubmitRequest(DockingRequest request)
        {
            try
            {
                // Validate request data
                var validationError = request.Validate();
                if (!string.IsNullOrEmpty(validationError))
                    return validationError;

                // Verify ship exists and belongs to owner
                var ship = await _shipRepo.GetByIdAsync(request.ShipId);
                if (ship == null)
                    return "Kapal tidak ditemukan";

                if (ship.OwnerId != request.OwnerId)
                    return "Anda tidak memiliki kapal ini";

                // Set initial status and timestamp
                request.Status = "Pending";
                request.CreatedAt = DateTime.Now;

                // Insert to database
                await _requestRepo.InsertAsync(request);

                return string.Empty; // Success
            }
            catch (Exception ex)
            {
                return $"Error submit request: {ex.Message}";
            }
        }

        /// <summary>
        /// Approve a docking request and allocate berth (Operator/HarborMaster)
        /// Returns: empty string if success, error message if failed
        /// </summary>
        public async Task<string> ApproveRequest(int requestId, int processorId, bool allowOverride = false)
        {
            try
            {
                // Get the request
                var request = await _requestRepo.GetByIdAsync(requestId);
                if (request == null)
                    return "Request tidak ditemukan";

                if (!request.IsPending())
                    return $"Request sudah diproses dengan status: {request.Status}";

                // Get ship data
                var ship = await _shipRepo.GetByIdAsync(request.ShipId);
                if (ship == null)
                    return "Kapal tidak ditemukan";

                // Get processor user data
                var processor = await _userRepo.GetByIdAsync(processorId);
                if (processor == null)
                    return "User processor tidak ditemukan";

                // Try to allocate berth using PortService
                var allocationResult = await _portService.TryAllocateBerth(
                    shipId: request.ShipId,
                    eta: request.RequestedETA,
                    etd: request.RequestedETD,
                    user: processor
                );

                if (!string.IsNullOrEmpty(allocationResult))
                {
                    // Allocation failed
                    return $"Gagal alokasi dermaga: {allocationResult}";
                }

                // Get the created berth assignment ID
                // The PortService should have created a BerthAssignment
                var assignments = await _assignmentRepo.GetByShipId(request.ShipId);
                var latestAssignment = assignments.Find(a =>
                    a.ETA == request.RequestedETA &&
                    a.ETD == request.RequestedETD);

                if (latestAssignment == null)
                    return "BerthAssignment tidak ditemukan setelah alokasi";

                // Update request status
                request.Status = "Approved";
                request.ProcessedBy = processorId;
                request.ProcessedAt = DateTime.Now;
                request.BerthAssignmentId = latestAssignment.Id;

                await _requestRepo.UpdateAsync(request);

                return string.Empty; // Success
            }
            catch (Exception ex)
            {
                return $"Error approve request: {ex.Message}";
            }
        }

        /// <summary>
        /// Reject a docking request (Operator/HarborMaster)
        /// </summary>
        public async Task<string> RejectRequest(int requestId, int processorId, string rejectionReason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rejectionReason))
                    return "Alasan penolakan harus diisi";

                var request = await _requestRepo.GetByIdAsync(requestId);
                if (request == null)
                    return "Request tidak ditemukan";

                if (!request.IsPending())
                    return $"Request sudah diproses dengan status: {request.Status}";

                // Update request status
                request.Status = "Rejected";
                request.ProcessedBy = processorId;
                request.ProcessedAt = DateTime.Now;
                request.RejectionReason = rejectionReason;

                await _requestRepo.UpdateAsync(request);

                return string.Empty; // Success
            }
            catch (Exception ex)
            {
                return $"Error reject request: {ex.Message}";
            }
        }

        /// <summary>
        /// Cancel a pending request (ShipOwner)
        /// </summary>
        public async Task<string> CancelRequest(int requestId, int ownerId)
        {
            try
            {
                var request = await _requestRepo.GetByIdAsync(requestId);
                if (request == null)
                    return "Request tidak ditemukan";

                if (request.OwnerId != ownerId)
                    return "Anda tidak berhak membatalkan request ini";

                if (!request.IsPending())
                    return $"Request sudah diproses dengan status: {request.Status}";

                // Update request status
                request.Status = "Cancelled";
                request.ProcessedAt = DateTime.Now;

                await _requestRepo.UpdateAsync(request);

                return string.Empty; // Success
            }
            catch (Exception ex)
            {
                return $"Error cancel request: {ex.Message}";
            }
        }

        /// <summary>
        /// Get all pending requests (for Operator/HarborMaster)
        /// </summary>
        public async Task<List<DockingRequest>> GetPendingRequests()
        {
            return await _requestRepo.GetPendingRequests();
        }

        /// <summary>
        /// Get requests by owner (for ShipOwner)
        /// </summary>
        public async Task<List<DockingRequest>> GetRequestsByOwner(int ownerId)
        {
            return await _requestRepo.GetRequestsByOwner(ownerId);
        }

        /// <summary>
        /// Get all requests (for HarborMaster overview)
        /// </summary>
        public async Task<List<DockingRequest>> GetAllRequests()
        {
            return await _requestRepo.GetAllAsync();
        }

        /// <summary>
        /// Get request by ID
        /// </summary>
        public async Task<DockingRequest> GetRequestById(int requestId)
        {
            return await _requestRepo.GetByIdAsync(requestId);
        }

        /// <summary>
        /// Get pending request count (for dashboard)
        /// </summary>
        public async Task<int> GetPendingRequestCount()
        {
            return await _requestRepo.GetPendingRequestCount();
        }
    }
}
