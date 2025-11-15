using HarborMaster.Models;
using System.Linq;
using System.Threading.Tasks;

namespace HarborMaster.Repositories
{
    public class SizeMultiplierRepository : BaseRepository<SizeMultiplier, int>
    {
        /// <summary>
        /// Get multiplier based on ship length
        /// </summary>
        public async Task<SizeMultiplier?> GetByShipLength(decimal length)
        {
            var allMultipliers = await GetAllAsync();

            // Find the size category that matches this length
            return allMultipliers.FirstOrDefault(sm =>
                length >= sm.MinLength && length < sm.MaxLength);
        }

        /// <summary>
        /// Get multiplier value directly, returns 1.0 if not found (no markup)
        /// </summary>
        public async Task<decimal> GetMultiplierValue(decimal length)
        {
            var record = await GetByShipLength(length);
            return record?.Multiplier ?? 1.0m;
        }
    }
}
