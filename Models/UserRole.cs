using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarborMaster.Models
{
    /// <summary>
    /// User roles with different permission levels
    /// ShipOwner: Can manage own ships and submit docking requests
    /// Operator: Can approve/reject requests and manage berth allocations
    /// HarborMaster: Has all Operator permissions + can override conflicts
    /// </summary>
    public enum UserRole
    {
        ShipOwner = 0,      // Lowest privilege - owns ships, submits requests
        Operator = 1,       // Mid privilege - approves requests, allocates berths
        HarborMaster = 2    // Highest privilege - can override schedule conflicts
    }
}
