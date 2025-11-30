using HarborMaster.Repositories;
using HarborMaster.Models;
using System;
using System.Threading.Tasks;

namespace HarborMaster.Services
{
    public class AuthenticationService
    {
        private readonly UserRepository _userRepository;
        public AuthenticationService()
        {
            _userRepository = new UserRepository();
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
    }
}
