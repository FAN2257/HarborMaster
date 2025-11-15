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

        public async Task<string> RegisterUserAsync(string username, string password, string fullName)
        {
            // 1. Validasi input
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return "Username dan password tidak boleh kosong.";
            }

            // 2. Cek apakah user sudah ada
            var existingUser = await _userRepository.GetByUsername(username);
            if (existingUser != null)
            {
                return "Username sudah digunakan. Silakan pilih yang lain.";
            }

            // --- PERUBAHAN DI SINI ---
            // 3. Hapus Hashing BCrypt (saat ini menyimpan plain untuk testing)
            // string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            // -------------------------

            // 4. Buat objek User baru
            User newUser = new User
            {
                Username = username,
                // Simpan password mentah (plain text)
                PasswordHash = password, // <-- Gunakan password langsung
                FullName = fullName,
                Role = UserRole.Operator
            };

            // 5. Simpan ke database
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

        // Return nullable User to avoid CS8603 when returning null
        public async Task<User?> ValidateUser(string username, string password)
        {
            User? user = await _userRepository.GetByUsername(username);
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
    }
}
