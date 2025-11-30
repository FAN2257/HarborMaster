namespace HarborMaster.Views
{
    partial class LoginWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panelLogin = new RoundedPanel();
            lblError = new Label();
            lnkForgotPassword = new LinkLabel();
            btnGoToRegister = new Button();
            btnLogin = new Button();
            txtPassword = new TextBox();
            lblPass = new Label();
            txtEmail = new TextBox();
            lblEmail = new Label();
            lblSubtitle = new Label();
            lblTitle = new Label();
            panelTitleBar = new Panel();
            btnClose = new Button();
            btnMaximize = new Button();
            btnMinimize = new Button();
            lblTitleBar = new Label();
            panelLogin.SuspendLayout();
            panelTitleBar.SuspendLayout();
            SuspendLayout();
            // 
            // panelLogin
            // 
            panelLogin.BackColor = Color.White;
            panelLogin.Controls.Add(lblError);
            panelLogin.Controls.Add(lnkForgotPassword);
            panelLogin.Controls.Add(btnGoToRegister);
            panelLogin.Controls.Add(btnLogin);
            panelLogin.Controls.Add(txtPassword);
            panelLogin.Controls.Add(lblPass);
            panelLogin.Controls.Add(txtEmail);
            panelLogin.Controls.Add(lblEmail);
            panelLogin.Controls.Add(lblSubtitle);
            panelLogin.Controls.Add(lblTitle);
            panelLogin.CornerRadius = 15;
            panelLogin.Location = new Point(300, 165);
            panelLogin.Name = "panelLogin";
            panelLogin.Size = new Size(350, 460);
            panelLogin.TabIndex = 0;
            panelLogin.Paint += panelLogin_Paint;
            // 
            // lblError
            // 
            lblError.AutoSize = true;
            lblError.Font = new Font("Segoe UI", 8F);
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(40, 400);
            lblError.MaximumSize = new Size(270, 0);
            lblError.Name = "lblError";
            lblError.Size = new Size(0, 19);
            lblError.TabIndex = 7;
            // 
            // lnkForgotPassword
            //
            lnkForgotPassword.ActiveLinkColor = Color.FromArgb(211, 84, 0);
            lnkForgotPassword.AutoSize = false;
            lnkForgotPassword.BackColor = Color.FromArgb(255, 243, 205);
            lnkForgotPassword.BorderStyle = BorderStyle.FixedSingle;
            lnkForgotPassword.Cursor = Cursors.Hand;
            lnkForgotPassword.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lnkForgotPassword.LinkColor = Color.FromArgb(230, 126, 34);
            lnkForgotPassword.Location = new Point(40, 255);
            lnkForgotPassword.Name = "lnkForgotPassword";
            lnkForgotPassword.Size = new Size(270, 25);
            lnkForgotPassword.TabIndex = 4;
            lnkForgotPassword.TabStop = true;
            lnkForgotPassword.Text = "🔐 Lupa Password? Klik di sini";
            lnkForgotPassword.TextAlign = ContentAlignment.MiddleCenter;
            lnkForgotPassword.VisitedLinkColor = Color.FromArgb(230, 126, 34);
            lnkForgotPassword.LinkClicked += lnkForgotPassword_LinkClicked;
            // 
            // btnGoToRegister
            // 
            btnGoToRegister.BackColor = Color.FromArgb(100, 181, 246);
            btnGoToRegister.Cursor = Cursors.Hand;
            btnGoToRegister.FlatAppearance.BorderSize = 0;
            btnGoToRegister.FlatStyle = FlatStyle.Flat;
            btnGoToRegister.Font = new Font("Segoe UI", 10F);
            btnGoToRegister.ForeColor = Color.White;
            btnGoToRegister.Location = new Point(40, 357);
            btnGoToRegister.Name = "btnGoToRegister";
            btnGoToRegister.Size = new Size(270, 40);
            btnGoToRegister.TabIndex = 6;
            btnGoToRegister.Text = "Belum punya akun? Daftar disini";
            btnGoToRegister.UseVisualStyleBackColor = false;
            btnGoToRegister.Click += btnGoToRegister_Click;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.FromArgb(52, 152, 219);
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(40, 296);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(270, 45);
            btnLogin.TabIndex = 5;
            btnLogin.Text = "LOGIN";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // txtPassword
            // 
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.Font = new Font("Segoe UI", 11F);
            txtPassword.Location = new Point(40, 215);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(270, 32);
            txtPassword.TabIndex = 2;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // lblPass
            // 
            lblPass.AutoSize = true;
            lblPass.Font = new Font("Segoe UI", 9F);
            lblPass.ForeColor = Color.Gray;
            lblPass.Location = new Point(40, 190);
            lblPass.Name = "lblPass";
            lblPass.Size = new Size(70, 20);
            lblPass.TabIndex = 4;
            lblPass.Text = "Password";
            // 
            // txtEmail
            // 
            txtEmail.BorderStyle = BorderStyle.FixedSingle;
            txtEmail.Font = new Font("Segoe UI", 11F);
            txtEmail.Location = new Point(40, 145);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(270, 32);
            txtEmail.TabIndex = 1;
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Font = new Font("Segoe UI", 9F);
            lblEmail.ForeColor = Color.Gray;
            lblEmail.Location = new Point(40, 120);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(46, 20);
            lblEmail.TabIndex = 2;
            lblEmail.Text = "Email";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 9F);
            lblSubtitle.ForeColor = Color.Gray;
            lblSubtitle.Location = new Point(70, 75);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(211, 20);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Masukkan email dan password";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(52, 73, 94);
            lblTitle.Location = new Point(44, 25);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(266, 50);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "HarborMaster";
            // 
            // panelTitleBar
            // 
            panelTitleBar.BackColor = Color.FromArgb(89, 171, 227);
            panelTitleBar.Controls.Add(btnClose);
            panelTitleBar.Controls.Add(btnMaximize);
            panelTitleBar.Controls.Add(btnMinimize);
            panelTitleBar.Controls.Add(lblTitleBar);
            panelTitleBar.Dock = DockStyle.Top;
            panelTitleBar.Location = new Point(0, 0);
            panelTitleBar.Name = "panelTitleBar";
            panelTitleBar.Size = new Size(989, 40);
            panelTitleBar.TabIndex = 1;
            panelTitleBar.MouseDown += panelTitleBar_MouseDown;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.BackColor = Color.FromArgb(231, 76, 60);
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(949, 0);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(40, 40);
            btnClose.TabIndex = 3;
            btnClose.Text = "✕";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // btnMaximize
            // 
            btnMaximize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMaximize.BackColor = Color.FromArgb(52, 152, 219);
            btnMaximize.FlatAppearance.BorderSize = 0;
            btnMaximize.FlatStyle = FlatStyle.Flat;
            btnMaximize.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnMaximize.ForeColor = Color.White;
            btnMaximize.Location = new Point(909, 0);
            btnMaximize.Name = "btnMaximize";
            btnMaximize.Size = new Size(40, 40);
            btnMaximize.TabIndex = 2;
            btnMaximize.Text = "□";
            btnMaximize.UseVisualStyleBackColor = false;
            btnMaximize.Click += btnMaximize_Click;
            // 
            // btnMinimize
            // 
            btnMinimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMinimize.BackColor = Color.FromArgb(52, 152, 219);
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.FlatStyle = FlatStyle.Flat;
            btnMinimize.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnMinimize.ForeColor = Color.White;
            btnMinimize.Location = new Point(869, 0);
            btnMinimize.Name = "btnMinimize";
            btnMinimize.Size = new Size(40, 40);
            btnMinimize.TabIndex = 1;
            btnMinimize.Text = "─";
            btnMinimize.UseVisualStyleBackColor = false;
            btnMinimize.Click += btnMinimize_Click;
            // 
            // lblTitleBar
            // 
            lblTitleBar.AutoSize = true;
            lblTitleBar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTitleBar.ForeColor = Color.White;
            lblTitleBar.Location = new Point(10, 10);
            lblTitleBar.Name = "lblTitleBar";
            lblTitleBar.Size = new Size(184, 23);
            lblTitleBar.TabIndex = 0;
            lblTitleBar.Text = "Login - HarborMaster";
            // 
            // LoginWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(89, 171, 227);
            ClientSize = new Size(989, 677);
            Controls.Add(panelTitleBar);
            Controls.Add(panelLogin);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(900, 600);
            Name = "LoginWindow";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "LoginWindow";
            panelLogin.ResumeLayout(false);
            panelLogin.PerformLayout();
            panelTitleBar.ResumeLayout(false);
            panelTitleBar.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private RoundedPanel panelLogin;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnGoToRegister;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.LinkLabel lnkForgotPassword;
        private System.Windows.Forms.Panel panelTitleBar;
        private System.Windows.Forms.Label lblTitleBar;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnMaximize;
        private System.Windows.Forms.Button btnClose;

        // Custom Panel dengan Rounded Corners
        public class RoundedPanel : System.Windows.Forms.Panel
        {
            public int CornerRadius { get; set; } = 10;

            protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
            {
                base.OnPaint(e);

                // Enable anti-aliasing
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Create rounded rectangle path
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, this.Width - 1, this.Height - 1);
                int radius = CornerRadius;

                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();

                // Set region for rounded corners
                this.Region = new System.Drawing.Region(path);
            }
        }
    }
}