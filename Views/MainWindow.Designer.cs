namespace HarborMaster.Views
{
    partial class MainWindow
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            panelHeader = new Panel();
            btnProfile = new Button();
            btnStatistics = new Button();
            btnBerthStatus = new Button();
            btnPendingRequests = new Button();
            btnMyRequests = new Button();
            btnSubmitRequest = new Button();
            btnMyShips = new Button();
            btnAddMyShip = new Button();
            btnBack = new Button();
            btnRefreshData = new Button();
            btnAddShip = new Button();
            lblTitle = new Label();
            cardTotalKapal = new Panel();
            lblCardTotalKapalValue = new Label();
            lblCardTotalKapalTitle = new Label();
            cardSedangBerlabuh = new Panel();
            lblCardSedangBerlabuhValue = new Label();
            lblCardSedangBerlabuhTitle = new Label();
            cardKapalMenunggu = new Panel();
            lblCardKapalMenungguValue = new Label();
            lblCardKapalMenungguTitle = new Label();
            dgvSchedule = new DataGridView();
            comboShips = new ComboBox();
            dtpETA = new DateTimePicker();
            dtpETD = new DateTimePicker();
            panelHeader.SuspendLayout();
            cardTotalKapal.SuspendLayout();
            cardSedangBerlabuh.SuspendLayout();
            cardKapalMenunggu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSchedule).BeginInit();
            SuspendLayout();
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.White;
            panelHeader.Controls.Add(btnProfile);
            panelHeader.Controls.Add(btnStatistics);
            panelHeader.Controls.Add(btnBerthStatus);
            panelHeader.Controls.Add(btnPendingRequests);
            panelHeader.Controls.Add(btnMyRequests);
            panelHeader.Controls.Add(btnSubmitRequest);
            panelHeader.Controls.Add(btnMyShips);
            panelHeader.Controls.Add(btnAddMyShip);
            panelHeader.Controls.Add(btnBack);
            panelHeader.Controls.Add(btnRefreshData);
            panelHeader.Controls.Add(btnAddShip);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(1400, 80);
            panelHeader.TabIndex = 0;
            // 
            // btnProfile
            // 
            btnProfile.BackColor = Color.FromArgb(96, 125, 139);
            btnProfile.Cursor = Cursors.Hand;
            btnProfile.FlatAppearance.BorderSize = 0;
            btnProfile.FlatStyle = FlatStyle.Flat;
            btnProfile.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnProfile.ForeColor = Color.White;
            btnProfile.Location = new Point(1056, 20);
            btnProfile.Name = "btnProfile";
            btnProfile.Size = new Size(140, 40);
            btnProfile.TabIndex = 11;
            btnProfile.Text = "👤 My Profile";
            btnProfile.UseVisualStyleBackColor = false;
            btnProfile.Click += btnProfile_Click;
            // 
            // btnStatistics
            // 
            btnStatistics.BackColor = Color.FromArgb(142, 68, 173);
            btnStatistics.Cursor = Cursors.Hand;
            btnStatistics.FlatAppearance.BorderSize = 0;
            btnStatistics.FlatStyle = FlatStyle.Flat;
            btnStatistics.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnStatistics.ForeColor = Color.White;
            btnStatistics.Location = new Point(830, 20);
            btnStatistics.Name = "btnStatistics";
            btnStatistics.Size = new Size(140, 40);
            btnStatistics.TabIndex = 10;
            btnStatistics.Text = "📊 Statistics";
            btnStatistics.UseVisualStyleBackColor = false;
            btnStatistics.Click += btnStatistics_Click;
            // 
            // btnBerthStatus
            // 
            btnBerthStatus.BackColor = Color.FromArgb(0, 150, 136);
            btnBerthStatus.Cursor = Cursors.Hand;
            btnBerthStatus.FlatAppearance.BorderSize = 0;
            btnBerthStatus.FlatStyle = FlatStyle.Flat;
            btnBerthStatus.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnBerthStatus.ForeColor = Color.White;
            btnBerthStatus.Location = new Point(670, 20);
            btnBerthStatus.Name = "btnBerthStatus";
            btnBerthStatus.Size = new Size(140, 40);
            btnBerthStatus.TabIndex = 9;
            btnBerthStatus.Text = "⚓ Berth Status";
            btnBerthStatus.UseVisualStyleBackColor = false;
            btnBerthStatus.Click += btnBerthStatus_Click;
            // 
            // btnPendingRequests
            // 
            btnPendingRequests.BackColor = Color.FromArgb(255, 152, 0);
            btnPendingRequests.Cursor = Cursors.Hand;
            btnPendingRequests.FlatAppearance.BorderSize = 0;
            btnPendingRequests.FlatStyle = FlatStyle.Flat;
            btnPendingRequests.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnPendingRequests.ForeColor = Color.White;
            btnPendingRequests.Location = new Point(500, 20);
            btnPendingRequests.Name = "btnPendingRequests";
            btnPendingRequests.Size = new Size(160, 40);
            btnPendingRequests.TabIndex = 8;
            btnPendingRequests.Text = "⏳ Pending Requests";
            btnPendingRequests.UseVisualStyleBackColor = false;
            btnPendingRequests.Click += btnPendingRequests_Click;
            // 
            // btnMyRequests
            // 
            btnMyRequests.BackColor = Color.FromArgb(0, 150, 136);
            btnMyRequests.Cursor = Cursors.Hand;
            btnMyRequests.FlatAppearance.BorderSize = 0;
            btnMyRequests.FlatStyle = FlatStyle.Flat;
            btnMyRequests.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnMyRequests.ForeColor = Color.White;
            btnMyRequests.Location = new Point(910, 20);
            btnMyRequests.Name = "btnMyRequests";
            btnMyRequests.Size = new Size(140, 40);
            btnMyRequests.TabIndex = 7;
            btnMyRequests.Text = "📋 My Requests";
            btnMyRequests.UseVisualStyleBackColor = false;
            btnMyRequests.Click += btnMyRequests_Click;
            // 
            // btnSubmitRequest
            // 
            btnSubmitRequest.BackColor = Color.FromArgb(255, 152, 0);
            btnSubmitRequest.Cursor = Cursors.Hand;
            btnSubmitRequest.FlatAppearance.BorderSize = 0;
            btnSubmitRequest.FlatStyle = FlatStyle.Flat;
            btnSubmitRequest.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSubmitRequest.ForeColor = Color.White;
            btnSubmitRequest.Location = new Point(750, 20);
            btnSubmitRequest.Name = "btnSubmitRequest";
            btnSubmitRequest.Size = new Size(150, 40);
            btnSubmitRequest.TabIndex = 6;
            btnSubmitRequest.Text = "📝 Submit Request";
            btnSubmitRequest.UseVisualStyleBackColor = false;
            btnSubmitRequest.Click += btnSubmitRequest_Click;
            // 
            // btnMyShips
            // 
            btnMyShips.BackColor = Color.FromArgb(156, 39, 176);
            btnMyShips.Cursor = Cursors.Hand;
            btnMyShips.FlatAppearance.BorderSize = 0;
            btnMyShips.FlatStyle = FlatStyle.Flat;
            btnMyShips.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnMyShips.ForeColor = Color.White;
            btnMyShips.Location = new Point(610, 20);
            btnMyShips.Name = "btnMyShips";
            btnMyShips.Size = new Size(130, 40);
            btnMyShips.TabIndex = 5;
            btnMyShips.Text = "📦 My Ships";
            btnMyShips.UseVisualStyleBackColor = false;
            btnMyShips.Click += btnMyShips_Click;
            // 
            // btnAddMyShip
            // 
            btnAddMyShip.BackColor = Color.FromArgb(76, 175, 80);
            btnAddMyShip.Cursor = Cursors.Hand;
            btnAddMyShip.FlatAppearance.BorderSize = 0;
            btnAddMyShip.FlatStyle = FlatStyle.Flat;
            btnAddMyShip.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAddMyShip.ForeColor = Color.White;
            btnAddMyShip.Location = new Point(450, 20);
            btnAddMyShip.Name = "btnAddMyShip";
            btnAddMyShip.Size = new Size(140, 40);
            btnAddMyShip.TabIndex = 4;
            btnAddMyShip.Text = "➕ Add Ship";
            btnAddMyShip.UseVisualStyleBackColor = false;
            btnAddMyShip.Click += btnAddMyShip_Click;
            // 
            // btnBack
            // 
            btnBack.BackColor = Color.FromArgb(158, 158, 158);
            btnBack.Cursor = Cursors.Hand;
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnBack.ForeColor = Color.White;
            btnBack.Location = new Point(1280, 20);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(110, 40);
            btnBack.TabIndex = 3;
            btnBack.Text = "← Kembali";
            btnBack.UseVisualStyleBackColor = false;
            btnBack.Click += btnBack_Click;
            // 
            // btnRefreshData
            // 
            btnRefreshData.BackColor = Color.FromArgb(33, 150, 243);
            btnRefreshData.Cursor = Cursors.Hand;
            btnRefreshData.FlatAppearance.BorderSize = 0;
            btnRefreshData.FlatStyle = FlatStyle.Flat;
            btnRefreshData.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnRefreshData.ForeColor = Color.White;
            btnRefreshData.Location = new Point(1130, 20);
            btnRefreshData.Name = "btnRefreshData";
            btnRefreshData.Size = new Size(140, 40);
            btnRefreshData.TabIndex = 2;
            btnRefreshData.Text = "🔄 Refresh Data";
            btnRefreshData.UseVisualStyleBackColor = false;
            btnRefreshData.Click += btnRefreshData_Click;
            // 
            // btnAddShip
            // 
            btnAddShip.BackColor = Color.FromArgb(76, 175, 80);
            btnAddShip.Cursor = Cursors.Hand;
            btnAddShip.FlatAppearance.BorderSize = 0;
            btnAddShip.FlatStyle = FlatStyle.Flat;
            btnAddShip.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAddShip.ForeColor = Color.White;
            btnAddShip.Location = new Point(750, 20);
            btnAddShip.Name = "btnAddShip";
            btnAddShip.Size = new Size(150, 40);
            btnAddShip.TabIndex = 1;
            btnAddShip.Text = "+ Tambah Kapal";
            btnAddShip.UseVisualStyleBackColor = false;
            btnAddShip.Click += btnAddShip_Click;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblTitle.Location = new Point(20, 25);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(394, 46);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Dashboard Operasional";
            // 
            // cardTotalKapal
            // 
            cardTotalKapal.BackColor = Color.FromArgb(52, 152, 219);
            cardTotalKapal.Controls.Add(lblCardTotalKapalValue);
            cardTotalKapal.Controls.Add(lblCardTotalKapalTitle);
            cardTotalKapal.Location = new Point(30, 110);
            cardTotalKapal.Name = "cardTotalKapal";
            cardTotalKapal.Size = new Size(410, 120);
            cardTotalKapal.TabIndex = 1;
            // 
            // lblCardTotalKapalValue
            // 
            lblCardTotalKapalValue.AutoSize = true;
            lblCardTotalKapalValue.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
            lblCardTotalKapalValue.ForeColor = Color.White;
            lblCardTotalKapalValue.Location = new Point(20, 50);
            lblCardTotalKapalValue.Name = "lblCardTotalKapalValue";
            lblCardTotalKapalValue.Size = new Size(61, 72);
            lblCardTotalKapalValue.TabIndex = 1;
            lblCardTotalKapalValue.Text = "2";
            // 
            // lblCardTotalKapalTitle
            // 
            lblCardTotalKapalTitle.AutoSize = true;
            lblCardTotalKapalTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblCardTotalKapalTitle.ForeColor = Color.White;
            lblCardTotalKapalTitle.Location = new Point(20, 20);
            lblCardTotalKapalTitle.Name = "lblCardTotalKapalTitle";
            lblCardTotalKapalTitle.Size = new Size(141, 32);
            lblCardTotalKapalTitle.TabIndex = 0;
            lblCardTotalKapalTitle.Text = "Total Kapal";
            // 
            // cardSedangBerlabuh
            // 
            cardSedangBerlabuh.BackColor = Color.FromArgb(76, 175, 80);
            cardSedangBerlabuh.Controls.Add(lblCardSedangBerlabuhValue);
            cardSedangBerlabuh.Controls.Add(lblCardSedangBerlabuhTitle);
            cardSedangBerlabuh.Location = new Point(470, 110);
            cardSedangBerlabuh.Name = "cardSedangBerlabuh";
            cardSedangBerlabuh.Size = new Size(410, 120);
            cardSedangBerlabuh.TabIndex = 2;
            // 
            // lblCardSedangBerlabuhValue
            // 
            lblCardSedangBerlabuhValue.AutoSize = true;
            lblCardSedangBerlabuhValue.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
            lblCardSedangBerlabuhValue.ForeColor = Color.White;
            lblCardSedangBerlabuhValue.Location = new Point(20, 50);
            lblCardSedangBerlabuhValue.Name = "lblCardSedangBerlabuhValue";
            lblCardSedangBerlabuhValue.Size = new Size(61, 72);
            lblCardSedangBerlabuhValue.TabIndex = 1;
            lblCardSedangBerlabuhValue.Text = "1";
            // 
            // lblCardSedangBerlabuhTitle
            // 
            lblCardSedangBerlabuhTitle.AutoSize = true;
            lblCardSedangBerlabuhTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblCardSedangBerlabuhTitle.ForeColor = Color.White;
            lblCardSedangBerlabuhTitle.Location = new Point(20, 20);
            lblCardSedangBerlabuhTitle.Name = "lblCardSedangBerlabuhTitle";
            lblCardSedangBerlabuhTitle.Size = new Size(207, 32);
            lblCardSedangBerlabuhTitle.TabIndex = 0;
            lblCardSedangBerlabuhTitle.Text = "Sedang Berlabuh";
            // 
            // cardKapalMenunggu
            // 
            cardKapalMenunggu.BackColor = Color.FromArgb(255, 193, 7);
            cardKapalMenunggu.Controls.Add(lblCardKapalMenungguValue);
            cardKapalMenunggu.Controls.Add(lblCardKapalMenungguTitle);
            cardKapalMenunggu.Location = new Point(910, 110);
            cardKapalMenunggu.Name = "cardKapalMenunggu";
            cardKapalMenunggu.Size = new Size(410, 120);
            cardKapalMenunggu.TabIndex = 3;
            // 
            // lblCardKapalMenungguValue
            // 
            lblCardKapalMenungguValue.AutoSize = true;
            lblCardKapalMenungguValue.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
            lblCardKapalMenungguValue.ForeColor = Color.White;
            lblCardKapalMenungguValue.Location = new Point(20, 50);
            lblCardKapalMenungguValue.Name = "lblCardKapalMenungguValue";
            lblCardKapalMenungguValue.Size = new Size(61, 72);
            lblCardKapalMenungguValue.TabIndex = 1;
            lblCardKapalMenungguValue.Text = "1";
            // 
            // lblCardKapalMenungguTitle
            // 
            lblCardKapalMenungguTitle.AutoSize = true;
            lblCardKapalMenungguTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblCardKapalMenungguTitle.ForeColor = Color.White;
            lblCardKapalMenungguTitle.Location = new Point(20, 20);
            lblCardKapalMenungguTitle.Name = "lblCardKapalMenungguTitle";
            lblCardKapalMenungguTitle.Size = new Size(211, 32);
            lblCardKapalMenungguTitle.TabIndex = 0;
            lblCardKapalMenungguTitle.Text = "Kapal Menunggu";
            // 
            // dgvSchedule
            // 
            dgvSchedule.AllowUserToAddRows = false;
            dgvSchedule.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(245, 247, 250);
            dgvSchedule.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvSchedule.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSchedule.BackgroundColor = Color.White;
            dgvSchedule.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(52, 73, 94);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.Padding = new Padding(5);
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvSchedule.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvSchedule.ColumnHeadersHeight = 40;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Window;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle3.Padding = new Padding(5);
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvSchedule.DefaultCellStyle = dataGridViewCellStyle3;
            dgvSchedule.EnableHeadersVisualStyles = false;
            dgvSchedule.Location = new Point(30, 260);
            dgvSchedule.MultiSelect = false;
            dgvSchedule.Name = "dgvSchedule";
            dgvSchedule.ReadOnly = true;
            dgvSchedule.RowHeadersVisible = false;
            dgvSchedule.RowHeadersWidth = 51;
            dgvSchedule.RowTemplate.Height = 35;
            dgvSchedule.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSchedule.Size = new Size(1340, 400);
            dgvSchedule.TabIndex = 4;
            dgvSchedule.CellContentClick += dgvSchedule_CellContentClick;
            // 
            // comboShips
            // 
            comboShips.FormattingEnabled = true;
            comboShips.Location = new Point(12, 669);
            comboShips.Name = "comboShips";
            comboShips.Size = new Size(10, 28);
            comboShips.TabIndex = 5;
            comboShips.Visible = false;
            // 
            // dtpETA
            // 
            dtpETA.Location = new Point(28, 669);
            dtpETA.Name = "dtpETA";
            dtpETA.Size = new Size(10, 27);
            dtpETA.TabIndex = 6;
            dtpETA.Visible = false;
            // 
            // dtpETD
            // 
            dtpETD.Location = new Point(44, 669);
            dtpETD.Name = "dtpETD";
            dtpETD.Size = new Size(10, 27);
            dtpETD.TabIndex = 7;
            dtpETD.Visible = false;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(1400, 700);
            Controls.Add(dtpETD);
            Controls.Add(dtpETA);
            Controls.Add(comboShips);
            Controls.Add(dgvSchedule);
            Controls.Add(cardKapalMenunggu);
            Controls.Add(cardSedangBerlabuh);
            Controls.Add(cardTotalKapal);
            Controls.Add(panelHeader);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainWindow";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "HarborMaster - Dashboard";
            Load += MainWindow_Load;
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            cardTotalKapal.ResumeLayout(false);
            cardTotalKapal.PerformLayout();
            cardSedangBerlabuh.ResumeLayout(false);
            cardSedangBerlabuh.PerformLayout();
            cardKapalMenunggu.ResumeLayout(false);
            cardKapalMenunggu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSchedule).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnAddShip;
        private System.Windows.Forms.Button btnRefreshData;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnAddMyShip;
        private System.Windows.Forms.Button btnMyShips;
        private System.Windows.Forms.Button btnSubmitRequest;
        private System.Windows.Forms.Button btnMyRequests;
        private System.Windows.Forms.Button btnPendingRequests;
        private System.Windows.Forms.Button btnBerthStatus;
        private System.Windows.Forms.Button btnStatistics;
        private System.Windows.Forms.Button btnProfile;
        private System.Windows.Forms.Panel cardTotalKapal;
        private System.Windows.Forms.Label lblCardTotalKapalValue;
        private System.Windows.Forms.Label lblCardTotalKapalTitle;
        private System.Windows.Forms.Panel cardSedangBerlabuh;
        private System.Windows.Forms.Label lblCardSedangBerlabuhValue;
        private System.Windows.Forms.Label lblCardSedangBerlabuhTitle;
        private System.Windows.Forms.Panel cardKapalMenunggu;
        private System.Windows.Forms.Label lblCardKapalMenungguValue;
        private System.Windows.Forms.Label lblCardKapalMenungguTitle;
        private System.Windows.Forms.DataGridView dgvSchedule;
        private System.Windows.Forms.ComboBox comboShips;
        private System.Windows.Forms.DateTimePicker dtpETA;
        private System.Windows.Forms.DateTimePicker dtpETD;
    }
}