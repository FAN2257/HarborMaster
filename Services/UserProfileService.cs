using HarborMaster.Repositories;
using HarborMaster.Models;
using System;
using System.Threading.Tasks;

namespace HarborMaster.Services
{
    public class UserProfileService
    {
        private readonly UserRepository _userRepository;

        public UserProfileService()
        {
            _userRepository = new UserRepository();
        }

        // Helper method for detailed error logging
        private void LogError(string method, Exception ex)
        {
            Console.WriteLine($"[ERROR] UserProfileService.{method}");
            Console.WriteLine($"  Message: {ex.Message}");
            Console.WriteLine($"  Type: {ex.GetType().Name}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"  Inner Exception: {ex.InnerException.Message}");
            }
            Console.WriteLine($"  StackTrace: {ex.StackTrace}");
            Console.WriteLine("---");
        }

        /// <summary>
        /// Update user profile information
        /// </summary>
        public async Task<string> UpdateProfileAsync(int userId, string fullName, string? email, string? phone, string? companyName)
        {
            Console.WriteLine($"[INFO] UpdateProfileAsync called for UserId: {userId}");
            Console.WriteLine($"  FullName: {fullName}");
            Console.WriteLine($"  Email: {email ?? "(null)"}");
            Console.WriteLine($"  Phone: {phone ?? "(null)"}");
            Console.WriteLine($"  CompanyName: {companyName ?? "(null)"}");

            // Validate input
            if (string.IsNullOrWhiteSpace(fullName))
            {
                Console.WriteLine("[VALIDATION] Full name is empty");
                return "Nama lengkap tidak boleh kosong.";
            }

            // Validate email format if provided
            if (!string.IsNullOrWhiteSpace(email))
            {
                if (!IsValidEmail(email))
                {
                    Console.WriteLine($"[VALIDATION] Invalid email format: {email}");
                    return "Format email tidak valid. Contoh: user@example.com";
                }
            }

            // Validate phone format if provided
            if (!string.IsNullOrWhiteSpace(phone))
            {
                if (!IsValidPhone(phone))
                {
                    Console.WriteLine($"[VALIDATION] Invalid phone format: {phone}");
                    return "Format nomor telepon tidak valid. Gunakan angka dan karakter +, -, (, ) saja.";
                }
            }

            try
            {
                Console.WriteLine("[INFO] Calling UserRepository.UpdateProfile...");
                await _userRepository.UpdateProfile(userId, fullName, email, phone, companyName);
                Console.WriteLine("[SUCCESS] Profile updated successfully");
                return string.Empty; // Success
            }
            catch (Postgrest.Exceptions.PostgrestException pgEx)
            {
                // PostgreSQL specific errors
                LogError("UpdateProfileAsync", pgEx);

                if (pgEx.Message.Contains("42703"))
                {
                    return "ERROR DATABASE: Kolom email/phone/company_name belum ada di tabel users.\n\nSilakan jalankan SQL migration terlebih dahulu:\nALTER TABLE users ADD COLUMN email VARCHAR(255);\nALTER TABLE users ADD COLUMN phone VARCHAR(50);\nALTER TABLE users ADD COLUMN company_name VARCHAR(255);";
                }
                else if (pgEx.Message.Contains("23505"))
                {
                    return "Email sudah digunakan oleh user lain. Gunakan email yang berbeda.";
                }
                else
                {
                    return $"Database Error: {pgEx.Message}\n\nCek console untuk detail lengkap.";
                }
            }
            catch (Exception ex)
            {
                LogError("UpdateProfileAsync", ex);
                return $"Gagal update profile: {ex.Message}\n\nTipe Error: {ex.GetType().Name}\nCek console untuk detail lengkap.";
            }
        }

        /// <summary>
        /// Change user password
        /// </summary>
        public async Task<string> ChangePasswordAsync(int userId, string currentPassword, string newPassword, string confirmPassword)
        {
            Console.WriteLine($"[INFO] ChangePasswordAsync called for UserId: {userId}");

            // Validate input
            if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                Console.WriteLine("[VALIDATION] Password fields are empty");
                return "Password tidak boleh kosong.";
            }

            if (newPassword != confirmPassword)
            {
                Console.WriteLine("[VALIDATION] Password confirmation doesn't match");
                return "Password baru dan konfirmasi password tidak cocok.";
            }

            if (newPassword.Length < 6)
            {
                Console.WriteLine($"[VALIDATION] Password too short: {newPassword.Length} chars");
                return "Password baru minimal 6 karakter.";
            }

            if (currentPassword == newPassword)
            {
                Console.WriteLine("[VALIDATION] New password same as old password");
                return "Password baru harus berbeda dengan password lama.";
            }

            try
            {
                Console.WriteLine("[INFO] Fetching user data...");
                // Get current user
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    Console.WriteLine($"[ERROR] User not found: {userId}");
                    return "User tidak ditemukan.";
                }

                Console.WriteLine("[INFO] Verifying current password...");
                // Verify current password (plain text for now, as per current implementation)
                if (user.PasswordHash != currentPassword)
                {
                    Console.WriteLine("[VALIDATION] Current password is incorrect");
                    return "Password lama tidak sesuai.";
                }

                Console.WriteLine("[INFO] Updating password...");
                // Update password (plain text for now)
                // TODO: Implement BCrypt hashing when moving to production
                await _userRepository.ChangePassword(userId, newPassword);
                Console.WriteLine("[SUCCESS] Password changed successfully");
                return string.Empty; // Success
            }
            catch (Postgrest.Exceptions.PostgrestException pgEx)
            {
                LogError("ChangePasswordAsync", pgEx);
                return $"Database Error saat mengubah password: {pgEx.Message}\n\nCek console untuk detail lengkap.";
            }
            catch (Exception ex)
            {
                LogError("ChangePasswordAsync", ex);
                return $"Gagal mengubah password: {ex.Message}\n\nTipe Error: {ex.GetType().Name}\nCek console untuk detail lengkap.";
            }
        }

        /// <summary>
        /// Get user profile by ID
        /// </summary>
        public async Task<User> GetUserProfileAsync(int userId)
        {
            try
            {
                Console.WriteLine($"[INFO] GetUserProfileAsync called for UserId: {userId}");
                var user = await _userRepository.GetByIdAsync(userId);

                if (user != null)
                {
                    Console.WriteLine("[SUCCESS] User profile loaded");
                    Console.WriteLine($"  Username: {user.Username}");
                    Console.WriteLine($"  FullName: {user.FullName}");
                    Console.WriteLine($"  Role: {user.Role}");
                    Console.WriteLine($"  Email: {user.Email ?? "(null)"}");
                    Console.WriteLine($"  Phone: {user.Phone ?? "(null)"}");
                    Console.WriteLine($"  Company: {user.CompanyName ?? "(null)"}");
                }
                else
                {
                    Console.WriteLine($"[WARNING] User not found: {userId}");
                }

                return user;
            }
            catch (Exception ex)
            {
                LogError("GetUserProfileAsync", ex);
                throw; // Re-throw to be handled by presenter
            }
        }

        // Helper: Validate email format
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Helper: Validate phone format (basic validation)
        private bool IsValidPhone(string phone)
        {
            // Allow digits, spaces, +, -, (, )
            foreach (char c in phone)
            {
                if (!char.IsDigit(c) && c != '+' && c != '-' && c != '(' && c != ')' && c != ' ')
                {
                    return false;
                }
            }
            return phone.Length >= 8; // Minimum 8 digits
        }
    }
}
