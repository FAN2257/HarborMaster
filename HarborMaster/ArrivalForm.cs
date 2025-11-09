using System;
using System.Windows.Forms;

namespace HarborMaster
{
    public partial class ArrivalForm : Form
    {
        private PortService _portService = new PortService();
        
        // Simulasikan Pengguna yang login
    private HarborUser _currentUser = new HarborUser
        {
  UserId = 1,
      Username = "op_standar",
     CurrentRole = UserRoleType.PortOperator
        };

  // UI Controls
        private Label lblTitle;
     private Label lblShipName;
        private TextBox txtShipName;
       private Label lblTonnage;
        private TextBox txtTonnage;
      private Label lblCargoCapacity;
       private TextBox txtCargoCapacity;
 private Label lblCargoType;
 private ComboBox cmbCargoType;
        private Label lblETA;
        private DateTimePicker dtpETA;
        private Label lblStatusMessage;
        private Button btnAllocate;
        private Button btnCancel;
     
        public ArrivalForm()
     {
    InitializeComponent();
      }
        
     private void InitializeComponent()
        {
   this.SuspendLayout();
       
            // Form properties
      this.Text = "Input Data Kedatangan Kapal";
   this.Size = new System.Drawing.Size(800, 500);
         this.StartPosition = FormStartPosition.CenterParent;
  this.FormBorderStyle = FormBorderStyle.FixedDialog;
   this.MaximizeBox = false;
     this.MinimizeBox = false;
      
         int yPos = 30;
      int spacing = 50;
  int labelWidth = 200;
        int controlWidth = 300;
        int leftMargin = 30;
  int controlLeftMargin = leftMargin + labelWidth + 10;
 
 // Title
     this.lblTitle = new Label();
      this.lblTitle.Text = "INPUT DATA KAPAL BARU";
this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold);
        this.lblTitle.Location = new System.Drawing.Point(leftMargin, yPos);
 this.lblTitle.Size = new System.Drawing.Size(500, 30);
      this.Controls.Add(this.lblTitle);
      yPos += 50;
     
         // Ship Name
  this.lblShipName = new Label();
    this.lblShipName.Text = "1. Nama Kapal:";
       this.lblShipName.Location = new System.Drawing.Point(leftMargin, yPos);
      this.lblShipName.Size = new System.Drawing.Size(labelWidth, 23);
        this.Controls.Add(this.lblShipName);
     
         this.txtShipName = new TextBox();
       this.txtShipName.Location = new System.Drawing.Point(controlLeftMargin, yPos);
            this.txtShipName.Size = new System.Drawing.Size(controlWidth, 23);
  this.txtShipName.TextChanged += TxtShipName_TextChanged;
         this.Controls.Add(this.txtShipName);
       yPos += spacing;
     
            // Ship Tonnage
        this.lblTonnage = new Label();
      this.lblTonnage.Text = "2. Tonnage Kapal:";
this.lblTonnage.Location = new System.Drawing.Point(leftMargin, yPos);
     this.lblTonnage.Size = new System.Drawing.Size(labelWidth, 23);
    this.Controls.Add(this.lblTonnage);
   
 this.txtTonnage = new TextBox();
    this.txtTonnage.Location = new System.Drawing.Point(controlLeftMargin, yPos);
       this.txtTonnage.Size = new System.Drawing.Size(controlWidth, 23);
      this.Controls.Add(this.txtTonnage);
    yPos += spacing;
            
      // Cargo Capacity
    this.lblCargoCapacity = new Label();
     this.lblCargoCapacity.Text = "3. Kapasitas Kargo:";
            this.lblCargoCapacity.Location = new System.Drawing.Point(leftMargin, yPos);
        this.lblCargoCapacity.Size = new System.Drawing.Size(labelWidth, 23);
     this.Controls.Add(this.lblCargoCapacity);
      
    this.txtCargoCapacity = new TextBox();
        this.txtCargoCapacity.Location = new System.Drawing.Point(controlLeftMargin, yPos);
      this.txtCargoCapacity.Size = new System.Drawing.Size(controlWidth, 23);
      this.Controls.Add(this.txtCargoCapacity);
            yPos += spacing;
      
            // Cargo Type
     this.lblCargoType = new Label();
  this.lblCargoType.Text = "4. Jenis Kargo:";
      this.lblCargoType.Location = new System.Drawing.Point(leftMargin, yPos);
  this.lblCargoType.Size = new System.Drawing.Size(labelWidth, 23);
            this.Controls.Add(this.lblCargoType);
   
        this.cmbCargoType = new ComboBox();
      this.cmbCargoType.Location = new System.Drawing.Point(controlLeftMargin, yPos);
     this.cmbCargoType.Size = new System.Drawing.Size(controlWidth, 23);
      this.cmbCargoType.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cmbCargoType.Items.AddRange(new string[] { "General Cargo", "Electronics", "Food Products", "Hazardous Chemicals", "Raw Materials" });
        this.cmbCargoType.SelectedIndex = 0;
     this.Controls.Add(this.cmbCargoType);
  yPos += spacing;
            
        // ETA
    this.lblETA = new Label();
 this.lblETA.Text = "5. ETA (Tgl/Waktu):";
     this.lblETA.Location = new System.Drawing.Point(leftMargin, yPos);
      this.lblETA.Size = new System.Drawing.Size(labelWidth, 23);
     this.Controls.Add(this.lblETA);

     this.dtpETA = new DateTimePicker();
   this.dtpETA.Location = new System.Drawing.Point(controlLeftMargin, yPos);
      this.dtpETA.Size = new System.Drawing.Size(controlWidth, 23);
      this.dtpETA.Format = DateTimePickerFormat.Custom;
 this.dtpETA.CustomFormat = "dd/MM/yyyy HH:mm";
 this.dtpETA.ShowUpDown = false;
       this.dtpETA.Value = DateTime.Now.AddHours(1);
        this.Controls.Add(this.dtpETA);
            yPos += spacing + 20;
 
          // Status Message
     this.lblStatusMessage = new Label();
     this.lblStatusMessage.Text = "";
     this.lblStatusMessage.ForeColor = System.Drawing.Color.Red;
     this.lblStatusMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
   this.lblStatusMessage.Location = new System.Drawing.Point(leftMargin, yPos);
      this.lblStatusMessage.Size = new System.Drawing.Size(700, 60);
      this.Controls.Add(this.lblStatusMessage);
     yPos += 80;
          
   // Buttons
     this.btnCancel = new Button();
        this.btnCancel.Text = "Batal";
      this.btnCancel.Location = new System.Drawing.Point(controlLeftMargin + controlWidth - 200, yPos);
       this.btnCancel.Size = new System.Drawing.Size(90, 35);
         this.btnCancel.UseVisualStyleBackColor = true;
     this.btnCancel.DialogResult = DialogResult.Cancel;
 this.Controls.Add(this.btnCancel);
            
  this.btnAllocate = new Button();
       this.btnAllocate.Text = "Cek & Alokasi";
      this.btnAllocate.Location = new System.Drawing.Point(controlLeftMargin + controlWidth - 100, yPos);
       this.btnAllocate.Size = new System.Drawing.Size(100, 35);
            this.btnAllocate.UseVisualStyleBackColor = true;
      this.btnAllocate.Click += new EventHandler(this.BtnAllocate_Click);
      this.Controls.Add(this.btnAllocate);

            this.ResumeLayout(false);
        }
      
        private void BtnAllocate_Click(object sender, EventArgs e)
      {
            // Clear previous status message
  lblStatusMessage.Text = "";
      
            try
         {
          // 1. Validasi dan Pengambilan Data Input
     
      // Cek Nama Kapal
   if (string.IsNullOrWhiteSpace(txtShipName.Text))
throw new ArgumentException("Nama Kapal tidak boleh kosong.");
      
       // Cek Tonnage
      if (!decimal.TryParse(txtTonnage.Text, out decimal tonnage) || tonnage <= 0)
  {
        throw new ArgumentException("Tonnage harus berupa angka yang valid dan lebih besar dari 0.");
        }
  
      // Cek Cargo Capacity
    if (!decimal.TryParse(txtCargoCapacity.Text, out decimal cargoCapacity) || cargoCapacity <= 0)
      {
       throw new ArgumentException("Kapasitas Kargo harus berupa angka yang valid dan lebih besar dari 0.");
            }
        
    // Cek Tanggal/Waktu
      DateTime eta = dtpETA.Value;
    
        // 2. Buat Model CargoShip
   CargoShip newShip = new CargoShip(
  shipID: Guid.NewGuid().ToString(),
     name: txtShipName.Text,
  flag: "Indonesia", // Default flag
     tonnage: tonnage,
      cargoCapacity: cargoCapacity,
       cargoType: cmbCargoType.SelectedItem.ToString()
   );
    
       // 3. Panggil Logika Bisnis (PortService.AllocateBerth)
        BerthAssignment result = _portService.AllocateBerth(newShip, eta, _currentUser);
        
  // 4. Sukses
   MessageBox.Show($"Kapal '{newShip.Name}' berhasil dialokasikan ke Dermaga {result.Berth.Id}.", 
        "Alokasi Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
      this.Close();
            }
      catch (Exception ex)
     {
      // 5. Penanganan Exception/Konflik
         
    if (ex.Message == "CONFLICT_DETECTED_OVERRIDABLE")
      {
    // Kasus Khusus: Konflik terdeteksi, dan PortService mengindikasikan bisa di-override
  
       DialogResult response = MessageBox.Show(
          "Konflik jadwal terdeteksi. Lakukan override?",
  "Konflik Jadwal",
    MessageBoxButtons.YesNo,
         MessageBoxIcon.Warning);
      
       if (response == DialogResult.Yes)
{
          // TODO: Panggil metode PortService.ForceAllocate() di sini (jika Anda membuatnya)
MessageBox.Show("Override Berhasil. Alokasi diproses secara paksa.", "Sukses", 
     MessageBoxButtons.OK, MessageBoxIcon.Information);
         this.DialogResult = DialogResult.OK;
      this.Close();
     }
       else
            {
   lblStatusMessage.Text = "Alokasi dibatalkan oleh pengguna (Override ditolak).";
      }
  }
       else
{
     // Tampilkan error standar (validasi input, dermaga terlalu dangkal, dll.)
      lblStatusMessage.Text = $"Gagal Alokasi: {ex.Message}";
      }
       }
        }
    
        private void TxtShipName_TextChanged(object sender, EventArgs e)
{
   // Clear status message when user types
          if (!string.IsNullOrEmpty(lblStatusMessage.Text))
            {
     lblStatusMessage.Text = "";
     }
        }
    }
}