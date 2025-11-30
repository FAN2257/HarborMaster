using HarborMaster.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborMaster.Repositories
{
    public class DockingRequestRepository : BaseRepository<DockingRequest, int>
    {
        public override async Task<List<DockingRequest>> GetAllAsync()
        {
            var response = await _client.From<DockingRequest>()
                .Select("*, ship:ships(*)")
                .Get();
            
            return response.Models;
        }

        public async Task<List<DockingRequest>> GetRequestsByShip(int shipId)
        {
            var response = await _client.From<DockingRequest>()
                .Select("*, ship:ships(*)")
                .Filter("ship_id", Postgrest.Constants.Operator.Equals, shipId)
                .Get();

            return response.Models;
        }

        public async Task<List<DockingRequest>> GetRequestsByOwner(int ownerId)
        {
            var response = await _client.From<DockingRequest>()
                .Select("*, ship:ships(*)")
                .Filter("owner_id", Postgrest.Constants.Operator.Equals, ownerId)
                .Get();
            
            return response.Models;
        }

        public async Task<List<DockingRequest>> GetPendingRequests()
        {
            var response = await _client.From<DockingRequest>()
                .Select("*, ship:ships(*)")
                .Filter("status", Postgrest.Constants.Operator.Equals, "Pending")
                .Get();
            
            return response.Models;
        }

        public async Task<int> GetPendingRequestCount()
        {
            var response = await _client.From<DockingRequest>()
                .Filter("status", Postgrest.Constants.Operator.Equals, "Pending")
                .Count(Postgrest.Constants.CountType.Exact);

            return (int)response;
        }
    }
}
