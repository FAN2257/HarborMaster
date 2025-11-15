using HarborMaster.Models;
using System;
using System.Collections.Generic; // Untuk List
using System.Threading.Tasks;

namespace HarborMaster.Repositories
{
    public class BerthAssignmentRepository : BaseRepository<BerthAssignment, int>
    {
        // Ini metode kustom yang sangat penting untuk cek konflik

        /// <summary>
        /// Mengecek apakah ada jadwal yang tumpang tindih untuk dermaga tertentu
        /// </summary>
        public async Task<bool> HasScheduleCollision(int berthId, DateTime eta, DateTime etd)
        {
            // Logika: Cari jadwal yang ada di mana
            // (Waktu Mulai Lama < Waktu Selesai Baru) DAN (Waktu Selesai Lama > Waktu Mulai Baru)
            var response = await _client.From<BerthAssignment>()
                .Where(a => a.BerthId == berthId)
                .Where(a => a.ETA < etd)
                .Where(a => a.ETD > eta)
                .Get();

            // Jika ada (Count > 0), berarti ada kolisi
            return response.Models.Count > 0;
        }

        /// <summary>
        /// Mengambil semua jadwal yang sedang aktif atau dijadwalkan
        /// </summary>
        public async Task<List<BerthAssignment>> GetActiveSchedule()
        {
            var response = await _client.From<BerthAssignment>()
                .Where(a => a.Status == "Scheduled" || a.Status == "Docked")
                .Get();

            return response.Models;
        }
    }
}