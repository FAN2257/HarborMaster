using HarborMaster.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborMaster.Repositories
{
    public class BerthRepository : BaseRepository<Berth, int>
    {
        // Ini metode kustom yang kita butuhkan untuk logika bisnis

        /// <summary>
        /// Menemukan semua dermaga yang tersedia & cocok secara fisik
        /// </summary>
        public async Task<List<Berth>> GetPhysicallySuitableBerths(double shipLength, double shipDraft)
        {
            var response = await _client.From<Berth>()
                                        .Where(b => b.IsAvailable == true)
                                        .Where(b => b.MaxLength >= shipLength)
                                        .Where(b => b.MaxDraft >= shipDraft)
                                        .Get();

            return response.Models;
        }
    }
}