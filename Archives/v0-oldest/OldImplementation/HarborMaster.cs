using System;
using System.Collections.Generic;
using System.Linq;

namespace HarborMaster
{
    /// <summary>
  /// Harbor Master Controller - Facade untuk operasi pelabuhan tingkat tinggi
        /// Bertindak sebagai interface antara UI dan business logic di PortService
        /// </summary>
    public class HarborMasterController
        {
   private readonly PortService _portService;
      private readonly HarborUser _currentUser;

        public string HarborMasterID { get; set; }
       public string Name { get; set; }
        public string Contact { get; set; }
        public string Role { get; set; } // Admin, Controller

        /// <summary>
   /// Constructor dengan user context
     /// </summary>
     public HarborMasterController(HarborUser currentUser)
       {
       _portService = new PortService();
            _currentUser = currentUser;
    
     HarborMasterID = currentUser.UserId.ToString();
       Name = currentUser.Username;
  Contact = currentUser.Contact;
      Role = currentUser.CurrentRole.ToString();
        }

   /// <summary>
    /// Constructor default (untuk backward compatibility)
     /// </summary>
  public HarborMasterController()
        {
     _portService = new PortService();
      _currentUser = new HarborUser
     {
     UserId = 1,
          Username = "Default HarborMaster",
       CurrentRole = UserRoleType.HarborMaster
      };
        }

 /// <summary>
      /// Create berth assignment (wrapper untuk PortService.CreateAssignment)
 /// </summary>
 public BerthAssignment CreateAssignment(Ship ship, Berth berth, DateTime arrival, DateTime departure)
        {
            return _portService.CreateAssignment(ship, berth, arrival, departure, _currentUser);
     }

/// <summary>
        /// Allocate berth automatically (wrapper untuk PortService.AllocateBerth)
        /// </summary>
   public BerthAssignment AllocateBerth(Ship ship, DateTime eta)
        {
      return _portService.AllocateBerth(ship, eta, _currentUser);
        }

     /// <summary>
        /// Force allocate berth (HarborMaster privilege)
        /// </summary>
        public BerthAssignment ForceAllocateBerth(Ship ship, DateTime eta, Berth berth)
 {
          if (_currentUser.CurrentRole != UserRoleType.HarborMaster)
        {
 throw new UnauthorizedAccessException("Only HarborMaster can force allocate berths.");
            }
  
     return _portService.ForceAllocateBerth(ship, eta, berth);
   }

      /// <summary>
        /// Monitor traffic for all ships
    /// </summary>
  public void MonitorTraffic(List<Ship> ships)
   {
        _portService.MonitorTraffic(ships);
        }

        /// <summary>
    /// Generate comprehensive harbor report
  /// </summary>
    public string GenerateReport(List<BerthAssignment> assignments = null)
     {
       return _portService.GenerateReport(assignments);
        }

        /// <summary>
   /// Generate report for current assignments only
      /// </summary>
 public string GenerateCurrentReport()
        {
      var currentAssignments = _portService.GetCurrentAssignments().ToList();
       return GenerateReport(currentAssignments);
  }

  /// <summary>
    /// Release berth (complete assignment)
        /// </summary>
    public void ReleaseBerth(Berth berth, Ship ship)
        {
      _portService.ReleaseBerth(berth, ship);
   }

  /// <summary>
        /// Get dashboard data for UI
        /// </summary>
        public DashboardData GetDashboardData()
        {
return new DashboardData
        {
   CurrentAssignments = _portService.GetCurrentAssignments().ToList(),
  AvailableBerths = _portService.GetAvailableBerths().ToList(),
TotalBerths = _portService.GetAllBerths().Count(),
        ActiveAssignments = _portService.GetCurrentAssignments().Count()
            };
        }

        /// <summary>
/// Check user permissions for specific operations
        /// </summary>
        public bool CanOverrideConflicts()
     {
     return _currentUser.CurrentRole == UserRoleType.HarborMaster;
        }

      /// <summary>
        /// Get current user info
   /// </summary>
public HarborUser GetCurrentUser()
        {
 return _currentUser;
     }
    }

    /// <summary>
    /// Data structure for dashboard information
      /// </summary>
     public class DashboardData
    {
       public List<BerthAssignment> CurrentAssignments { get; set; } = new List<BerthAssignment>();
        public List<Berth> AvailableBerths { get; set; } = new List<Berth>();
        public int TotalBerths { get; set; }
        public int ActiveAssignments { get; set; }
    
        public string GetSummary()
   {
     return $"Active: {ActiveAssignments}/{TotalBerths} berths, Available: {AvailableBerths.Count}";
    }
    }
}
