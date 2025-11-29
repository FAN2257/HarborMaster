using HarborMaster.Models;
using System.Threading.Tasks;

namespace HarborMaster.Repositories
{
    public class UserRepository : BaseRepository<User, int>
    {
        // Metode ini spesifik untuk User
        public async Task<User> GetByUsername(string username)
        {
            var response = await _client.From<User>()
                                        .Where(u => u.Username == username)
                                        .Single();
            return response;
        }

        // New method: Get by email (primary method for login)
        public async Task<User> GetByEmail(string email)
        {
            var response = await _client.From<User>()
                                        .Where(u => u.Email == email)
                                        .Single();
            return response;
        }
    }
}