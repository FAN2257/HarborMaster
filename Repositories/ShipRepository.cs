using HarborMaster.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborMaster.Repositories
{
    public class ShipRepository : BaseRepository<Ship, int>
    {
        public async Task<List<Ship>> GetShipsByOwnerIdAsync(int ownerId)
        {
            var response = await _client.From<Ship>()
                                        .Where(s => s.OwnerId == ownerId)
                                        .Get();
            return response.Models;
        }
    }
}