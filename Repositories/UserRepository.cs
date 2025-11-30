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
            // Fetch current user data first
            var currentUser = await GetByIdAsync(userId);
            if (currentUser == null)
            {
                throw new Exception("User not found");
            }

            // Update only the fields we want to change
            currentUser.FullName = fullName;
            currentUser.Email = email;
            currentUser.Phone = phone;
            currentUser.CompanyName = companyName;

            // Update the user with complete data
            await _client.From<User>()
                        .Where(u => u.Id == userId)
                        .Update(currentUser);
        }

        /// <summary>
        /// Change user password
        /// </summary>
        public async Task ChangePassword(int userId, string newPasswordHash)
        {
            // Fetch current user data first
            var currentUser = await GetByIdAsync(userId);
            if (currentUser == null)
            {
                throw new Exception("User not found");
            }

            // Update only the password field
            currentUser.PasswordHash = newPasswordHash; // Plain text for development

            // Update the user with complete data
            await _client.From<User>()
                        .Where(u => u.Id == userId)
                        .Update(currentUser);
        }
    }
}
