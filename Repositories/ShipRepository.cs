using HarborMaster.Models;

namespace HarborMaster.Repositories
{
    // Untuk saat ini, ShipRepository tidak butuh metode khusus
    // Semua fungsi (GetAll, GetById, Insert, dll) sudah ada di BaseRepository
    public class ShipRepository : BaseRepository<Ship, int>
    {
        // Tambahkan metode kustom di sini jika perlu
        // Contoh: public async Task<List<Ship>> GetShipsByFlag(string flag) { ... }
    }
}