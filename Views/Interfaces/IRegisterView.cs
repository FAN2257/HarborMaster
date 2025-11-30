using HarborMaster.Models;

namespace HarborMaster.Views.Interfaces
{
    public interface IRegisterView
    {
        // Properti 'get' untuk input
        string Email { get; }
        string Password { get; }
        string ConfirmPassword { get; }
        string FullName { get; }
        UserRole SelectedRole { get; } // NEW: Selected role from ComboBox

        // Properti 'set' untuk kontrol UI
        string ErrorMessage { set; }
        bool IsLoading { set; }

        // Metode kontrol
        void CloseView();
        void ShowViewAsDialog();
    }
}