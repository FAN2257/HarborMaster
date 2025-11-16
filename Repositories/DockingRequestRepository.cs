using HarborMaster.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarborMaster.Repositories
{
    /// <summary>
    /// Repository for DockingRequest operations
    /// </summary>
    public class DockingRequestRepository : BaseRepository<DockingRequest, int>
    {
        /// <summary>
        /// Get all pending docking requests (for Operator/HarborMaster)
        /// </summary>
        public async Task<List<DockingRequest>> GetPendingRequests()
        {
            var response = await _client.From<DockingRequest>()
                                        .Where(r => r.Status == "Pending")
                                        .Order("created_at", Postgrest.Constants.Ordering.Descending)
                                        .Get();

            return response.Models;
        }

        /// <summary>
        /// Get all requests from a specific ship owner (for ShipOwner view)
        /// </summary>
        public async Task<List<DockingRequest>> GetRequestsByOwner(int ownerId)
        {
            var response = await _client.From<DockingRequest>()
                                        .Where(r => r.OwnerId == ownerId)
                                        .Order("created_at", Postgrest.Constants.Ordering.Descending)
                                        .Get();

            return response.Models;
        }

        /// <summary>
        /// Get requests by ship ID
        /// </summary>
        public async Task<List<DockingRequest>> GetRequestsByShip(int shipId)
        {
            var response = await _client.From<DockingRequest>()
                                        .Where(r => r.ShipId == shipId)
                                        .Order("created_at", Postgrest.Constants.Ordering.Descending)
                                        .Get();

            return response.Models;
        }

        /// <summary>
        /// Get requests by status
        /// </summary>
        public async Task<List<DockingRequest>> GetRequestsByStatus(string status)
        {
            var response = await _client.From<DockingRequest>()
                                        .Where(r => r.Status == status)
                                        .Order("created_at", Postgrest.Constants.Ordering.Descending)
                                        .Get();

            return response.Models;
        }

        /// <summary>
        /// Get approved requests that have been assigned to berths
        /// </summary>
        public async Task<List<DockingRequest>> GetApprovedRequestsWithAssignments()
        {
            var response = await _client.From<DockingRequest>()
                                        .Where(r => r.Status == "Approved")
                                        .Where(r => r.BerthAssignmentId != null)
                                        .Order("created_at", Postgrest.Constants.Ordering.Descending)
                                        .Get();

            return response.Models;
        }

        /// <summary>
        /// Get count of pending requests (for dashboard statistics)
        /// </summary>
        public async Task<int> GetPendingRequestCount()
        {
            var requests = await GetPendingRequests();
            return requests.Count;
        }

        /// <summary>
        /// Get recent requests (last N requests)
        /// </summary>
        public async Task<List<DockingRequest>> GetRecentRequests(int limit = 10)
        {
            var response = await _client.From<DockingRequest>()
                                        .Order("created_at", Postgrest.Constants.Ordering.Descending)
                                        .Limit(limit)
                                        .Get();

            return response.Models;
        }
    }
}
