using HarborMaster.Services;
using HarborMaster.Views.Interfaces;
using System;
using System.Threading.Tasks;

namespace HarborMaster.Presenters
{
    public class ShipArrivalPresenter
    {
        private readonly IAddShipDialog _view;
        private readonly PortService _portService;

        public ShipArrivalPresenter(IAddShipDialog view)
        {
            _view = view;
            _portService = new PortService();
        }

        /// <summary>
        /// Process ship arrival registration
        /// Called when user clicks "ALOKASI DERMAGA" button
        /// </summary>
        public async Task ProcessArrivalAsync()
        {
            try
            {
                // Set loading state
                _view.IsLoading = true;
                _view.ErrorMessage = "";
                _view.SuccessMessage = "";

                // Get data from view
                string shipName = _view.ShipName;
                string imoNumber = _view.ImoNumber;
                decimal shipLength = _view.ShipLength;
                decimal shipDraft = _view.ShipDraft;
                string shipType = _view.ShipType;
                DateTime eta = _view.ETA;
                DateTime etd = _view.ETD;

                // DEBUG: Show parsed values
                System.Windows.Forms.MessageBox.Show(
                    $"DEBUG INFO:\n\n" +
                    $"Nama: {shipName}\n" +
                    $"Panjang (decimal): {shipLength}\n" +
                    $"Panjang (double): {(double)shipLength}\n" +
                    $"Draft (decimal): {shipDraft}\n" +
                    $"Draft (double): {(double)shipDraft}\n" +
                    $"Tipe: {shipType}",
                    "Debug Parsing",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Information
                );

                // Additional validation for parsed values
                if (shipLength <= 0)
                {
                    _view.ErrorMessage = "Panjang kapal tidak valid. Gunakan angka (contoh: 180 atau 180.5)";
                    return;
                }

                if (shipDraft <= 0)
                {
                    _view.ErrorMessage = "Draft kapal tidak valid. Gunakan angka (contoh: 10 atau 10.5)";
                    return;
                }

                // Get owner ID if available (for Ship Owner role)
                int? ownerId = _view.OwnerId;

                // Call service to process arrival
                var result = await _portService.ProcessShipArrival(
                    shipName,
                    imoNumber,
                    (double)shipLength,
                    (double)shipDraft,
                    shipType,
                    eta,
                    etd,
                    ownerId
                );

                if (result.IsSuccess)
                {
                    // Success!
                    string detailMessage = result.IsNewShip
                        ? $"Kapal baru '{shipName}' berhasil didaftarkan dan dialokasikan ke {result.AllocatedBerth?.BerthName}."
                        : $"Kapal '{shipName}' (terdaftar) berhasil dialokasikan ke {result.AllocatedBerth?.BerthName}.";

                    _view.HandleSuccess(detailMessage);
                }
                else
                {
                    // Failed
                    _view.ErrorMessage = result.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                _view.ErrorMessage = $"Terjadi kesalahan: {ex.Message}";
            }
            finally
            {
                _view.IsLoading = false;
            }
        }
    }
}
