using System;
using System.Collections.Generic;
using System.Linq;

namespace HarborMaster
{
    // Base abstract class untuk semua port services
    public abstract class PortServiceBase
    {
        public string ServiceID { get; protected set; }
     public decimal BaseCost { get; protected set; }
        public string Status { get; set; } = "Available";
        
        protected PortServiceBase(string serviceId, decimal baseCost)
        {
 if (string.IsNullOrWhiteSpace(serviceId))
     throw new ArgumentException("ServiceID cannot be empty");
   
     if (baseCost < 0)
     throw new ArgumentException("BaseCost cannot be negative");
 
        ServiceID = serviceId;
            BaseCost = baseCost;
        }
    
        public abstract decimal CalculateCost(Ship ship);
    public abstract string GetServiceDescription();
  public abstract void RequestService(Ship ship);
    }

    // Docking Service - Layanan berlabuh
    public class DockingService : PortServiceBase
    {
 public int DurationHours { get; set; }
     public bool IncludesPowerSupply { get; set; }
        
public DockingService(string serviceId, decimal baseCost, int durationHours, bool includesPowerSupply)
 : base(serviceId, baseCost)
        {
            DurationHours = durationHours;
 IncludesPowerSupply = includesPowerSupply;
        }
        
        public override decimal CalculateCost(Ship ship)
        {
            decimal cost = ship.CalculateDockingFee();
            cost += DurationHours * 50; // $50 per hour

      if (IncludesPowerSupply)
 cost += DurationHours * 25; // Additional $25 per hour for power
       
            return cost;
        }
        
      public override string GetServiceDescription()
        {
      return $"Docking Service: {DurationHours}h, Power: {(IncludesPowerSupply ? "Yes" : "No")}";
 }
        
    public override void RequestService(Ship ship)
     {
            Status = "In Use";
            // Logic for docking service
        }
    }

    // Cargo Handling Service - Layanan bongkar muat
    public class CargoHandlingService : PortServiceBase
    {
        public decimal CargoWeight { get; set; }
     public string EquipmentType { get; set; }
     public bool RequiresSpecialHandling { get; set; }
      
        public CargoHandlingService(string serviceId, decimal baseCost, decimal cargoWeight, string equipmentType)
: base(serviceId, baseCost)
        {
  CargoWeight = cargoWeight;
    EquipmentType = equipmentType;
        }
        
        public override decimal CalculateCost(Ship ship)
        {
            decimal cost = BaseCost;
            cost += CargoWeight * 0.5m; // $0.5 per unit weight
       
     if (ship is CargoShip cargoShip)
   {
                cost += cargoShip.CargoCapacity * 0.2m;
     if (cargoShip.CargoType.Contains("Hazardous"))
       cost *= 1.5m; // 50% surcharge for hazardous materials
    }
        
       if (RequiresSpecialHandling)
     cost *= 1.3m;
     
       return cost;
 }
        
     public override string GetServiceDescription()
        {
         return $"Cargo Handling: {CargoWeight}t, Equipment: {EquipmentType}";
   }
        
   public override void RequestService(Ship ship)
        {
 Status = "Loading/Unloading";
     }
    }

    // Maintenance Service - Layanan perawatan
    public class MaintenanceService : PortServiceBase
    {
 public string MaintenanceType { get; set; }
        public int EstimatedDays { get; set; }
        public List<string> WorkItems { get; private set; } = new List<string>();
   
        public MaintenanceService(string serviceId, decimal baseCost, string maintenanceType, int estimatedDays)
     : base(serviceId, baseCost)
        {
MaintenanceType = maintenanceType;
          EstimatedDays = estimatedDays;
   }
        
        public void AddWorkItem(string workItem)
        {
            WorkItems.Add(workItem);
     }
        
      public override decimal CalculateCost(Ship ship)
  {
            decimal cost = BaseCost * EstimatedDays;
 
            // Additional cost based on ship tonnage
      cost += ship.Tonnage * 0.1m;
            
   // Emergency maintenance costs more
      if (MaintenanceType.Contains("Emergency"))
              cost *= 2.0m;
          
      // Cost per work item
      cost += WorkItems.Count * 500m;
            
    return cost;
    }
        
        public override string GetServiceDescription()
     {
    return $"Maintenance: {MaintenanceType}, {EstimatedDays} days, {WorkItems.Count} work items";
        }
        
        public override void RequestService(Ship ship)
        {
     Status = "Maintenance in Progress";
        }
    }

 // Refueling Service - Layanan pengisian bahan bakar
    public class RefuelingService : PortServiceBase
    {
   public decimal FuelAmount { get; set; } // in liters
        public string FuelType { get; set; }
        public decimal PricePerLiter { get; set; }
        
 public RefuelingService(string serviceId, decimal baseCost, decimal fuelAmount, string fuelType, decimal pricePerLiter)
       : base(serviceId, baseCost)
  {
            FuelAmount = fuelAmount;
          FuelType = fuelType;
            PricePerLiter = pricePerLiter;
      }
        
        public override decimal CalculateCost(Ship ship)
      {
      decimal fuelCost = FuelAmount * PricePerLiter;
       decimal serviceFee = BaseCost;
            
 // Larger ships get volume discounts
   if (ship.Tonnage > 100000)
         fuelCost *= 0.95m; // 5% discount
         else if (ship.Tonnage > 50000)
     fuelCost *= 0.98m; // 2% discount
   
  return fuelCost + serviceFee;
      }
        
    public override string GetServiceDescription()
        {
   return $"Refueling: {FuelAmount:N0}L {FuelType} @ ${PricePerLiter:F2}/L";
        }
        
        public override void RequestService(Ship ship)
        {
            Status = "Refueling";
        }
    }

    // Service Manager - Mengelola multiple services
    public class ServiceManager
    {
        private List<PortServiceBase> _services = new List<PortServiceBase>();
        
        public void AddService(PortServiceBase service)
        {
      _services.Add(service);
     }
        
     public void RemoveService(string serviceId)
   {
            _services.RemoveAll(s => s.ServiceID == serviceId);
        }
    
      public decimal CalculateTotalCost(Ship ship)
        {
       return _services.Sum(service => service.CalculateCost(ship));
        }
        
        public string GenerateServiceReport(Ship ship)
      {
        var report = new System.Text.StringBuilder();
       report.AppendLine($"=== Service Report for {ship.Name} ===");
            report.AppendLine($"Ship Type: {ship.Type}");
            report.AppendLine($"Tonnage: {ship.Tonnage:N0} tons");
         report.AppendLine();
    
    decimal totalCost = 0;
 foreach (var service in _services)
     {
      decimal cost = service.CalculateCost(ship);
        totalCost += cost;
     
       report.AppendLine($"Service: {service.GetServiceDescription()}");
                report.AppendLine($"Cost: ${cost:N2}");
       report.AppendLine();
      }
        
        report.AppendLine($"Total Cost: ${totalCost:N2}");
  return report.ToString();
      }
        
        public List<PortServiceBase> GetAllServices()
        {
       return _services.ToList();
   }
      
        public List<PortServiceBase> GetServicesByType<T>() where T : PortServiceBase
        {
            return _services.OfType<T>().Cast<PortServiceBase>().ToList();
    }
    }
}