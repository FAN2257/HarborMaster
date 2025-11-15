// Ganti nama file dari 'Notification.cs' agar konsisten
using System.Windows.Forms; // <-- PENTING: Gunakan 'System.Windows.Forms'

namespace HarborMaster.Services
{
    public class NotificationService
    {
        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Informasi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public void ShowError(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        public bool ShowConfirmation(string question)
        {
            var result = MessageBox.Show(question, "Konfirmasi",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            return result == DialogResult.Yes;
        }
    }
}