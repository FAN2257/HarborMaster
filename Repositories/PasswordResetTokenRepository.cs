using HarborMaster.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HarborMaster.Repositories
{
    public class PasswordResetTokenRepository : BaseRepository<PasswordResetToken, int>
    {
        /// <summary>
        /// Ambil token berdasarkan token string dan validasi
        /// </summary>
        public async Task<PasswordResetToken?> GetValidToken(string token)
        {
            try
            {
                var response = await _client.From<PasswordResetToken>()
                    .Where(t => t.Token == token)
                    .Where(t => t.IsUsed == false)
                    .Single();

                // Cek apakah sudah expired
                if (response != null && response.ExpiresAt < DateTime.UtcNow)
                {
                    return null; // Token sudah expired
                }

                return response;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Tandai token sebagai sudah digunakan
        /// </summary>
        public async Task MarkAsUsed(int tokenId)
        {
            await _client.From<PasswordResetToken>()
                .Filter("id", Postgrest.Constants.Operator.Equals, tokenId)
                .Set(x => x.IsUsed, true)
                .Update();
        }

        /// <summary>
        /// Hapus semua token lama untuk user tertentu
        /// </summary>
        public async Task DeleteUserTokens(int userId)
        {
            var userTokens = await _client.From<PasswordResetToken>()
                .Where(t => t.UserId == userId)
                .Get();

            foreach (var token in userTokens.Models)
            {
                await DeleteAsync(token);
            }
        }
    }
}
