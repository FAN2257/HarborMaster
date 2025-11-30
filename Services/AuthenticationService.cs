using HarborMaster.Repositories;
using HarborMaster.Models;
using System;
using System.Threading.Tasks;

namespace HarborMaster.Services
{
    public class AuthenticationService
    {
        private readonly UserRepository _userRepository;
        private readonly PasswordResetTokenRepository _tokenRepository;
        private readonly EmailService _emailService;

        public AuthenticationService()
        {
            _userRepository = new UserRepository();
            _tokenRepository = new PasswordResetTokenRepository();
            _emailService = new EmailService();
        }

        /// <summary>
        /// Register new user with email, password, full name, and role
        /// </summary>
        public async Task<string> RegisterUserAsync(string email, string password, string fullName, UserRole role)
        {
            // 1. Validasi input
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return "Email dan password tidak boleh kosong.";
            }

            if (string.IsNullOrWhiteSpace(fullName))
            {
                return "Nama lengkap tidak boleh kosong.";
            }

            // 2. Validate email format
            if (!IsValidEmail(email))
            {
                return "Format email tidak valid.";
            }

            // 3. Cek apakah email sudah ada
            var existingUser = await _userRepository.GetByEmail(email);
            if (existingUser != null)
            {
                return "Email sudah terdaftar. Silakan gunakan email lain.";
            }

            // --- PERUBAHAN DI SINI ---
            // 4. Hapus Hashing BCrypt (saat ini menyimpan plain untuk testing)
            // string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            // -------------------------

            // 5. Buat objek User baru
            User newUser = new User
            {
                Email = email,
                // Simpan password mentah (plain text)
                PasswordHash = password, // <-- Gunakan password langsung
                FullName = fullName,
                Role = role
            };

            // 6. Simpan ke database
            try
            {
                await _userRepository.InsertAsync(newUser);
                return string.Empty; // Sukses!
            }
            catch (Postgrest.Exceptions.PostgrestException ex) // <-- Tangkap EXCEPTION SPESIFIK Supabase
            {
                // Lempar kembali exception ke Presenter
                throw new Exception($"Supabase (PGRST) Error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Lempar exception umum
                throw new Exception($"Gagal mendaftar: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Validate user login with email instead of username
        /// </summary>
        public async Task<User?> ValidateUser(string email, string password)
        {
            User? user = await _userRepository.GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            // Jika PasswordHash disimpan plain (development), bandingkan langsung.
            // Jika nanti menggunakan hashing, ganti pengecekan ini dengan BCrypt.Verify(...)
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                return null;
            }

            if (user.PasswordHash == password)
            {
                return user;
            }

            return null;
        }

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

        /// <summary>
        /// Request password reset - Generate token dan kirim via email
        /// </summary>
        public async Task<string> RequestPasswordReset(string email)
        {
            // 1. Validasi input
            if (string.IsNullOrWhiteSpace(email))
            {
                return "Email tidak boleh kosong.";
            }

            if (!IsValidEmail(email))
            {
                return "Format email tidak valid.";
            }

            // 2. Cari user berdasarkan email
            var user = await _userRepository.GetByEmail(email);
            if (user == null)
            {
                // Untuk keamanan, jangan beritahu bahwa email tidak ada
                // Return sukses tetapi tidak kirim email
                return string.Empty; // "Jika email valid, kode reset telah dikirim"
            }

            // 3. Generate random 6-digit code
            Random random = new Random();
            string resetCode = random.Next(100000, 999999).ToString();

            // 4. Hapus token lama user ini (jika ada)
            await _tokenRepository.DeleteUserTokens(user.Id);

            // 5. Buat token baru
            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = resetCode,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30), // Berlaku 30 menit
                IsUsed = false
            };

            try
            {
                await _tokenRepository.InsertAsync(resetToken);

                // 6. Kirim email dengan reset code
                bool emailSent = await _emailService.SendPasswordResetEmail(
                    user.Email, 
                    user.FullName, 
                    resetCode
                );

                if (!emailSent)
                {
                    throw new Exception("Gagal mengirim email. Silakan coba lagi.");
                }

                return string.Empty; // Sukses!
            }
            catch (Exception ex)
            {
                throw new Exception($"Gagal memproses reset password: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verify reset token dan reset password
        /// </summary>
        public async Task<string> ResetPassword(string resetCode, string newPassword)
        {
            // 1. Validasi input
            if (string.IsNullOrWhiteSpace(resetCode) || string.IsNullOrWhiteSpace(newPassword))
            {
                return "Kode reset dan password baru tidak boleh kosong.";
            }

            if (newPassword.Length < 6)
            {
                return "Password minimal 6 karakter.";
            }

            // 2. Cari token yang valid
            var token = await _tokenRepository.GetValidToken(resetCode);
            if (token == null)
            {
                return "Kode reset tidak valid atau sudah expired.";
            }

            // 3. Ambil user
            var user = await _userRepository.GetByIdAsync(token.UserId);
            if (user == null)
            {
                return "User tidak ditemukan.";
            }

            // 4. Update password (plain text untuk development)
            // Untuk production, gunakan: BCrypt.Net.BCrypt.HashPassword(newPassword)
            try
            {
                await _userRepository.ChangePassword(user.Id, newPassword);

                // 5. Tandai token sebagai sudah digunakan
                await _tokenRepository.MarkAsUsed(token.Id);

                return string.Empty; // Sukses!
            }
            catch (Exception ex)
            {
                throw new Exception($"Gagal mengubah password: {ex.Message}", ex);
            }
        }
    }
}
