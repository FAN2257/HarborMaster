using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarborMaster
{
    public class Schedule
    {
        public string ScheduleID { get; set; }
        public DateTime Date { get; set; }
        public string Remarks { get; set; }

        private List<BerthAssignment> _assignments = new List<BerthAssignment>();

        public void AddAssignment(BerthAssignment assignment)
        {
            _assignments.Add(assignment);
        }

        public void RemoveAssignment(string assignmentID)
        {
            _assignments = _assignments.Where(a => a.AssignmentID != assignmentID).ToList();
        }

        public List<BerthAssignment> ListAssignments()
        {
            return _assignments;
        }
    }
}
