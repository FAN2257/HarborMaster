using HarborMaster.Models;
using System.Threading.Tasks;

namespace HarborMaster.Repositories
{
    public class ShipTypeMultiplierRepository : BaseRepository<ShipTypeMultiplier, int>
    {
        /// <summary>
        /// Get multiplier by ship type name
        /// </summary>
        public async Task<ShipTypeMultiplier?> GetByShipType(string shipType)
        {
            var response = await _client.From<ShipTypeMultiplier>()
                .Filter("ship_type", Postgrest.Constants.Operator.Equals, shipType)
                .Single();

            return response;
        }

        /// <summary>
        /// Get multiplier value directly, returns 1.0 if not found (no markup)
        /// </summary>
        public async Task<decimal> GetMultiplierValue(string shipType)
        {
            var record = await GetByShipType(shipType);
            return record?.Multiplier ?? 1.0m;
        }
    }
}
