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
            btnGoToRegister = new Button();
            btnLogin = new Button();
            txtPassword = new TextBox();
            lblPass = new Label();
            txtUsername = new TextBox();
            lblUser = new Label();
            lblSubtitle = new Label();
            lblTitle = new Label();
            panelLogin.SuspendLayout();
            SuspendLayout();
            // 
            // panelLogin
            // 
            panelLogin.BackColor = Color.White;
            panelLogin.Controls.Add(lblError);
            panelLogin.Controls.Add(btnGoToRegister);
            panelLogin.Controls.Add(btnLogin);
            panelLogin.Controls.Add(txtPassword);
            panelLogin.Controls.Add(lblPass);
            panelLogin.Controls.Add(txtUsername);
            panelLogin.Controls.Add(lblUser);
            panelLogin.Controls.Add(lblSubtitle);
            panelLogin.Controls.Add(lblTitle);
            panelLogin.CornerRadius = 15;
            panelLogin.Location = new Point(300, 125);
            panelLogin.Name = "panelLogin";
            panelLogin.Size = new Size(350, 420);
            panelLogin.TabIndex = 0;
            // 
            // lblError
            // 
            lblError.AutoSize = true;
            lblError.Font = new Font("Segoe UI", 8F);
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(40, 375);
            lblError.MaximumSize = new Size(270, 0);
            lblError.Name = "lblError";
            lblError.Size = new Size(0, 19);
            lblError.TabIndex = 8;
            // 
            // btnGoToRegister
            // 
            btnGoToRegister.BackColor = Color.FromArgb(100, 181, 246);
            btnGoToRegister.Cursor = Cursors.Hand;
            btnGoToRegister.FlatAppearance.BorderSize = 0;
            btnGoToRegister.FlatStyle = FlatStyle.Flat;
            btnGoToRegister.Font = new Font("Segoe UI", 10F);
            btnGoToRegister.ForeColor = Color.White;
            btnGoToRegister.Location = new Point(40, 325);
            btnGoToRegister.Name = "btnGoToRegister";
            btnGoToRegister.Size = new Size(270, 40);
            btnGoToRegister.TabIndex = 7;
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
            btnLogin.Location = new Point(40, 270);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(270, 45);
            btnLogin.TabIndex = 6;
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
            txtPassword.TabIndex = 5;
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
            // txtUsername
            // 
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            txtUsername.Font = new Font("Segoe UI", 11F);
            txtUsername.Location = new Point(40, 145);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(270, 32);
            txtUsername.TabIndex = 3;
            // 
            // lblUser
            // 
            lblUser.AutoSize = true;
            lblUser.Font = new Font("Segoe UI", 9F);
            lblUser.ForeColor = Color.Gray;
            lblUser.Location = new Point(40, 120);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(75, 20);
            lblUser.TabIndex = 2;
            lblUser.Text = "Username";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 9F);
            lblSubtitle.ForeColor = Color.Gray;
            lblSubtitle.Location = new Point(40, 75);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(270, 20);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Masukkan kredensial Anda untuk masuk";
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
            // LoginWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(89, 171, 227);
            ClientSize = new Size(900, 600);
            Controls.Add(panelLogin);
            FormBorderStyle = FormBorderStyle.None;
            Name = "LoginWindow";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "LoginWindow";
            panelLogin.ResumeLayout(false);
            panelLogin.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private RoundedPanel panelLogin;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnGoToRegister;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Label lblSubtitle;

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