using HarborMaster.Models;

namespace HarborMaster.Views.Interfaces
{
    public interface IUserProfileView
    {
        // Input properties (from user)
        string FullName { get; }
        string? Email { get; }
        string? Phone { get; }
        string? CompanyName { get; }
        string CurrentPassword { get; }
        string NewPassword { get; }
        string ConfirmPassword { get; }

        // Output properties (to UI)
        string ErrorMessage { set; }
        string SuccessMessage { set; }
        bool IsLoading { set; }
        bool IsShipOwner { get; }

        // Methods
        void LoadUserData(User user);
        void CloseView();
        void ClearPasswordFields();
    }
}
