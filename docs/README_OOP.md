# HarborMaster - Implementasi OOP Concepts

## ?? Overview

Proyek ini mendemonstrasikan implementasi komprehensif dari tiga pilar utama **Object-Oriented Programming (OOP)** dalam sistem manajemen pelabuhan (Harbor Management System):

- ? **Inheritance (Pewarisan)**
- ? **Encapsulation (Enkapsulasi)**  
- ? **Polymorphism (Polimorfisme)**

## ?? Struktur File Utama

### Files yang Dimodifikasi/Dibuat:

1. **Ship.cs** - Abstract base class dengan 3 derived classes
   - `CargoShip` - Kapal kargo
   - `PassengerShip` - Kapal penumpang/cruise
   - `TankerShip` - Kapal tanker

2. **PortService.cs** - Abstract base class dengan 4 derived classes
   - `DockingService` - Layanan berlabuh
   - `CargoHandlingService` - Layanan bongkar muat
   - `MaintenanceService` - Layanan perawatan
   - `RefuelingService` - Layanan pengisian bahan bakar
   - `ServiceManager` - Class untuk mengelola services

3. **Invoice.cs** - Updated untuk mendukung polymorphic cost calculation

4. **HarborOperationsDemo.cs** - Class demo untuk semua konsep OOP

5. **ProgramExample.cs** - Contoh-contoh penggunaan praktis

6. **HarborMaster.cs** - Renamed menjadi `HarborMasterController` (menghindari konflik namespace)

## ?? Cara Menggunakan

### Option 1: Menjalankan Demo Lengkap

Edit file `ProgramExample.cs` dan uncomment method `Main()`, kemudian jalankan aplikasi. Demo akan menampilkan:

1. Polymorphism - Docking Fee Calculation
2. Polymorphism - Service Management
3. Encapsulation - Data Validation
4. Complete Workflow - Multiple Ships

### Option 2: Menggunakan Example Methods

Anda bisa memanggil individual example methods:

```csharp
// Di MainWindow.xaml.cs atau file lain
ProgramExample.ExampleShipCreation();
ProgramExample.ExampleServiceManagement();
ProgramExample.ExampleCompleteWorkflow();
ProgramExample.ExampleEncapsulation();
ProgramExample.ExampleCostComparison();
```

### Option 3: Menggunakan API Secara Langsung

```csharp
// 1. Membuat kapal (Inheritance)
var cargoShip = new CargoShip(
    "CARGO-001", 
    "Pacific Trader", 
    "Singapore", 
    50000m,      // tonnage
    30000m,      // cargo capacity
    "Electronics"
);

// 2. Menghitung docking fee (Polymorphism)
decimal fee = cargoShip.CalculateDockingFee();

// 3. Membuat layanan
var dockingService = new DockingService("DOCK-001", 1000m, 48, true);
var cargoService = new CargoHandlingService("CARGO-001", 500m, 30000m, "Crane");

// 4. Hitung biaya layanan (Polymorphism)
decimal serviceCost = dockingService.CalculateCost(cargoShip);

// 5. Generate invoice
var invoice = Invoice.GenerateInvoice(cargoShip);
invoice.AddService(dockingService);
invoice.AddService(cargoService);
decimal total = invoice.CalculateTotal();
```

## ?? Konsep OOP yang Diimplementasikan

### 1. INHERITANCE (Pewarisan)

**Ship Class Hierarchy:**
```
Ship (abstract)
??? CargoShip
??? PassengerShip
??? TankerShip
```

**PortService Class Hierarchy:**
```
PortService (abstract)
??? DockingService
??? CargoHandlingService
??? MaintenanceService
??? RefuelingService
```

**Keuntungan:**
- Code reusability
- Extensibility - mudah menambah jenis baru
- Type hierarchy yang jelas

### 2. ENCAPSULATION (Enkapsulasi)

**Private fields dengan validation:**
```csharp
private string _shipID;
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
```

**Access modifiers yang tepat:**
- `public` - API yang harus diakses dari luar
- `private` - Implementation details
- `protected` - Untuk derived classes
- `private set` - Read-only dari luar class

**Validations yang diterapkan:**
- ShipID tidak boleh kosong
- Name tidak boleh kosong
- Tonnage harus > 0
- ServiceID tidak boleh kosong
- Cost tidak boleh negatif

### 3. POLYMORPHISM (Polimorfisme)

**Method Overriding:**

Setiap jenis kapal menghitung docking fee berbeda:
```csharp
// CargoShip
public override decimal CalculateDockingFee()
{
    return base.CalculateDockingFee() + (CargoCapacity * 5);
}

// PassengerShip  
public override decimal CalculateDockingFee()
{
    decimal fee = base.CalculateDockingFee() + (PassengerCapacity * 2);
    if (ShipClass == "Cruise") fee *= 1.5m;
    return fee;
}

// TankerShip
public override decimal CalculateDockingFee()
{
    decimal fee = base.CalculateDockingFee() + (LiquidCargoCapacity * 8);
    if (RequiresSpecialHandling) fee *= 1.8m;
    return fee;
}
```

**Runtime polymorphism:**
```csharp
List<Ship> ships = new List<Ship> { cargo, passenger, tanker };
foreach (var ship in ships)
{
    // Different calculation for each type
    decimal fee = ship.CalculateDockingFee();
}
```

## ?? Contoh Skenario Penggunaan

### Skenario 1: Kapal Kargo dengan Hazmat

```csharp
var hazmatShip = new CargoShip(
    "CARGO-HAZ-001", 
    "Chemical Express", 
    "Netherlands", 
    55000m, 
    35000m, 
    "Hazardous Chemicals"
);

// Prioritas lebih tinggi karena hazardous
int priority = hazmatShip.GetPriorityLevel(); // Returns 3

// Services khusus untuk hazmat
var services = hazmatShip.GetRequiredServices();
// Includes: Hazmat Inspection, Special Handling

// Biaya lebih tinggi
decimal fee = hazmatShip.CalculateDockingFee();
```

### Skenario 2: Kapal Cruise Luxury

```csharp
var cruiseShip = new PassengerShip(
    "CRUISE-001",
    "Royal Caribbean",
    "Bahamas",
    95000m,
    3500,      // passengers
    1200,      // crew
    "Cruise"
);

// Prioritas tinggi untuk safety
int priority = cruiseShip.GetPriorityLevel(); // Returns 4

// Services premium
var services = cruiseShip.GetRequiredServices();
// Includes: VIP Boarding Services

// Fee premium untuk cruise
decimal fee = cruiseShip.CalculateDockingFee();
```

### Skenario 3: LNG Tanker

```csharp
var lngTanker = new TankerShip(
    "TANK-LNG-001",
    "Arctic LNG",
    "Norway",
    110000m,
    75000m,
    "LNG"
);

// Prioritas tertinggi untuk safety
int priority = lngTanker.GetPriorityLevel(); // Returns 5

// Services safety-critical
var services = lngTanker.GetRequiredServices();
// Includes: Hazmat Team Standby, Fire Safety Equipment

// Fee tertinggi karena special handling
decimal fee = lngTanker.CalculateDockingFee();
```

## ?? Testing & Validation

### Unit Test Recommendations:

1. **Test Inheritance:**
   - Semua derived classes bisa di-assign ke base class variable
   - Derived classes inherit base class properties/methods

2. **Test Encapsulation:**
   - Invalid data throws exceptions
   - Data validation bekerja dengan benar
   - Private fields tidak bisa diakses langsung

3. **Test Polymorphism:**
   - Method overrides bekerja dengan benar
   - Runtime type determination
   - Correct cost calculations per ship type

### Manual Testing:

Jalankan `HarborOperationsDemo`:
```csharp
var demo = new HarborOperationsDemo();
demo.DemonstrateDockingFeeCalculation();
demo.DemonstrateServiceManagement();
demo.DemonstrateEncapsulation();
demo.RunCompleteScenario();
```

## ?? Perbandingan Biaya (dengan Tonnage Sama)

Untuk kapal dengan tonnage 50,000 tons:

| Ship Type              | Docking Fee | Priority | Alasan                        |
|------------------------|-------------|----------|-------------------------------|
| Cargo (General)        | $650,000    | 2        | Base + cargo capacity         |
| Cargo (Hazmat)         | $975,000    | 3        | Base + cargo + hazmat premium |
| Passenger (Ferry)      | $502,000    | 3        | Base + passenger capacity     |
| Passenger (Cruise)     | $503,000    | 4        | Ferry + cruise premium        |
| Tanker (Oil)           | $820,000    | 3        | Base + liquid capacity        |
| Tanker (LNG)           | $1,476,000  | 5        | Oil + special handling 1.8x   |

## ?? Learning Points

### Inheritance
- Base class menyediakan common functionality
- Derived classes extend dengan specialized behavior
- Abstract classes mencegah instantiation langsung

### Encapsulation
- Data hiding melindungi integrity
- Validation mencegah invalid states
- Controlled access melalui properties

### Polymorphism
- Same interface, different behavior
- Runtime type determination
- Extensible tanpa mengubah existing code

## ??? Extensibility

### Menambah Jenis Kapal Baru:

```csharp
public class FishingShip : Ship
{
    public decimal FishCapacity { get; set; }
    
    public FishingShip(string shipID, string name, string flag, 
                      decimal tonnage, decimal fishCapacity)
        : base(shipID, name, flag, tonnage)
    {
        Type = "Fishing";
        FishCapacity = fishCapacity;
    }
    
    public override decimal CalculateDockingFee()
    {
        return base.CalculateDockingFee() + (FishCapacity * 3);
    }
    
    public override string GetShipDetails()
    {
        return $"Fishing Ship: {Name} | Capacity: {FishCapacity}t";
    }
}
```

### Menambah Service Baru:

```csharp
public class WasteDisposalService : PortService
{
    public decimal WasteVolume { get; set; }
    
    public WasteDisposalService(string serviceID, decimal baseCost, 
                                decimal wasteVolume)
        : base(serviceID, baseCost)
    {
        Type = "Waste Disposal";
        WasteVolume = wasteVolume;
    }
    
    public override decimal CalculateCost(Ship ship)
    {
        return BaseCost + (WasteVolume * 10);
    }
    
    public override string GetServiceDescription()
    {
        return $"Waste Disposal: {WasteVolume}m³";
    }
}
```

## ?? Dokumentasi Lengkap

Lihat `OOP_IMPLEMENTATION_GUIDE.md` untuk dokumentasi detail tentang:
- Arsitektur lengkap
- Best practices yang diterapkan
- Detailed explanations untuk setiap konsep
- Advanced usage scenarios

## ? Highlights

- ? Clean code dengan separation of concerns
- ? SOLID principles (terutama Open-Closed)
- ? Type-safe dengan compile-time checking
- ? Extensible untuk requirements masa depan
- ? Comprehensive validation
- ? Real-world applicable scenarios
- ? Modern C# features (pattern matching, expression-bodied members)

## ?? Contributing

Untuk menambah fitur baru:
1. Extend base classes dengan inheritance
2. Override virtual methods sesuai kebutuhan
3. Maintain encapsulation principles
4. Add validation untuk data integrity
5. Update documentation

## ?? License

MIT License - See LICENSE file for details

---

**Dibuat oleh:** Software Engineer menggunakan OOP best practices untuk Desktop Development dengan C# .NET 8
