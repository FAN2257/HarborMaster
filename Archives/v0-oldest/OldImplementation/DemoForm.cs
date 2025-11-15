using System;
using System.Windows.Forms;

namespace HarborMaster
{
    public partial class DemoForm : Form
    {
        private HarborOperationsDemo _demo;
        private TextBox txtOutput;
    private Button btnRunDemos;
        private Button btnDockingDemo;
      private Button btnServiceDemo;
   private Button btnEncapsulationDemo;
      private Button btnClose;
        
     public DemoForm()
        {
         _demo = new HarborOperationsDemo();
            InitializeComponent();
 }
        
        private void InitializeComponent()
      {
 this.SuspendLayout();
          
        // Form properties
            this.Text = "HarborMaster - OOP Demonstrations";
      this.Size = new System.Drawing.Size(900, 700);
  this.StartPosition = FormStartPosition.CenterParent;
     this.FormBorderStyle = FormBorderStyle.Sizable;
      
         // Output TextBox
   this.txtOutput = new TextBox();
     this.txtOutput.Location = new System.Drawing.Point(20, 20);
     this.txtOutput.Size = new System.Drawing.Size(840, 500);
     this.txtOutput.Multiline = true;
     this.txtOutput.ScrollBars = ScrollBars.Vertical;
  this.txtOutput.ReadOnly = true;
      this.txtOutput.Font = new System.Drawing.Font("Courier New", 9F);
 this.txtOutput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    this.Controls.Add(this.txtOutput);
        
        // Buttons
    int buttonY = 540;
        int buttonSpacing = 10;
            int currentX = 20;

       this.btnRunDemos = new Button();
  this.btnRunDemos.Text = "Run All Demos";
            this.btnRunDemos.Location = new System.Drawing.Point(currentX, buttonY);
            this.btnRunDemos.Size = new System.Drawing.Size(120, 30);
            this.btnRunDemos.Click += BtnRunDemos_Click;
     this.Controls.Add(this.btnRunDemos);
currentX += 120 + buttonSpacing;
            
   this.btnDockingDemo = new Button();
    this.btnDockingDemo.Text = "Polymorphism Demo";
      this.btnDockingDemo.Location = new System.Drawing.Point(currentX, buttonY);
        this.btnDockingDemo.Size = new System.Drawing.Size(140, 30);
      this.btnDockingDemo.Click += BtnDockingDemo_Click;
      this.Controls.Add(this.btnDockingDemo);
   currentX += 140 + buttonSpacing;
     
      this.btnServiceDemo = new Button();
this.btnServiceDemo.Text = "Service Demo";
    this.btnServiceDemo.Location = new System.Drawing.Point(currentX, buttonY);
      this.btnServiceDemo.Size = new System.Drawing.Size(100, 30);
   this.btnServiceDemo.Click += BtnServiceDemo_Click;
     this.Controls.Add(this.btnServiceDemo);
      currentX += 100 + buttonSpacing;
     
            this.btnEncapsulationDemo = new Button();
     this.btnEncapsulationDemo.Text = "Encapsulation Demo";
    this.btnEncapsulationDemo.Location = new System.Drawing.Point(currentX, buttonY);
            this.btnEncapsulationDemo.Size = new System.Drawing.Size(140, 30);
            this.btnEncapsulationDemo.Click += BtnEncapsulationDemo_Click;
         this.Controls.Add(this.btnEncapsulationDemo);
     currentX += 140 + buttonSpacing;
        
   this.btnClose = new Button();
            this.btnClose.Text = "Close";
       this.btnClose.Location = new System.Drawing.Point(currentX, buttonY);
      this.btnClose.Size = new System.Drawing.Size(80, 30);
          this.btnClose.Click += (s, e) => this.Close();
    this.Controls.Add(this.btnClose);
            
      // Anchor buttons to bottom
            this.btnRunDemos.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnDockingDemo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
   this.btnServiceDemo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
         this.btnEncapsulationDemo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
     
   this.ResumeLayout(false);
       }
        
      private void BtnRunDemos_Click(object sender, EventArgs e)
        {
      try
   {
     txtOutput.Text = "Running all OOP demonstrations...\r\n\r\n";
          txtOutput.Text += _demo.RunAllDemos();
       txtOutput.Text += "\r\n\r\n=== All Demonstrations Completed ===";
     }
catch (Exception ex)
        {
     txtOutput.Text += $"\r\n\r\nError: {ex.Message}";
      }
   }
      
      private void BtnDockingDemo_Click(object sender, EventArgs e)
        {
      try
  {
  var originalOut = Console.Out;
        using (var writer = new System.IO.StringWriter())
        {
     Console.SetOut(writer);
     _demo.DemonstrateDockingFeeCalculation();
     Console.SetOut(originalOut);
       txtOutput.Text = writer.ToString();
    }
            }
       catch (Exception ex)
   {
      txtOutput.Text = $"Error: {ex.Message}";
       }
      }
        
   private void BtnServiceDemo_Click(object sender, EventArgs e)
   {
 try
      {
    var originalOut = Console.Out;
            using (var writer = new System.IO.StringWriter())
   {
     Console.SetOut(writer);
    _demo.DemonstrateServiceManagement();
        Console.SetOut(originalOut);
     txtOutput.Text = writer.ToString();
       }
   }
 catch (Exception ex)
     {
     txtOutput.Text = $"Error: {ex.Message}";
     }
    }
        
   private void BtnEncapsulationDemo_Click(object sender, EventArgs e)
  {
       try
    {
       var originalOut = Console.Out;
       using (var writer = new System.IO.StringWriter())
  {
 Console.SetOut(writer);
   _demo.DemonstrateEncapsulation();
   Console.SetOut(originalOut);
    txtOutput.Text = writer.ToString();
    }
    }
       catch (Exception ex)
     {
     txtOutput.Text = $"Error: {ex.Message}";
      }
   }
    }
}