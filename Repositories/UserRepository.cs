using HarborMaster.Models;
using System.Linq;
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

        // Update user profile (full name, email, phone, company name)
        public async Task<User> UpdateProfile(int userId, string fullName, string? email, string? phone, string? companyName)
        {
            // Create new user object with updated values
            var user = new User
            {
                Id = userId,
                FullName = fullName,
                Email = email,
                Phone = phone,
                CompanyName = companyName
            };

            // Use direct Postgrest Update with filter - UPDATE ONLY the fields we want
            var response = await _client.From<User>()
                .Filter("id", Postgrest.Constants.Operator.Equals, userId)
                .Set(x => x.FullName, fullName)
                .Set(x => x.Email, email)
                .Set(x => x.Phone, phone)
                .Set(x => x.CompanyName, companyName)
                .Update();

            return response.Models.FirstOrDefault() ?? user;
        }

        // Change user password
        public async Task<bool> ChangePassword(int userId, string newPasswordHash)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            user.PasswordHash = newPasswordHash;

            // Use direct Postgrest Update with filter
            await _client.From<User>()
                .Filter("id", Postgrest.Constants.Operator.Equals, userId)
                .Update(user);

            return true;
        }
    }
}