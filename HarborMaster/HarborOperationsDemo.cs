using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarborMaster
{
    /// <summary>
    /// Demonstrasi penggunaan konsep OOP: Inheritance, Encapsulation, dan Polymorphism
    /// dalam sistem HarborMaster
    /// </summary>
    public class HarborOperationsDemo
    {
        private List<Ship> _ships = new List<Ship>();
        private List<Berth> _berths = new List<Berth>();
        private ServiceManager _serviceManager = new ServiceManager();

        /// <summary>
        /// Mendemonstrasikan Polymorphism dengan berbagai jenis kapal
        /// </summary>
        public void DemonstrateDockingFeeCalculation()
        {
            Console.WriteLine("=== POLYMORPHISM DEMO: Docking Fee Calculation ===\n");

            // Membuat berbagai jenis kapal (Inheritance)
            var ships = new List<Ship>
            {
                new CargoShip("CARGO-001", "Pacific Trader", "USA", 50000, 30000, "Electronics"),
                new CargoShip("CARGO-002", "Chemical Express", "Singapore", 45000, 25000, "Hazardous Chemicals"),
                new PassengerShip("PASS-001", "Ocean Dream", "Norway", 70000, 2500, 800, "Cruise"),
                new PassengerShip("PASS-002", "Island Ferry", "Greece", 8000, 500, 50, "Ferry"),
                new TankerShip("TANK-001", "Oil Giant", "Saudi Arabia", 120000, 80000, "Oil"),
                new TankerShip("TANK-002", "LNG Carrier", "Qatar", 95000, 60000, "LNG")
            };

            // Polymorphism in action: setiap jenis kapal menghitung fee berbeda
            foreach (var ship in ships)
            {
                Console.WriteLine(ship.GetShipDetails());
                Console.WriteLine($"Docking Fee: ${ship.CalculateDockingFee():N2}");
                Console.WriteLine($"Priority Level: {ship.GetPriorityLevel()}");
                Console.WriteLine($"Required Services: {string.Join(", ", ship.GetRequiredServices())}");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Mendemonstrasikan Service Management dengan polymorphic cost calculation
        /// </summary>
        public void DemonstrateServiceManagement()
        {
            Console.WriteLine("=== SERVICE MANAGEMENT DEMO ===\n");

            // Membuat kapal
            var cargoShip = new CargoShip("CARGO-100", "Global Trader", "China", 60000, 40000, "Hazardous Chemicals");
            
            // Membuat berbagai layanan (Polymorphism)
            var dockingService = new DockingService("DOCK-001", 1000, 48, true);
            var cargoService = new CargoHandlingService("CARGO-001", 500, 40000, "Special Crane");
            cargoService.RequiresSpecialHandling = true;
            
            var maintenanceService = new MaintenanceService("MAINT-001", 2000, "Routine", 3);
            maintenanceService.AddWorkItem("Hull Inspection");
            maintenanceService.AddWorkItem("Engine Check");
            maintenanceService.AddWorkItem("Safety Equipment Test");
            
            var refuelingService = new RefuelingService("FUEL-001", 100, 50000, "Diesel", 1.5m);

            var serviceManager = new ServiceManager();
            serviceManager.AddService(dockingService);
            serviceManager.AddService(cargoService);
            serviceManager.AddService(maintenanceService);
            serviceManager.AddService(refuelingService);

            // Generate report (menggunakan polymorphic methods)
            Console.WriteLine(serviceManager.GenerateServiceReport(cargoShip));
        }

        /// <summary>
        /// Mendemonstrasikan complete workflow dari kapal masuk hingga invoice
        /// </summary>
        public Invoice ProcessShipArrival(Ship ship, Berth berth, List<PortService> requestedServices)
        {
            Console.WriteLine($"\n=== Processing Arrival for {ship.Name} ===");
            
            // 1. Check ship priority
            int priority = ship.GetPriorityLevel();
            Console.WriteLine($"Ship Priority Level: {priority}");
            
            // 2. Assign berth
            try
            {
                berth.AssignShip(ship);
                Console.WriteLine($"Ship assigned to berth {berth.BerthID} at {berth.Location}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error assigning berth: {ex.Message}");
                return null;
            }
            
            // 3. Process services
            var serviceManager = new ServiceManager();
            foreach (var service in requestedServices)
            {
                serviceManager.AddService(service);
                service.RequestService(ship);
            }
            
            // 4. Generate Invoice
            var invoice = Invoice.GenerateInvoice(ship);
            foreach (var service in requestedServices)
            {
                invoice.AddService(service);
            }
            
            decimal total = invoice.CalculateTotal();
            Console.WriteLine($"Invoice generated: {invoice.InvoiceID}");
            Console.WriteLine($"Total Amount: ${total:N2}");
            
            return invoice;
        }

        /// <summary>
        /// Demonstrasi complete scenario dengan berbagai jenis kapal
        /// </summary>
        public void RunCompleteScenario()
        {
            Console.WriteLine("=== COMPLETE HARBOR OPERATIONS SCENARIO ===\n");
            
            // Setup berths
            var berth1 = new Berth { BerthID = "B1", Location = "North Dock", Capacity = 100000 };
            var berth2 = new Berth { BerthID = "B2", Location = "South Dock", Capacity = 150000 };
            var berth3 = new Berth { BerthID = "B3", Location = "East Dock", Capacity = 80000 };
            
            // Scenario 1: Cargo Ship with hazardous materials
            Console.WriteLine("\n--- SCENARIO 1: Hazardous Cargo Ship ---");
            var hazmatShip = new CargoShip("CARGO-HAZ-001", "Hazmat Express", "Netherlands", 55000, 35000, "Hazardous Chemicals");
            
            var hazmatServices = new List<PortService>
            {
                new DockingService("DOCK-H1", 1500, 72, true),
                new CargoHandlingService("CARGO-H1", 800, 35000, "Special Crane") { RequiresSpecialHandling = true },
                new MaintenanceService("MAINT-H1", 3000, "Emergency", 2)
            };
            
            var invoice1 = ProcessShipArrival(hazmatShip, berth1, hazmatServices);
            if (invoice1 != null)
            {
                Console.WriteLine("\n" + invoice1.GetInvoiceDetails());
            }
            
            // Scenario 2: Luxury Cruise Ship
            Console.WriteLine("\n--- SCENARIO 2: Luxury Cruise Ship ---");
            var cruiseShip = new PassengerShip("CRUISE-001", "Royal Caribbean", "Bahamas", 95000, 3500, 1200, "Cruise");
            
            var cruiseServices = new List<PortService>
            {
                new DockingService("DOCK-C1", 2000, 24, true),
                new MaintenanceService("MAINT-C1", 1500, "Routine", 1),
                new RefuelingService("FUEL-C1", 200, 80000, "Heavy Fuel Oil", 1.2m)
            };
            
            var invoice2 = ProcessShipArrival(cruiseShip, berth2, cruiseServices);
            if (invoice2 != null)
            {
                Console.WriteLine("\n" + invoice2.GetInvoiceDetails());
            }
            
            // Scenario 3: LNG Tanker
            Console.WriteLine("\n--- SCENARIO 3: LNG Tanker ---");
            var lngTanker = new TankerShip("TANK-LNG-001", "Arctic LNG", "Russia", 110000, 75000, "LNG");
            
            var tankerServices = new List<PortService>
            {
                new DockingService("DOCK-T1", 2500, 96, false),
                new MaintenanceService("MAINT-T1", 5000, "Routine", 5),
                new RefuelingService("FUEL-T1", 150, 30000, "Diesel", 1.3m)
            };
            
            var invoice3 = ProcessShipArrival(lngTanker, berth3, tankerServices);
            if (invoice3 != null)
            {
                Console.WriteLine("\n" + invoice3.GetInvoiceDetails());
            }
            
            // Summary
            Console.WriteLine("\n=== OPERATIONS SUMMARY ===");
            Console.WriteLine($"Total Ships Processed: 3");
            Console.WriteLine($"Berth 1 Status: {berth1.Status}");
            Console.WriteLine($"Berth 2 Status: {berth2.Status}");
            Console.WriteLine($"Berth 3 Status: {berth3.Status}");
        }

        /// <summary>
        /// Method untuk mendemonstrasikan encapsulation dengan validation
        /// </summary>
        public void DemonstrateEncapsulation()
        {
            Console.WriteLine("=== ENCAPSULATION DEMO: Data Validation ===\n");
            
            try
            {
                // Valid ship creation
                var ship1 = new CargoShip("VALID-001", "Valid Ship", "USA", 50000, 30000, "Electronics");
                Console.WriteLine("? Valid ship created successfully");
                Console.WriteLine(ship1.GetShipDetails());
                
                // Attempt invalid operations
                Console.WriteLine("\nAttempting invalid operations...");
                
                try
                {
                    ship1.Tonnage = -1000; // Should fail
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"? Validation prevented invalid tonnage: {ex.Message}");
                }
                
                try
                {
                    var invalidShip = new CargoShip("", "No ID Ship", "USA", 50000, 30000, "Electronics");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"? Validation prevented empty ShipID: {ex.Message}");
                }
                
                try
                {
                    var invalidService = new DockingService("", 1000, 24, true);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"? Validation prevented empty ServiceID: {ex.Message}");
                }
                
                Console.WriteLine("\n? Encapsulation successfully protects data integrity!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
