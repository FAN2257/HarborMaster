namespace HarborMaster.Views.Interfaces
{
    /// <summary>
    /// Interface untuk Forgot Password View
    /// User memasukkan email untuk request reset code
    /// </summary>
    public interface IForgotPasswordView
    {
        // Input dari user
        string Email { get; }

        // Output ke user
        string ErrorMessage { set; }
        string SuccessMessage { set; }
        bool IsLoading { set; }

        // Actions
        void CloseView();
        void CloseWithSuccess();
    }
}
