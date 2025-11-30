using HarborMaster.Models;
using System.Linq;
using System.Threading.Tasks;

namespace HarborMaster.Repositories
{
    public class UserRepository : BaseRepository<User, int>
    {
        // Get user by email (primary method for login)
        public async Task<User> GetByEmail(string email)
        {
            var response = await _client.From<User>()
                                        .Where(u => u.Email == email)
                                        .Single();
            return response;
        }

        /// <summary>
        /// Update user profile (full name, email, phone, company name)
        /// </summary>
        public async Task UpdateProfile(int userId, string fullName, string? email, string? phone, string? companyName)
        {
            var updateData = new User
            {
                Id = userId,
                FullName = fullName,
                Email = email,
                Phone = phone,
                CompanyName = companyName
            };

            await _client.From<User>()
                        .Where(u => u.Id == userId)
                        .Update(updateData);
        }

        /// <summary>
        /// Change user password
        /// </summary>
        public async Task ChangePassword(int userId, string newPasswordHash)
        {
            var updateData = new User
            {
                Id = userId,
                PasswordHash = newPasswordHash // Plain text for development
            };

            await _client.From<User>()
                        .Where(u => u.Id == userId)
                        .Update(updateData);
        }
    }
}
