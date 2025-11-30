using HarborMaster.Models; // <-- Tambahkan ini

namespace HarborMaster.Views.Interfaces
{
    public interface ILoginView
    {
        string Email { get; }
        string Password { get; }
        string ErrorMessage { set; }
        bool IsLoading { set; }
        User? LoggedInUser { get; } // Nullable untuk keamanan

        void CloseView();
        void ShowViewAsDialog();
        void HandleLoginSuccess(User user);
    }
}