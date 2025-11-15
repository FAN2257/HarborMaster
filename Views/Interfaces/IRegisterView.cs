namespace HarborMaster.Views.Interfaces
{
    public interface IRegisterView
    {
        // Properti 'get' untuk input
        string Username { get; }
        string Password { get; }
        string ConfirmPassword { get; }
        string FullName { get; }

        // Properti 'set' untuk kontrol UI
        string ErrorMessage { set; }
        bool IsLoading { set; }

        // Metode kontrol
        void CloseView();
        void ShowViewAsDialog();
    }
}