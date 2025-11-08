# HarborMaster - Implementasi Konsep OOP

## Ringkasan Implementasi

Implementasi ini mendemonstrasikan penerapan tiga pilar utama Object-Oriented Programming (OOP) dalam sistem manajemen pelabuhan (HarborMaster):

1. **Inheritance (Pewarisan)**
2. **Encapsulation (Enkapsulasi)**
3. **Polymorphism (Polimorfisme)**

---

## 1. INHERITANCE (Pewarisan)

### A. Ship Class Hierarchy

**Base Class: `Ship` (Abstract)**
- Class abstract yang menjadi dasar untuk semua jenis kapal
- Menyediakan properti dan method umum untuk semua kapal
- Menggunakan `protected` constructor untuk memaksa penggunaan derived classes

**Derived Classes:**

1. **`CargoShip`** - Kapal Kargo
   - Properties tambahan: `CargoCapacity`, `CargoType`
   - Khusus untuk mengangkut barang/kargo
   
2. **`PassengerShip`** - Kapal Penumpang
   - Properties tambahan: `PassengerCapacity`, `CrewCount`, `ShipClass`
   - Untuk kapal penumpang, ferry, dan cruise
   
3. **`TankerShip`** - Kapal Tanker
   - Properties tambahan: `LiquidCargoCapacity`, `LiquidType`, `RequiresSpecialHandling`
   - Untuk mengangkut cairan (minyak, LNG, chemical)

**Keuntungan:**
- Code reusability: properti dan method umum hanya ditulis sekali
- Extensibility: mudah menambah jenis kapal baru
- Type safety: compile-time checking untuk jenis kapal

### B. PortService Class Hierarchy

**Base Class: `PortService` (Abstract)**
- Class abstract untuk semua layanan pelabuhan
- Menyediakan struktur dasar untuk semua services

**Derived Classes:**

1. **`DockingService`** - Layanan Berlabuh
2. **`CargoHandlingService`** - Layanan Bongkar Muat
3. **`MaintenanceService`** - Layanan Perawatan
4. **`RefuelingService`** - Layanan Pengisian Bahan Bakar

---

## 2. ENCAPSULATION (Enkapsulasi)

### A. Data Hiding dengan Private Fields

```csharp
// Contoh di Ship.cs
private string _shipID;
private string _name;
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
```

**Keuntungan:**
- Data validation: memastikan data selalu valid
- Data integrity: mencegah nilai yang tidak valid
- Controlled access: hanya method tertentu yang bisa mengubah data

### B. Access Modifiers yang Tepat

- **`public`**: Untuk API yang harus diakses dari luar
- **`private`**: Untuk implementasi internal
- **`protected`**: Untuk member yang harus diakses oleh derived classes
- **`private set`**: Untuk properties read-only dari luar class

### C. Validation di Properties

Setiap property penting memiliki validasi:
- `ShipID`: tidak boleh kosong
- `Name`: tidak boleh kosong
- `Tonnage`: harus > 0
- `ServiceID`: tidak boleh kosong
- `BaseCost`: tidak boleh negatif

---

## 3. POLYMORPHISM (Polimorfisme)

### A. Method Overriding

**1. CalculateDockingFee()**

Setiap jenis kapal menghitung biaya berbeda:

```csharp
// CargoShip
public override decimal CalculateDockingFee()
{
    decimal baseFee = base.CalculateDockingFee();
    decimal cargoFee = CargoCapacity * 5;
    return baseFee + cargoFee;
}

// PassengerShip
public override decimal CalculateDockingFee()
{
    decimal baseFee = base.CalculateDockingFee();
    decimal passengerFee = PassengerCapacity * 2;
    if (ShipClass == "Cruise")
        passengerFee *= 1.5m;
    return baseFee + passengerFee;
}

// TankerShip
public override decimal CalculateDockingFee()
{
    decimal baseFee = base.CalculateDockingFee();
    decimal liquidFee = LiquidCargoCapacity * 8;
    if (RequiresSpecialHandling)
        liquidFee *= 1.8m;
    return baseFee + liquidFee;
}
```

**2. CalculateCost() di PortService**

Setiap jenis layanan menghitung biaya berbeda berdasarkan jenis kapal:

```csharp
// DockingService
public override decimal CalculateCost(Ship ship)
{
    decimal cost = ship.CalculateDockingFee(); // Polymorphic call
    cost += DurationHours * 50;
    if (IncludesPowerSupply)
        cost += DurationHours * 25;
    return cost;
}

// CargoHandlingService
public override decimal CalculateCost(Ship ship)
{
    decimal cost = BaseCost;
    cost += CargoWeight * 0.5m;
    
    // Type checking dengan pattern matching
    if (ship is CargoShip cargoShip)
    {
        cost += cargoShip.CargoCapacity * 0.2m;
        if (cargoShip.CargoType.Contains("Hazardous"))
            cost *= 1.5m;
    }
    
    return cost;
}
```

**3. GetPriorityLevel()**

Prioritas berbeda untuk setiap jenis kapal:
- CargoShip: 2-3 (lebih tinggi untuk hazardous cargo)
- PassengerShip: 3-4 (tinggi karena safety)
- TankerShip: 3-5 (tertinggi untuk LNG/Chemical)

**4. GetRequiredServices()**

Services yang diperlukan berbeda untuk setiap jenis kapal:
- Base ship: Basic Inspection, Documentation
- CargoShip: + Cargo Handling, Loading Equipment (+ Hazmat jika perlu)
- PassengerShip: + Terminal Access, Customs, Medical Screening
- TankerShip: + Liquid Handling, Pipeline, Environmental Check

### B. Abstract Methods

```csharp
public abstract string GetShipDetails();
public abstract string GetServiceDescription();
```

Memaksa setiap derived class untuk menyediakan implementasi spesifik.

---

## 4. FITUR TAMBAHAN

### A. ServiceManager Class

Class untuk mengelola multiple services dengan polymorphism:

```csharp
public decimal CalculateTotalCost(Ship ship)
{
    decimal total = 0;
    foreach (var service in _services)
    {
        total += service.CalculateCost(ship); // Polymorphic call
    }
    return total;
}
```

### B. HarborOperationsDemo Class

Demonstrasi lengkap penggunaan semua konsep OOP:

**Method-method demo:**
1. `DemonstrateDockingFeeCalculation()` - Polymorphism demo
2. `DemonstrateServiceManagement()` - Service polymorphism
3. `ProcessShipArrival()` - Complete workflow
4. `RunCompleteScenario()` - Full scenarios dengan berbagai ship types
5. `DemonstrateEncapsulation()` - Validation demo

---

## 5. CONTOH PENGGUNAAN

### Membuat Kapal

```csharp
// Cargo Ship
var cargoShip = new CargoShip(
    "CARGO-001", 
    "Pacific Trader", 
    "USA", 
    50000,      // tonnage
    30000,      // cargo capacity
    "Electronics"
);

// Passenger Ship
var cruiseShip = new PassengerShip(
    "CRUISE-001",
    "Ocean Dream",
    "Norway",
    70000,      // tonnage
    2500,       // passenger capacity
    800,        // crew count
    "Cruise"
);

// Tanker Ship
var tanker = new TankerShip(
    "TANK-001",
    "LNG Carrier",
    "Qatar",
    95000,      // tonnage
    60000,      // liquid capacity
    "LNG"
);
```

### Menghitung Biaya (Polymorphism)

```csharp
List<Ship> ships = new List<Ship> { cargoShip, cruiseShip, tanker };

foreach (var ship in ships)
{
    // Polymorphic call - each ship calculates differently
    decimal fee = ship.CalculateDockingFee();
    Console.WriteLine($"{ship.Name}: ${fee}");
}
```

### Mengelola Services

```csharp
var serviceManager = new ServiceManager();

// Add various services
serviceManager.AddService(new DockingService("DOCK-001", 1000, 48, true));
serviceManager.AddService(new CargoHandlingService("CARGO-001", 500, 30000, "Crane"));
serviceManager.AddService(new RefuelingService("FUEL-001", 100, 50000, "Diesel", 1.5m));

// Calculate total cost - polymorphic calculation
decimal total = serviceManager.CalculateTotalCost(cargoShip);

// Generate report
string report = serviceManager.GenerateServiceReport(cargoShip);
```

### Generate Invoice

```csharp
var invoice = Invoice.GenerateInvoice(cargoShip);

foreach (var service in services)
{
    invoice.AddService(service);
}

decimal total = invoice.CalculateTotal(); // Uses polymorphic CalculateCost
string details = invoice.GetInvoiceDetails();
```

---

## 6. MANFAAT IMPLEMENTASI OOP

### A. Maintainability
- Code terorganisir dengan baik
- Mudah menemukan dan memperbaiki bugs
- Separation of concerns

### B. Extensibility
- Mudah menambah jenis kapal baru
- Mudah menambah jenis service baru
- Tidak perlu mengubah existing code

### C. Reusability
- Base classes menyediakan functionality umum
- Derived classes hanya menambah yang spesifik
- Menghindari code duplication

### D. Type Safety
- Compile-time checking
- Intellisense support
- Reduced runtime errors

### E. Flexibility
- Polymorphism memungkinkan treatment berbeda untuk object berbeda
- Runtime type checking dengan pattern matching
- Dynamic behavior based on actual type

---

## 7. BEST PRACTICES YANG DITERAPKAN

1. **Abstract Classes untuk Base Classes**
   - Mencegah instantiasi langsung
   - Memaksa implementasi method penting

2. **Protected Constructor di Base Class**
   - Hanya derived classes yang bisa create instance
   - Enforce proper initialization

3. **Validation di Properties**
   - Data integrity
   - Fail-fast principle

4. **Virtual Methods untuk Extensibility**
   - Derived classes bisa override jika perlu
   - Default implementation tersedia

5. **Pattern Matching untuk Type Checking**
   - Modern C# syntax
   - Type-safe casting

6. **Separation of Concerns**
   - ServiceManager untuk service management
   - Invoice untuk billing
   - Ship classes untuk ship data

---

## 8. TESTING RECOMMENDATIONS

Untuk menguji implementasi:

```csharp
// Di Program.cs atau test file
var demo = new HarborOperationsDemo();

// Test 1: Polymorphism
demo.DemonstrateDockingFeeCalculation();

// Test 2: Service Management
demo.DemonstrateServiceManagement();

// Test 3: Encapsulation & Validation
demo.DemonstrateEncapsulation();

// Test 4: Complete Workflow
demo.RunCompleteScenario();
```

---

## KESIMPULAN

Implementasi ini mendemonstrasikan penerapan komprehensif dari tiga pilar OOP:

1. **Inheritance** - Hierarki Ship dan PortService yang jelas dan extensible
2. **Encapsulation** - Data protection dan validation yang robust
3. **Polymorphism** - Runtime behavior yang berbeda untuk tipe berbeda

Sistem ini siap untuk production use dan mudah untuk di-extend dengan fitur baru tanpa mengubah code yang sudah ada (Open-Closed Principle).
