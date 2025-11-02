using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarborMaster
{
    // Base class untuk Port Service dengan encapsulation
    public abstract class PortService
    {
        private string _serviceID;
        private string _type;
        private decimal _baseCost;

        public string ServiceID 
        { 
            get => _serviceID;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("ServiceID cannot be empty");
                _serviceID = value;
            }
        }

        public string Type 
        { 
            get => _type;
            protected set => _type = value;
        }

        public decimal BaseCost 
        { 
            get => _baseCost;
            protected set
            {
                if (value < 0)
                    throw new ArgumentException("Cost cannot be negative");
                _baseCost = value;
            }
        }

        public string Status { get; protected set; }
        public DateTime? RequestedTime { get; protected set; }
        public DateTime? CompletedTime { get; protected set; }

        protected PortService(string serviceID, decimal baseCost)
        {
            ServiceID = serviceID;
            BaseCost = baseCost;
            Status = "Pending";
        }

        public virtual void RequestService(Ship ship)
        {
            Status = "Requested";
            RequestedTime = DateTime.Now;
            Console.WriteLine($"Service {Type} requested for {ship.Name}");
        }

        public virtual void CompleteService()
        {
            Status = "Completed";
            CompletedTime = DateTime.Now;
        }

        // Polymorphism: Virtual method untuk calculate cost berdasarkan jenis ship
        public virtual decimal CalculateCost(Ship ship)
        {
            return BaseCost;
        }

        // Compatibility dengan kode lama
        public decimal CalculateCost()
        {
            return BaseCost;
        }

        public decimal Cost 
        { 
            get => BaseCost;
            set => BaseCost = value;
        }

        public abstract string GetServiceDescription();
    }

    // Derived Service 1: Docking Service
    public class DockingService : PortService
    {
        public int DurationHours { get; set; }
        public bool IncludesPowerSupply { get; set; }

        public DockingService(string serviceID, decimal baseCost, int durationHours, bool includesPowerSupply = false)
            : base(serviceID, baseCost)
        {
            Type = "Docking";
            DurationHours = durationHours;
            IncludesPowerSupply = includesPowerSupply;
        }

        public override decimal CalculateCost(Ship ship)
        {
            decimal cost = ship.CalculateDockingFee();
            
            // Additional cost per hour
            cost += DurationHours * 50;
            
            // Power supply surcharge
            if (IncludesPowerSupply)
                cost += DurationHours * 25;
            
            return cost;
        }

        public override string GetServiceDescription()
        {
            return $"Docking Service for {DurationHours} hours {(IncludesPowerSupply ? "with power supply" : "")}";
        }
    }

    // Derived Service 2: Cargo Handling Service
    public class CargoHandlingService : PortService
    {
        public decimal CargoWeight { get; set; }
        public string HandlingEquipment { get; set; }
        public bool RequiresSpecialHandling { get; set; }

        public CargoHandlingService(string serviceID, decimal baseCost, decimal cargoWeight, string equipment)
            : base(serviceID, baseCost)
        {
            Type = "Cargo Handling";
            CargoWeight = cargoWeight;
            HandlingEquipment = equipment;
        }

        public override decimal CalculateCost(Ship ship)
        {
            decimal cost = BaseCost;
            
            // Cost based on cargo weight
            cost += CargoWeight * 0.5m;
            
            // Premium for cargo ships
            if (ship is CargoShip cargoShip)
            {
                cost += cargoShip.CargoCapacity * 0.2m;
                
                if (cargoShip.CargoType.Contains("Hazardous"))
                    cost *= 1.5m;
            }
            
            // Equipment surcharge
            if (HandlingEquipment == "Crane")
                cost += 500;
            else if (HandlingEquipment == "Special Crane")
                cost += 1000;
            
            if (RequiresSpecialHandling)
                cost *= 1.3m;
            
            return cost;
        }

        public override string GetServiceDescription()
        {
            return $"Cargo Handling Service: {CargoWeight}t using {HandlingEquipment}{(RequiresSpecialHandling ? " (Special Handling)" : "")}";
        }
    }

    // Derived Service 3: Maintenance Service
    public class MaintenanceService : PortService
    {
        public string MaintenanceType { get; set; } // Routine, Emergency, Repair
        public List<string> WorkItems { get; set; }
        public int EstimatedDays { get; set; }

        public MaintenanceService(string serviceID, decimal baseCost, string maintenanceType, int estimatedDays)
            : base(serviceID, baseCost)
        {
            Type = "Maintenance";
            MaintenanceType = maintenanceType;
            EstimatedDays = estimatedDays;
            WorkItems = new List<string>();
        }

        public void AddWorkItem(string item)
        {
            WorkItems.Add(item);
        }

        public override decimal CalculateCost(Ship ship)
        {
            decimal cost = BaseCost;
            
            // Cost per day
            cost += EstimatedDays * 1000;
            
            // Cost per work item
            cost += WorkItems.Count * 200;
            
            // Emergency surcharge
            if (MaintenanceType == "Emergency")
                cost *= 2.0m;
            
            // Ship size factor
            cost += ship.Tonnage * 0.1m;
            
            return cost;
        }

        public override string GetServiceDescription()
        {
            return $"{MaintenanceType} Maintenance Service: {EstimatedDays} days, {WorkItems.Count} work items";
        }
    }

    // Derived Service 4: Refueling Service
    public class RefuelingService : PortService
    {
        public decimal FuelAmount { get; set; } // in liters
        public string FuelType { get; set; } // Diesel, Heavy Fuel Oil, LNG
        public decimal FuelPricePerLiter { get; set; }

        public RefuelingService(string serviceID, decimal baseCost, decimal fuelAmount, string fuelType, decimal pricePerLiter)
            : base(serviceID, baseCost)
        {
            Type = "Refueling";
            FuelAmount = fuelAmount;
            FuelType = fuelType;
            FuelPricePerLiter = pricePerLiter;
        }

        public override decimal CalculateCost(Ship ship)
        {
            decimal cost = BaseCost; // Service charge
            
            // Fuel cost
            cost += FuelAmount * FuelPricePerLiter;
            
            // Premium for special fuel types
            if (FuelType == "LNG")
                cost *= 1.2m;
            
            // Discount for tanker ships
            if (ship is TankerShip)
                cost *= 0.9m;
            
            return cost;
        }

        public override string GetServiceDescription()
        {
            return $"Refueling Service: {FuelAmount}L of {FuelType} @ ${FuelPricePerLiter}/L";
        }
    }

    // Service Manager untuk mengelola services dengan polymorphism
    public class ServiceManager
    {
        private List<PortService> _services = new List<PortService>();

        public void AddService(PortService service)
        {
            _services.Add(service);
        }

        public decimal CalculateTotalCost(Ship ship)
        {
            decimal total = 0;
            foreach (var service in _services)
            {
                total += service.CalculateCost(ship);
            }
            return total;
        }

        public List<PortService> GetServicesByStatus(string status)
        {
            return _services.Where(s => s.Status == status).ToList();
        }

        public string GenerateServiceReport(Ship ship)
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine($"=== Service Report for {ship.Name} ===");
            report.AppendLine(ship.GetShipDetails());
            report.AppendLine("\nServices:");
            
            foreach (var service in _services)
            {
                report.AppendLine($"- {service.GetServiceDescription()}");
                report.AppendLine($"  Cost: ${service.CalculateCost(ship):N2}");
                report.AppendLine($"  Status: {service.Status}");
            }
            
            report.AppendLine($"\nTotal Cost: ${CalculateTotalCost(ship):N2}");
            return report.ToString();
        }

        public void ProcessAllServices(Ship ship)
        {
            foreach (var service in _services)
            {
                if (service.Status == "Pending" || service.Status == "Requested")
                {
                    service.RequestService(ship);
                }
            }
        }

        public void CompleteAllServices()
        {
            foreach (var service in _services.Where(s => s.Status == "Requested"))
            {
                service.CompleteService();
            }
        }
    }
}
