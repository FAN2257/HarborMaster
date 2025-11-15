using HarborMaster.Models; // Kita perlu ini untuk menampilkan data
using System.Collections.Generic;

namespace HarborMaster.Views.Interfaces
{
    public interface IMainView
    {
        // Properti 'set' untuk menampilkan data
        // Presenter akan mengisi daftar ini dari PortService
        void SetScheduleDataSource(List<BerthAssignment> schedule);
        void SetBerthDataSource(List<Berth> berths);

        // Properti 'get' untuk input dari form (misal: form alokasi)
        int SelectedShipId { get; }
        System.DateTime SelectedETA { get; }
        System.DateTime SelectedETD { get; }

        // Metode kontrol
        void ShowMessage(string message);
        void SetCurrentUser(string fullName, bool canOverride);
    }
}