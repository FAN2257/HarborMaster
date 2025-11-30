namespace HarborMaster.Views.Interfaces
{
    /// <summary>
    /// Interface untuk Reset Password View
    /// User memasukkan reset code dan password baru
    /// </summary>
    public interface IResetPasswordView
    {
        // Input dari user
        string ResetCode { get; }
        string NewPassword { get; }
        string ConfirmPassword { get; }

        // Output ke user
        string ErrorMessage { set; }
        bool IsLoading { set; }

        // Actions
        void CloseView();
        void ShowSuccessAndClose();
    }
}
