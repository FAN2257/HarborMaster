using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarborMaster
{
    // Base class menggunakan abstract untuk menerapkan Inheritance
    public abstract class Ship
    {
        // Encapsulation: private fields dengan public properties
        private string _shipID;
        private string _name;
        private string _type;
        private decimal _tonnage;

        public string ShipID 
        { 
            get => _shipID;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("ShipID cannot be empty");
                _shipID = value;
            }
        }

        public string Name 
        { 
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Ship name cannot be empty");
                _name = value;
            }
        }

        public string Type 
        { 
            get => _type;
            protected set => _type = value; // Protected untuk derived classes
        }

        public string Flag { get; set; }

        public decimal Tonnage
        {
            get => _tonnage;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Tonnage must be greater than zero");
                _tonnage = value;
            }
        }

        public DateTime? ArrivalTime { get; private set; }
        public DateTime? DepartureTime { get; private set; }
        public string Status { get; private set; } // Arrived, Departed, Waiting

        // Constructor
        protected Ship(string shipID, string name, string flag, decimal tonnage)
        {
            ShipID = shipID;
            Name = name;
            Flag = flag;
            Tonnage = tonnage;
            Status = "Waiting";
        }

        public void Arrive()
        {
            Status = "Arrived";
            ArrivalTime = DateTime.Now;
        }

        public void Depart()
        {
            Status = "Departed";
            DepartureTime = DateTime.Now;
        }

        public void UpdateStatus(string newStatus)
        {
            Status = newStatus;
        }

        // Polymorphism: Virtual method untuk docking fee calculation
        // Setiap jenis kapal memiliki perhitungan berbeda
        public virtual decimal CalculateDockingFee()
        {
            // Base calculation: $10 per ton
            return Tonnage * 10;
        }

        // Polymorphism: Virtual method untuk priority level
        public virtual int GetPriorityLevel()
        {
            return 1; // Default priority
        }

        // Polymorphism: Virtual method untuk service requirements
        public virtual List<string> GetRequiredServices()
        {
            return new List<string> { "Basic Inspection", "Documentation" };
        }

        // Abstract method yang harus diimplementasikan oleh derived classes
        public abstract string GetShipDetails();
    }

    // Derived Class 1: Cargo Ship
    public class CargoShip : Ship
    {
        public decimal CargoCapacity { get; set; } // in tons
        public string CargoType { get; set; }

        public CargoShip(string shipID, string name, string flag, decimal tonnage, decimal cargoCapacity, string cargoType)
            : base(shipID, name, flag, tonnage)
        {
            Type = "Cargo";
            CargoCapacity = cargoCapacity;
            CargoType = cargoType;
        }

        // Override: Cargo ships pay extra based on cargo capacity
        public override decimal CalculateDockingFee()
        {
            decimal baseFee = base.CalculateDockingFee();
            decimal cargoFee = CargoCapacity * 5; // $5 per ton of cargo capacity
            return baseFee + cargoFee;
        }

        public override int GetPriorityLevel()
        {
            // Higher priority for hazardous cargo
            return CargoType.Contains("Hazardous") ? 3 : 2;
        }

        public override List<string> GetRequiredServices()
        {
            var services = base.GetRequiredServices();
            services.Add("Cargo Handling");
            services.Add("Loading/Unloading Equipment");
            
            if (CargoType.Contains("Hazardous"))
            {
                services.Add("Hazmat Inspection");
                services.Add("Special Handling");
            }
            
            return services;
        }

        public override string GetShipDetails()
        {
            return $"Cargo Ship: {Name} | ID: {ShipID} | Flag: {Flag} | Tonnage: {Tonnage}t | Cargo Capacity: {CargoCapacity}t | Type: {CargoType}";
        }
    }

    // Derived Class 2: Passenger Ship
    public class PassengerShip : Ship
    {
        public int PassengerCapacity { get; set; }
        public int CrewCount { get; set; }
        public string ShipClass { get; set; } // Cruise, Ferry, etc.

        public PassengerShip(string shipID, string name, string flag, decimal tonnage, int passengerCapacity, int crewCount, string shipClass)
            : base(shipID, name, flag, tonnage)
        {
            Type = "Passenger";
            PassengerCapacity = passengerCapacity;
            CrewCount = crewCount;
            ShipClass = shipClass;
        }

        // Override: Passenger ships pay based on passenger capacity
        public override decimal CalculateDockingFee()
        {
            decimal baseFee = base.CalculateDockingFee();
            decimal passengerFee = PassengerCapacity * 2; // $2 per passenger capacity
            
            // Premium for cruise ships
            if (ShipClass == "Cruise")
                passengerFee *= 1.5m;
            
            return baseFee + passengerFee;
        }

        public override int GetPriorityLevel()
        {
            // Higher priority for passenger ships for safety
            return PassengerCapacity > 500 ? 4 : 3;
        }

        public override List<string> GetRequiredServices()
        {
            var services = base.GetRequiredServices();
            services.Add("Passenger Terminal Access");
            services.Add("Customs & Immigration");
            services.Add("Medical Screening");
            services.Add("Security Check");
            
            if (ShipClass == "Cruise")
            {
                services.Add("VIP Boarding Services");
            }
            
            return services;
        }

        public override string GetShipDetails()
        {
            return $"Passenger Ship: {Name} | ID: {ShipID} | Flag: {Flag} | Tonnage: {Tonnage}t | Passengers: {PassengerCapacity} | Crew: {CrewCount} | Class: {ShipClass}";
        }
    }

    // Derived Class 3: Tanker Ship
    public class TankerShip : Ship
    {
        public decimal LiquidCargoCapacity { get; set; } // in cubic meters
        public string LiquidType { get; set; } // Oil, LNG, Chemical
        public bool RequiresSpecialHandling { get; set; }

        public TankerShip(string shipID, string name, string flag, decimal tonnage, decimal liquidCargoCapacity, string liquidType)
            : base(shipID, name, flag, tonnage)
        {
            Type = "Tanker";
            LiquidCargoCapacity = liquidCargoCapacity;
            LiquidType = liquidType;
            RequiresSpecialHandling = liquidType == "LNG" || liquidType == "Chemical";
        }

        // Override: Tankers have special fees based on liquid type
        public override decimal CalculateDockingFee()
        {
            decimal baseFee = base.CalculateDockingFee();
            decimal liquidFee = LiquidCargoCapacity * 8; // $8 per cubic meter
            
            // Additional fee for special handling
            if (RequiresSpecialHandling)
                liquidFee *= 1.8m;
            
            return baseFee + liquidFee;
        }

        public override int GetPriorityLevel()
        {
            // Highest priority for safety-critical tankers
            return RequiresSpecialHandling ? 5 : 3;
        }

        public override List<string> GetRequiredServices()
        {
            var services = base.GetRequiredServices();
            services.Add("Liquid Cargo Handling");
            services.Add("Pipeline Connection");
            services.Add("Environmental Compliance Check");
            
            if (RequiresSpecialHandling)
            {
                services.Add("Special Safety Measures");
                services.Add("Hazmat Team Standby");
                services.Add("Fire Safety Equipment");
            }
            
            return services;
        }

        public override string GetShipDetails()
        {
            return $"Tanker Ship: {Name} | ID: {ShipID} | Flag: {Flag} | Tonnage: {Tonnage}t | Liquid Capacity: {LiquidCargoCapacity}m³ | Type: {LiquidType} | Special Handling: {RequiresSpecialHandling}";
        }
    }
}
