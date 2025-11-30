using HarborMaster.Models;
using HarborMaster.Presenters;
using HarborMaster.Views.Interfaces;
using System;
using System.Windows.Forms;

namespace HarborMaster.Views
{
    public partial class UserProfileWindow : Form, IUserProfileView
    {
        private readonly UserProfilePresenter _presenter;
        private readonly User _currentUser;

        public UserProfileWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            _presenter = new UserProfilePresenter(this, user);

            // Load user data when form loads
            this.Load += UserProfileWindow_Load;
        }

        private async void UserProfileWindow_Load(object sender, EventArgs e)
        {
            await _presenter.LoadUserProfileAsync();
        }

        // --- IUserProfileView Implementation ---

        // Input properties (from UI)
        public string FullName => txtFullName.Text;
        public string? Email => string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text;
        public string? Phone => string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text;
        public string? CompanyName => string.IsNullOrWhiteSpace(txtCompanyName.Text) ? null : txtCompanyName.Text;
        public string CurrentPassword => txtCurrentPassword.Text;
        public string NewPassword => txtNewPassword.Text;
        public string ConfirmPassword => txtConfirmPassword.Text;

        // Output properties (to UI)
        public string ErrorMessage
        {
            set
            {
                lblMessage.Text = value;
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Visible = !string.IsNullOrEmpty(value);
            }
        }

        public string SuccessMessage
        {
            set
            {
                lblMessage.Text = value;
                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Visible = !string.IsNullOrEmpty(value);
            }
        }

        public bool IsLoading
        {
            set
            {
                btnSaveProfile.Enabled = !value;
                btnChangePassword.Enabled = !value;
                Cursor = value ? Cursors.WaitCursor : Cursors.Default;
            }
        }

        public bool IsShipOwner => _currentUser.Role == UserRole.ShipOwner;

        // Interface methods
        public void LoadUserData(User user)
        {
            txtUsername.Text = user.Email;
            txtRole.Text = user.Role.ToString();
            txtFullName.Text = user.FullName;
            txtEmail.Text = user.Email ?? string.Empty;
            txtPhone.Text = user.Phone ?? string.Empty;
            txtCompanyName.Text = user.CompanyName ?? string.Empty;

            // Show/hide company fields based on role
            lblCompanyName.Visible = IsShipOwner;
            txtCompanyName.Visible = IsShipOwner;

            // Show role-specific help text
            if (IsShipOwner)
            {
                lblRoleInfo.Text = "Sebagai Ship Owner, kami akan menghubungi Anda melalui email/phone untuk notifikasi terkait docking request.";
                lblRoleInfo.Visible = true;
            }
            else
            {
                lblRoleInfo.Visible = false;
            }
        }

        public void CloseView()
        {
            this.Close();
        }

        public void ClearPasswordFields()
        {
            txtCurrentPassword.Clear();
            txtNewPassword.Clear();
            txtConfirmPassword.Clear();
        }

        // --- Event Handlers ---

        private async void btnSaveProfile_Click(object sender, EventArgs e)
        {
            await _presenter.UpdateProfileAsync();
        }

        private async void btnChangePassword_Click(object sender, EventArgs e)
        {
            await _presenter.ChangePasswordAsync();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
