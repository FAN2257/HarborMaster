using System;
using System.Collections.Generic;

namespace HarborMaster
{
    /// <summary>
    /// Program untuk mendemonstrasikan implementasi OOP concepts
    /// Uncomment kode di Main() untuk menjalankan demo
    /// </summary>
    public class ProgramExample
    {
        /* 
         * Uncomment method di bawah untuk menjalankan demo
         * 
        public static void Main(string[] args)
        {
            Console.WriteLine("??????????????????????????????????????????????????????????????");
            Console.WriteLine("?     HARBORMASTER - OOP IMPLEMENTATION DEMO                ?");
            Console.WriteLine("?  Inheritance | Encapsulation | Polymorphism              ?");
            Console.WriteLine("??????????????????????????????????????????????????????????????");
            Console.WriteLine();

            var demo = new HarborOperationsDemo();

            // Demo 1: Polymorphism dengan berbagai jenis kapal
            RunDemo("POLYMORPHISM - Docking Fee Calculation", () =>
            {
                demo.DemonstrateDockingFeeCalculation();
            });

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();

            // Demo 2: Service Management
            RunDemo("POLYMORPHISM - Service Management", () =>
            {
                demo.DemonstrateServiceManagement();
            });

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();

            // Demo 3: Encapsulation & Validation
            RunDemo("ENCAPSULATION - Data Validation", () =>
            {
                demo.DemonstrateEncapsulation();
            });

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();

            // Demo 4: Complete Scenario
            RunDemo("COMPLETE WORKFLOW - Multiple Ships", () =>
            {
                demo.RunCompleteScenario();
            });

            Console.WriteLine("\n\n??????????????????????????????????????????????????????????????");
            Console.WriteLine("?                    DEMO COMPLETED                         ?");
            Console.WriteLine("??????????????????????????????????????????????????????????????");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static void RunDemo(string title, Action demoAction)
        {
            Console.WriteLine("??????????????????????????????????????????????????????????????");
            Console.WriteLine($"? {title.PadRight(58)} ?");
            Console.WriteLine("??????????????????????????????????????????????????????????????");
            Console.WriteLine();

            try
            {
                demoAction();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n? Error: {ex.Message}");
            }
        }
        */

        /// <summary>
        /// Example: Membuat dan mengelola berbagai jenis kapal
        /// </summary>
        public static void ExampleShipCreation()
        {
            Console.WriteLine("=== Ship Creation Example ===\n");

            // 1. Membuat Cargo Ship
            var cargoShip = new CargoShip(
                shipID: "CARGO-001",
                name: "Pacific Trader",
                flag: "Singapore",
                tonnage: 50000m,
                cargoCapacity: 30000m,
                cargoType: "Electronics"
            );

            Console.WriteLine(cargoShip.GetShipDetails());
            Console.WriteLine($"Docking Fee: ${cargoShip.CalculateDockingFee():N2}");
            Console.WriteLine($"Priority: {cargoShip.GetPriorityLevel()}");
            Console.WriteLine();

            // 2. Membuat Passenger Ship (Cruise)
            var cruiseShip = new PassengerShip(
                shipID: "CRUISE-001",
                name: "Ocean Dream",
                flag: "Bahamas",
                tonnage: 85000m,
                passengerCapacity: 3000,
                crewCount: 1200,
                shipClass: "Cruise"
            );

            Console.WriteLine(cruiseShip.GetShipDetails());
            Console.WriteLine($"Docking Fee: ${cruiseShip.CalculateDockingFee():N2}");
            Console.WriteLine($"Priority: {cruiseShip.GetPriorityLevel()}");
            Console.WriteLine();

            // 3. Membuat Tanker Ship (LNG)
            var lngTanker = new TankerShip(
                shipID: "TANK-LNG-001",
                name: "Arctic LNG",
                flag: "Norway",
                tonnage: 110000m,
                liquidCargoCapacity: 75000m,
                liquidType: "LNG"
            );

            Console.WriteLine(lngTanker.GetShipDetails());
            Console.WriteLine($"Docking Fee: ${lngTanker.CalculateDockingFee():N2}");
            Console.WriteLine($"Priority: {lngTanker.GetPriorityLevel()}");
            Console.WriteLine();

            // 4. Demonstrasi Polymorphism - semua ship dalam satu list
            Console.WriteLine("=== Polymorphism Demo ===");
            List<Ship> allShips = new List<Ship> { cargoShip, cruiseShip, lngTanker };

            foreach (var ship in allShips)
            {
                // Method calls are polymorphic - different behavior for each type
                Console.WriteLine($"{ship.Name}: ${ship.CalculateDockingFee():N2} (Priority: {ship.GetPriorityLevel()})");
            }
        }

        /// <summary>
        /// Example: Membuat dan menghitung biaya layanan
        /// </summary>
        public static void ExampleServiceManagement()
        {
            Console.WriteLine("=== Service Management Example ===\n");

            // Buat kapal
            var cargoShip = new CargoShip("CARGO-100", "Global Trader", "China", 60000m, 40000m, "Hazardous Chemicals");

            // Buat layanan
            var dockingService = new DockingService("DOCK-001", 1000m, 72, includesPowerSupply: true);
            var cargoService = new CargoHandlingService("CARGO-001", 500m, 40000m, "Special Crane")
            {
                RequiresSpecialHandling = true
            };
            var maintenanceService = new MaintenanceService("MAINT-001", 2000m, "Routine", 3);
            maintenanceService.AddWorkItem("Hull Inspection");
            maintenanceService.AddWorkItem("Engine Check");
            
            var refuelingService = new RefuelingService("FUEL-001", 100m, 50000m, "Diesel", 1.5m);

            // Gunakan ServiceManager
            var manager = new ServiceManager();
            manager.AddService(dockingService);
            manager.AddService(cargoService);
            manager.AddService(maintenanceService);
            manager.AddService(refuelingService);

            // Generate report (menggunakan polymorphism)
            Console.WriteLine(manager.GenerateServiceReport(cargoShip));
        }

        /// <summary>
        /// Example: Complete workflow dari ship arrival hingga invoice
        /// </summary>
        public static void ExampleCompleteWorkflow()
        {
            Console.WriteLine("=== Complete Workflow Example ===\n");

            // 1. Setup
            var berth = new Berth
            {
                BerthID = "B1",
                Location = "North Dock",
                Capacity = 100000
            };

            var ship = new PassengerShip("PASS-001", "Royal Caribbean", "Bahamas", 95000m, 3500, 1200, "Cruise");

            // 2. Prepare services
            var services = new List<PortService>
            {
                new DockingService("DOCK-001", 2000m, 24, true),
                new MaintenanceService("MAINT-001", 1500m, "Routine", 1),
                new RefuelingService("FUEL-001", 200m, 80000m, "Heavy Fuel Oil", 1.2m)
            };

            // 3. Process arrival
            Console.WriteLine($"Processing arrival for {ship.Name}...");
            Console.WriteLine($"Priority Level: {ship.GetPriorityLevel()}");
            Console.WriteLine($"Required Services: {string.Join(", ", ship.GetRequiredServices())}");
            Console.WriteLine();

            // 4. Assign berth
            try
            {
                berth.AssignShip(ship);
                Console.WriteLine($"? Ship assigned to berth {berth.BerthID} at {berth.Location}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Error: {ex.Message}");
                return;
            }

            // 5. Generate invoice
            var invoice = Invoice.GenerateInvoice(ship);
            foreach (var service in services)
            {
                invoice.AddService(service);
                service.RequestService(ship);
            }

            decimal total = invoice.CalculateTotal();
            Console.WriteLine($"\n? Invoice generated: {invoice.InvoiceID}");
            Console.WriteLine($"Total Amount: ${total:N2}");
            Console.WriteLine("\n" + invoice.GetInvoiceDetails());
        }

        /// <summary>
        /// Example: Demonstrasi encapsulation dengan validation
        /// </summary>
        public static void ExampleEncapsulation()
        {
            Console.WriteLine("=== Encapsulation & Validation Example ===\n");

            // Valid operations
            Console.WriteLine("Valid Operations:");
            var ship = new CargoShip("VALID-001", "Valid Ship", "USA", 50000m, 30000m, "Electronics");
            Console.WriteLine($"? Ship created: {ship.Name}");
            
            ship.Tonnage = 55000m;
            Console.WriteLine($"? Tonnage updated to: {ship.Tonnage}");

            // Invalid operations
            Console.WriteLine("\nInvalid Operations (will throw exceptions):");
            
            try
            {
                ship.Tonnage = -1000m;
                Console.WriteLine("? This should not print");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"? Prevented invalid tonnage: {ex.Message}");
            }

            try
            {
                var invalidShip = new CargoShip("", "No ID", "USA", 50000m, 30000m, "Electronics");
                Console.WriteLine("? This should not print");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"? Prevented empty ShipID: {ex.Message}");
            }

            try
            {
                var invalidShip = new CargoShip("SHIP-001", "", "USA", 50000m, 30000m, "Electronics");
                Console.WriteLine("? This should not print");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"? Prevented empty Name: {ex.Message}");
            }

            try
            {
                var invalidShip = new CargoShip("SHIP-001", "Test", "USA", 0m, 30000m, "Electronics");
                Console.WriteLine("? This should not print");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"? Prevented zero/negative tonnage: {ex.Message}");
            }

            Console.WriteLine("\n? Encapsulation successfully protects data integrity!");
        }

        /// <summary>
        /// Example: Comparison biaya untuk berbagai jenis kapal
        /// </summary>
        public static void ExampleCostComparison()
        {
            Console.WriteLine("=== Cost Comparison Example ===\n");

            // Create ships dengan tonnage yang sama
            decimal tonnage = 50000m;

            var cargoShip = new CargoShip("C1", "Cargo", "USA", tonnage, 30000m, "General");
            var cargoHazmat = new CargoShip("C2", "Cargo Hazmat", "USA", tonnage, 30000m, "Hazardous Chemicals");
            var passengerShip = new PassengerShip("P1", "Passenger", "USA", tonnage, 1000, 200, "Ferry");
            var cruiseShip = new PassengerShip("P2", "Cruise", "USA", tonnage, 2500, 800, "Cruise");
            var tankerOil = new TankerShip("T1", "Oil Tanker", "USA", tonnage, 40000m, "Oil");
            var tankerLNG = new TankerShip("T2", "LNG Tanker", "USA", tonnage, 40000m, "LNG");

            Console.WriteLine($"All ships have {tonnage:N0} tons\n");
            Console.WriteLine("Ship Type                  | Docking Fee    | Priority");
            Console.WriteLine("---------------------------|----------------|----------");

            var ships = new List<Ship> { cargoShip, cargoHazmat, passengerShip, cruiseShip, tankerOil, tankerLNG };
            foreach (var ship in ships)
            {
                Console.WriteLine($"{ship.Name,-26} | ${ship.CalculateDockingFee(),12:N2} | {ship.GetPriorityLevel()}");
            }

            Console.WriteLine("\nObservations:");
            Console.WriteLine("- Hazardous cargo ships have higher fees and priority");
            Console.WriteLine("- Cruise ships have higher fees than ferries");
            Console.WriteLine("- LNG tankers have highest fees due to special handling");
            Console.WriteLine("- Priority increases with safety requirements");
        }
    }
}
