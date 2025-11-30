using System;
using System.Globalization;
using System.Windows.Forms;

namespace HarborMaster.Views
{
    public partial class EditInvoiceDialog : Form
    {
        public decimal NewAmount { get; private set; }
        public string AdjustmentNotes { get; private set; }

        public EditInvoiceDialog(decimal currentAmount)
        {
            InitializeComponent();
            txtAmount.Text = currentAmount.ToString("F2", CultureInfo.InvariantCulture);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtAmount.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal newAmount))
            {
                if (newAmount < 0)
                {
                    MessageBox.Show("Amount cannot be negative.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                NewAmount = newAmount;
                AdjustmentNotes = txtNotes.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid decimal amount.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
