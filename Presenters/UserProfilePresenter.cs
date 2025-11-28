using HarborMaster.Models;
using HarborMaster.Services;
using HarborMaster.Views.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HarborMaster.Presenters
{
    public class UserProfilePresenter
    {
        private readonly IUserProfileView _view;
        private readonly UserProfileService _profileService;
        private readonly User _currentUser;

        public UserProfilePresenter(IUserProfileView view, User currentUser)
        {
            _view = view;
            _currentUser = currentUser;
            _profileService = new UserProfileService();

            Console.WriteLine($"[INFO] UserProfilePresenter initialized for user: {currentUser.Username} (ID: {currentUser.Id})");
        }

        /// <summary>
        /// Load user data when the form opens
        /// </summary>
        public async Task LoadUserProfileAsync()
        {
            Console.WriteLine("[INFO] LoadUserProfileAsync started");
            try
            {
                _view.IsLoading = true;
                _view.ErrorMessage = string.Empty;

                // Reload user data from database to ensure we have latest info
                var user = await _profileService.GetUserProfileAsync(_currentUser.Id);
                if (user != null)
                {
                    _view.LoadUserData(user);
                    Console.WriteLine("[SUCCESS] User profile loaded into view");
                }
                else
                {
                    Console.WriteLine("[ERROR] User data is null");
                    _view.ErrorMessage = "Gagal memuat data user. User tidak ditemukan.";

                    MessageBox.Show(
                        "User tidak ditemukan dalam database.\n\nSilakan login ulang.",
                        "Error - User Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"[ERROR] LoadUserProfileAsync failed: {ex.Message}");
                string errorMsg = $"Gagal memuat data profile:\n{ex.Message}";
                _view.ErrorMessage = errorMsg;

                // Show popup for critical errors
                MessageBox.Show(
                    $"{errorMsg}\n\nCek console untuk detail lengkap.",
                    "Error - Load Profile",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        /// <summary>
        /// Update user profile information
        /// </summary>
        public async Task UpdateProfileAsync()
        {
            Console.WriteLine("[INFO] UpdateProfileAsync started");
            _view.IsLoading = true;
            _view.ErrorMessage = string.Empty;
            _view.SuccessMessage = string.Empty;

            try
            {
                // Get input from view
                string fullName = _view.FullName;
                string? email = _view.Email;
                string? phone = _view.Phone;
                string? companyName = _view.CompanyName;

                Console.WriteLine($"[INFO] Updating profile for user {_currentUser.Username}");

                // Call service to update profile
                string result = await _profileService.UpdateProfileAsync(
                    _currentUser.Id,
                    fullName,
                    email,
                    phone,
                    companyName
                );

                if (string.IsNullOrEmpty(result))
                {
                    // Success
                    Console.WriteLine("[SUCCESS] Profile updated successfully in presenter");
                    _view.SuccessMessage = "✅ Profile berhasil diupdate!";

                    // Update current user object
                    _currentUser.FullName = fullName;
                    _currentUser.Email = email;
                    _currentUser.Phone = phone;
                    _currentUser.CompanyName = companyName;

                    // Show success popup
                    MessageBox.Show(
                        "Profile Anda berhasil diupdate!",
                        "Sukses",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    // Business logic error or database error
                    Console.WriteLine($"[ERROR] Update failed: {result}");
                    _view.ErrorMessage = result;

                    // Show error popup for database errors
                    if (result.Contains("ERROR DATABASE") || result.Contains("Database Error"))
                    {
                        MessageBox.Show(
                            result,
                            "Database Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"[ERROR] UpdateProfileAsync exception: {ex.Message}");
                string errorMsg = $"Unexpected Error: {ex.Message}";
                _view.ErrorMessage = errorMsg;

                MessageBox.Show(
                    $"{errorMsg}\n\nTipe: {ex.GetType().Name}\n\nCek console untuk detail lengkap.",
                    "Critical Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        /// <summary>
        /// Change user password
        /// </summary>
        public async Task ChangePasswordAsync()
        {
            Console.WriteLine("[INFO] ChangePasswordAsync started");
            _view.IsLoading = true;
            _view.ErrorMessage = string.Empty;
            _view.SuccessMessage = string.Empty;

            try
            {
                // Get input from view
                string currentPassword = _view.CurrentPassword;
                string newPassword = _view.NewPassword;
                string confirmPassword = _view.ConfirmPassword;

                Console.WriteLine($"[INFO] Changing password for user {_currentUser.Username}");

                // Call service to change password
                string result = await _profileService.ChangePasswordAsync(
                    _currentUser.Id,
                    currentPassword,
                    newPassword,
                    confirmPassword
                );

                if (string.IsNullOrEmpty(result))
                {
                    // Success
                    Console.WriteLine("[SUCCESS] Password changed successfully");
                    _view.SuccessMessage = "✅ Password berhasil diubah!";
                    _view.ClearPasswordFields();

                    // Show success popup
                    MessageBox.Show(
                        "Password Anda berhasil diubah!\n\nGunakan password baru untuk login berikutnya.",
                        "Sukses - Password Changed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    // Business logic error
                    Console.WriteLine($"[ERROR] Change password failed: {result}");
                    _view.ErrorMessage = result;

                    // Show popup for database errors
                    if (result.Contains("Database Error"))
                    {
                        MessageBox.Show(
                            result,
                            "Database Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"[ERROR] ChangePasswordAsync exception: {ex.Message}");
                string errorMsg = $"Unexpected Error: {ex.Message}";
                _view.ErrorMessage = errorMsg;

                MessageBox.Show(
                    $"{errorMsg}\n\nTipe: {ex.GetType().Name}\n\nCek console untuk detail lengkap.",
                    "Critical Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                _view.IsLoading = false;
            }
        }
    }
}
