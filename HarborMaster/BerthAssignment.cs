using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarborMaster
{
    public class BerthAssignment
    {
        public string AssignmentID { get; set; }
        public DateTime AssignmentDate { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }
        public string Status { get; set; }

        public Ship Ship { get; set; }
        public Berth Berth { get; set; }
        public DateTime ETA { get; set; }
        public DateTime ETD { get; set; }
        public string status { get; set; }
        public HarborUser HarborUser { get; set; }
        public List<PortService> PortService { get; set; } = new List<PortService>();

        public void Schedule(Ship ship, Berth berth, DateTime arrival, DateTime departure)
        {
            Ship = ship;
            Berth = berth;
            ArrivalTime = arrival;
            DepartureTime = departure;
            Status = "Scheduled";
        }

        public void CompleteAssignment()
        {
            // Cek apakah BerthAssignment ini memiliki properti Ship dan Berth
            if (this.Ship == null || this.Berth == null)
            {
                throw new InvalidOperationException("Assignment tidak lengkap. Tidak ada kapal atau dermaga yang ditugaskan.");
            }

            // 1. Perbarui Status Tugas ini
            Status = "Completed";

            // 2. MINTA SERVICE UNTUK MELEPAS DERMAGA DAN KAPAL
            // Kita harus membuat instance PortService di sini atau melewatkannya sebagai parameter.

            PortService service = new PortService(); // PANGGIL SERVICE LAYER

            // Panggil fungsi pelepasan di PortService
            service.ReleaseBerth(this.Berth, this.Ship);
        }
        public string GetAssignmentInfo()
        {
            return $"AssignmentID: {AssignmentID}, Ship: {Ship?.Name ?? "N/A"}, Berth: {Berth?.Id ?? "N/A"}, Arrival: {ArrivalTime}, Departure: {DepartureTime}, Status: {Status}";
        }
    }
}
