using System;
using System.Linq;
using System.Windows.Forms;

namespace HarborMaster
{
    public partial class MainForm : Form
    {
    private PortService _portService = new PortService();
  
        // UI Controls
private Label lblTitle;
        private Button btnNewShip;
        private Button btnRefresh;
    private Button btnDemo; // New demo button
   private Label lblScheduleTitle;
 private DataGridView dgvSchedule;
        
        public MainForm()
        {
            InitializeComponent();
  RefreshSchedule();
        }
    
        private void InitializeComponent()
        {
   this.SuspendLayout();
            
        // Form properties
       this.Text = "HarborMaster - Monitor Jadwal Pelabuhan";
            this.Size = new System.Drawing.Size(1200, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
          this.MinimumSize = new System.Drawing.Size(800, 500);

          // Title Label
      this.lblTitle = new Label();
this.lblTitle.Text = "DASHBOARD OPERASIONAL";
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold);
        this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(0, 122, 204);
       this.lblTitle.Location = new System.Drawing.Point(15, 15);
   this.lblTitle.Size = new System.Drawing.Size(400, 35);
            this.Controls.Add(this.lblTitle);
            
      // New Ship Button
            this.btnNewShip = new Button();
      this.btnNewShip.Text = "Input Kapal Baru";
          this.btnNewShip.Location = new System.Drawing.Point(450, 20);
      this.btnNewShip.Size = new System.Drawing.Size(150, 30);
            this.btnNewShip.UseVisualStyleBackColor = true;
      this.btnNewShip.Click += new EventHandler(this.OpenArrivalWindow_Click);
            this.Controls.Add(this.btnNewShip);
       
       // Refresh Button
   this.btnRefresh = new Button();
            this.btnRefresh.Text = "Refresh Jadwal";
            this.btnRefresh.Location = new System.Drawing.Point(610, 20);
      this.btnRefresh.Size = new System.Drawing.Size(150, 30);
    this.btnRefresh.UseVisualStyleBackColor = true;
    this.btnRefresh.Click += new EventHandler(this.RefreshSchedule_Click);
            this.Controls.Add(this.btnRefresh);
            
      // Demo Button
            this.btnDemo = new Button();
   this.btnDemo.Text = "OOP Demos";
         this.btnDemo.Location = new System.Drawing.Point(770, 20);
            this.btnDemo.Size = new System.Drawing.Size(100, 30);
            this.btnDemo.UseVisualStyleBackColor = true;
    this.btnDemo.Click += new EventHandler(this.OpenDemo_Click);
     this.Controls.Add(this.btnDemo);
          
 // Schedule Title Label
  this.lblScheduleTitle = new Label();
      this.lblScheduleTitle.Text = "JADWAL ALOKASI DERMAGA AKTIF";
      this.lblScheduleTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblScheduleTitle.Location = new System.Drawing.Point(15, 65);
    this.lblScheduleTitle.Size = new System.Drawing.Size(400, 25);
            this.Controls.Add(this.lblScheduleTitle);
        
 // DataGridView
            this.dgvSchedule = new DataGridView();
            this.dgvSchedule.Location = new System.Drawing.Point(15, 100);
      this.dgvSchedule.Size = new System.Drawing.Size(1155, 500);
     this.dgvSchedule.AutoGenerateColumns = false;
    this.dgvSchedule.ReadOnly = true;
       this.dgvSchedule.AllowUserToAddRows = false;
            this.dgvSchedule.AllowUserToDeleteRows = false;
            this.dgvSchedule.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvSchedule.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    
            // Add columns to DataGridView
       this.dgvSchedule.Columns.Add(new DataGridViewTextBoxColumn()
   {
       Name = "AssignmentID",
    HeaderText = "ID Tugas",
  DataPropertyName = "AssignmentID",
          Width = 100
            });
            
        this.dgvSchedule.Columns.Add(new DataGridViewTextBoxColumn()
   {
                Name = "BerthName",
                HeaderText = "Nama Dermaga",
   DataPropertyName = "BerthName",
 Width = 150
            });
     
            this.dgvSchedule.Columns.Add(new DataGridViewTextBoxColumn()
      {
     Name = "ShipName",
           HeaderText = "Nama Kapal",
      DataPropertyName = "ShipName",
     Width = 200
            });
        
     this.dgvSchedule.Columns.Add(new DataGridViewTextBoxColumn()
            {
    Name = "ShipType",
 HeaderText = "Jenis Kapal",
           DataPropertyName = "ShipType",
         Width = 150
      });
   
this.dgvSchedule.Columns.Add(new DataGridViewTextBoxColumn()
       {
  Name = "ShipTonnage",
     HeaderText = "Tonnage (ton)",
    DataPropertyName = "ShipTonnage",
           Width = 150
       });
      
  this.dgvSchedule.Columns.Add(new DataGridViewTextBoxColumn()
            {
         Name = "ETA",
        HeaderText = "ETA",
         DataPropertyName = "ETA",
     Width = 200
  });
     
   this.dgvSchedule.Columns.Add(new DataGridViewTextBoxColumn()
    {
           Name = "ETD",
                HeaderText = "ETD",
                DataPropertyName = "ETD",
     Width = 200
          });
            
     this.dgvSchedule.Columns.Add(new DataGridViewTextBoxColumn()
 {
         Name = "Status",
  HeaderText = "Status",
        DataPropertyName = "Status",
      Width = 150
            });
            
         this.Controls.Add(this.dgvSchedule);
  
            this.ResumeLayout(false);
        }
        
  private void OpenArrivalWindow_Click(object sender, EventArgs e)
        {
       ArrivalForm arrivalForm = new ArrivalForm();
   if (arrivalForm.ShowDialog() == DialogResult.OK)
            {
     RefreshSchedule();
       }
 }
        
        private void RefreshSchedule_Click(object sender, EventArgs e)
        {
    RefreshSchedule();
}
        
        private void OpenDemo_Click(object sender, EventArgs e)
        {
DemoForm demoForm = new DemoForm();
            demoForm.ShowDialog();
        }
     
      private void RefreshSchedule()
        {
            try
            {
       var currentSchedule = _portService.GetCurrentAssignments()
     .Select(assignment => new
   {
          AssignmentID = assignment.AssignmentID,
      BerthName = assignment.Berth?.Id ?? "N/A",
 ShipName = assignment.Ship?.Name ?? "N/A",
  ShipType = assignment.Ship?.Type ?? "N/A",
      ShipTonnage = assignment.Ship?.Tonnage.ToString("F0") ?? "N/A",
    ETA = assignment.ETA.ToString("dd/MM/yyyy HH:mm"),
            ETD = assignment.ETD.ToString("dd/MM/yyyy HH:mm"),
          Status = assignment.Status ?? "N/A"
        }).ToList();
     
    dgvSchedule.DataSource = currentSchedule;
       }
        catch (Exception ex)
   {
          MessageBox.Show($"Gagal memuat jadwal: {ex.Message}", "Kesalahan Data", 
        MessageBoxButtons.OK, MessageBoxIcon.Error);
   }
      }
    }
}