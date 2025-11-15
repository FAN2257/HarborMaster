# 🚢 HARBORMASTER PROJECT STATUS

**Last Updated:** 2025-01-12
**Version:** 2.0 (Major Refactoring)
**Team:** 2 Developers
**Status:** Active Development - Phase 1

---

## 📖 PROJECT OVERVIEW

**HarborMasterNice** adalah aplikasi Windows Forms untuk manajemen operasional pelabuhan yang mengelola:
- Pendaftaran kapal milik ship owner
- Permintaan docking (berth allocation request)
- Approval workflow oleh operator pelabuhan
- Alokasi dermaga otomatis dengan conflict detection
- Penghitungan invoice & pricing dinamis
- Monitoring & analytics dashboard

### **Key Business Process:**
```
Ship Owner                    Operator                    System
    |                            |                           |
    | 1. Register & Add Ships    |                           |
    |---------------------------->|                           |
    |                            |                           |
    | 2. Request Docking         |                           |
    |--------------------------->|                           |
    |    (ETA, ETD, Ship)        |                           |
    |                            |                           |
    |                            | 3. Review Request         |
    |                            |-------------------------->|
    |                            |                           |
    |                            | 4. Allocate Berth         |
    |                            |   (Auto-suggest suitable) |
    |                            |<--------------------------|
    |                            |                           |
    | 5. Approval Notification   |                           |
    |<---------------------------|                           |
    |                            |                           |
    | 6. Ship Arrives & Departs  |                           |
    |--------------------------->|                           |
    |                            |                           |
    | 7. Invoice Generated       |                           |
    |<---------------------------------------------------|
    |                            |                           |
    | 8. Download PDF Invoice    |                           |
    |                            |                           |
```

---

## 🛠️ TEKNOLOGI YANG DIGUNAKAN

### **Frontend**
- **Framework:** .NET 8.0 Windows Forms
- **Language:** C# 12
- **UI Pattern:** MVP (Model-View-Presenter)
- **Charts:** LiveCharts 2 (untuk visualisasi data)
- **PDF Export:** iTextSharp / PdfSharp (upcoming)

### **Backend/Database**
- **Database:** Supabase (PostgreSQL Cloud)
- **ORM:** Supabase-csharp v0.16.2 + Postgrest
- **Connection:** Hard-coded URL & API Key (development mode)

### **Architecture Pattern**
- **Design Pattern:** Repository Pattern + Service Layer
- **Separation:** 6-layer architecture
  ```
  Views (UI)
    ↓
  Presenters (UI Logic)
    ↓
  Services (Business Logic)
    ↓
  Repositories (Data Access)
    ↓
  Models (Entities)
    ↓
  Data Layer (Supabase Client)
  ```

### **Dependencies**
```xml
<PackageReference Include="supabase-csharp" Version="0.16.2" />
<PackageReference Include="LiveChartsCore.SkiaSharpView.WinForms" Version="2.0.0-rc2" /> <!-- Upcoming -->
<PackageReference Include="iTextSharp.LGPLv2.Core" Version="3.4.0" /> <!-- Upcoming -->
```

---

## ✅ YANG SUDAH BERHASIL (COMPLETED)

### **Phase 0: Initial Implementation (Before Refactoring)**

#### **1. Database Schema (Original)**
- ✅ `users` table - User authentication
- ✅ `ships` table - Ship registry
- ✅ `berths` table - Berth/dock information
- ✅ `berth_assignments` table - Ship-berth assignments
- ✅ `invoices` table - Billing records
- ✅ `ship_type_multipliers` table - Dynamic pricing by type
- ✅ `size_multipliers` table - Dynamic pricing by size

#### **2. Models Layer** (8 Models)
- ✅ `User` - dengan UserRole enum (Operator, HarborMaster)
- ✅ `Ship` - dengan OOP principles (encapsulation, polymorphism, validation)
  - Virtual methods: `CalculateSpecialFee()`, `GetPriorityLevel()`, `GetRequiredServices()`, `GetMaxDockingDuration()`, `CanDockAt()`
- ✅ `Berth` - Dermaga dengan capacity limits
- ✅ `BerthAssignment` - Ship-berth schedule linkage
- ✅ `Invoice` - Billing information
- ✅ `ShipTypeMultiplier` - Type-based pricing
- ✅ `SizeMultiplier` - Size-based pricing

#### **3. Repository Layer** (8 Repositories)
- ✅ `BaseRepository<T, K>` - Generic CRUD operations
- ✅ `ShipRepository` - Ship data access
- ✅ `BerthRepository` - Custom: `GetPhysicallySuitableBerths()`
- ✅ `BerthAssignmentRepository` - Custom: `HasScheduleCollision()`, `GetActiveSchedule()`
- ✅ `UserRepository` - Custom: `GetByUsername()`
- ✅ `InvoiceRepository` - Basic CRUD
- ✅ `ShipTypeMultiplierRepository` - Custom: `GetMultiplierValue()`
- ✅ `SizeMultiplierRepository` - Custom: `GetMultiplierValue()`

#### **4. Service Layer** (4 Services)
- ✅ `AuthenticationService` - Registration & login (plain-text password for dev)
- ✅ `PortService` - Core harbor operations
  - `TryAllocateBerth()` - Berth allocation with conflict detection
  - `ProcessShipArrival()` - Advanced arrival processing with polymorphism
  - `GetActiveScheduleAsync()` - Current schedule
- ✅ `PricingService` - Invoice calculation
  - Formula: `(base_rate × days × size_multiplier × type_multiplier) + special_fee`
  - `GetPricingBreakdown()` - Detailed cost breakdown
- ✅ `NotificationService` - UI dialogs abstraction

#### **5. Presenter Layer** (4 Presenters)
- ✅ `LoginPresenter` - Login orchestration
- ✅ `RegisterPresenter` - User registration with validation
- ✅ `MainPresenter` - Dashboard operations
- ✅ `ShipArrivalPresenter` - Ship arrival processing

#### **6. View Layer** (4 Windows + 4 Interfaces)
- ✅ `LoginWindow` (implements `ILoginView`) - Custom UI dengan rounded panels
- ✅ `RegisterWindow` (implements `IRegisterView`) - Registration form
- ✅ `MainWindow` (implements `IMainView`) - Dashboard dengan:
  - 3 metric cards (Total Kapal, Sedang Berlabuh, Kapal Menunggu)
  - DataGridView untuk schedule
  - Refresh button
- ✅ `AddShipDialog` (implements `IAddShipDialog`) - Ship arrival form dengan:
  - Ship data input (name, IMO, length, draft, type)
  - ETA/ETD scheduling dengan auto-calculation
  - Decimal parsing (culture-aware)

#### **7. Core Features**
- ✅ User authentication (username/password)
- ✅ Role-based permissions (Operator vs HarborMaster)
- ✅ Ship registration dengan validation
- ✅ Automatic berth allocation
- ✅ Schedule conflict detection (time-based)
- ✅ Physical suitability check (length/draft matching)
- ✅ Dynamic pricing system
- ✅ Dashboard metrics (real-time counts)

#### **8. Application Entry Point**
- ✅ `Program.cs` - Login flow → Main window based on login success

---

## 🔄 YANG SEDANG TERJADI (IN PROGRESS)

### **Phase 1: Database Refactoring** (CURRENT)

#### **Task 1: Database Schema Update** ✅ JUST COMPLETED
- ✅ **BERHASIL!** Menambahkan `ships.owner_id` column (ship ownership)
- ✅ **BERHASIL!** Menambahkan `berths.status` column (Available/Occupied/Maintenance/Damaged)
- ✅ **BERHASIL!** Membuat table `docking_requests` dengan:
  - Ship & owner reference
  - Requested ETA/ETD
  - Cargo type & special requirements
  - Status workflow (Pending → Approved/Rejected)
  - Processed by operator tracking
  - Link to berth_assignment setelah approved

**File Created:**
- `database_migration_001.sql` - SQL migration script yang telah dieksekusi

**Next Immediate Tasks:**
1. Task 2: Create `DockingRequest` model (C#)
2. Task 3: Update `Ship` model - add `OwnerId`
3. Task 4: Update `Berth` model - add `Status`
4. Task 5: Update `UserRole` enum - add `ShipOwner` role

---

## 🎯 UPCOMING TASKS (PLANNED)

### **Phase 1: Foundation** (Remaining Tasks)
**Estimasi:** 1-2 jam

- [ ] **Task 2:** Create DockingRequest model
- [ ] **Task 3:** Update Ship model (add OwnerId property)
- [ ] **Task 4:** Update Berth model (add Status property)
- [ ] **Task 5:** Update UserRole enum (add ShipOwner = 0)

---

### **Phase 2: Repository & Service Layer**
**Estimasi:** 4-5 jam

#### **Repositories**
- [ ] **Task 6:** Create `DockingRequestRepository`
  - `GetByOwner(int ownerId)` - Ship owner's requests
  - `GetPendingRequests()` - Operator view
  - `GetByStatus(string status)` - Filter by status
- [ ] **Task 7:** Update `ShipRepository`
  - `GetShipsByOwner(int ownerId)` - Owner's ships only
- [ ] **Task 8:** Update `BerthRepository`
  - `GetByStatus(string status)` - Filter by status
  - `UpdateStatus(int berthId, string newStatus)` - Status management

#### **Services**
- [ ] **Task 9:** Create `DockingRequestService`
  - `CreateRequest(int shipId, DateTime eta, DateTime etd, ...)` - Ship owner submit
  - `ApproveRequest(int requestId, int berthId, int operatorId)` - Operator approve
  - `RejectRequest(int requestId, int operatorId, string reason)` - Operator reject
  - `CancelRequest(int requestId, int ownerId)` - Owner cancel
- [ ] **Task 10:** Update `PortService`
  - Integrate with DockingRequestService
  - Refactor allocation to work with approved requests
- [ ] **Task 11:** Create `BerthManagementService`
  - `SetBerthStatus(int berthId, string status, string notes)` - Mark maintenance/damaged
  - `GetBerthOccupancy()` - Real-time status for charts
- [ ] **Task 12:** Create `InvoiceService`
  - `GenerateInvoice(int assignmentId)` - Auto-generate after departure
  - `GetInvoicesByOwner(int ownerId)` - Owner's billing history

---

### **Phase 3A: Ship Owner UI**
**Estimasi:** 6-8 jam

- [ ] **Task 13:** Update `RegisterWindow` - Add role selection dropdown (ShipOwner/Operator/HarborMaster)
- [ ] **Task 14:** Create `MyShipsWindow`
  - DataGridView: Ship Name | IMO | Type | Length | Draft
  - Buttons: [Add Ship] [Edit Ship] [Delete Ship] [Request Docking]
  - Filter: By ship type
- [ ] **Task 15:** Create `AddEditShipDialog`
  - Unified form untuk Add & Edit mode
  - Validation: Required fields, dimension limits
  - Permission check: Only owner dapat edit
- [ ] **Task 16:** Create `DockingRequestDialog`
  - ComboBox: Select ship (from owner's ships)
  - DateTimePicker: ETA & ETD
  - TextBox: Cargo type, special requirements
  - Button: [Submit Request]
- [ ] **Task 17:** Create `MyRequestsWindow`
  - DataGridView: Request ID | Ship | ETA-ETD | Status | Berth (if approved)
  - Color coding: Pending (yellow), Approved (green), Rejected (red)
  - Actions: [Cancel] untuk pending requests
- [ ] **Task 18:** Create `MyInvoicesWindow`
  - DataGridView: Invoice # | Ship | Period | Total | Paid Status
  - Buttons: [View Details] [Download PDF] [Mark as Paid]

---

### **Phase 3B: Operator UI**
**Estimasi:** 6-8 jam

- [ ] **Task 19:** Create `OperatorDashboard`
  - Metric cards: Pending Requests (badge), Active Berths, Today's Arrivals/Departures
  - Quick stats: Total Revenue, Occupancy Rate
  - Navigation: [Pending Requests] [Manage Berths] [Schedule] [Analytics]
- [ ] **Task 20:** Create `PendingRequestsWindow`
  - DataGridView: Request # | Ship Owner | Ship | Type | ETA-ETD | Days
  - Buttons per row: [Allocate] [Reject]
  - Auto-refresh setiap 30 detik
- [ ] **Task 21:** Create `AllocateBerthDialog`
  - Show request details (Ship info, requested dates)
  - List suitable berths dengan availability status
  - Visual timeline: Show conflicts if any
  - ComboBox: Select berth
  - Button: [Confirm Allocation]
- [ ] **Task 22:** Create `BerthManagementWindow`
  - DataGridView: Berth Name | Location | Max Size | Status | Current Ship | Available From
  - Buttons: [Add Berth] [Edit] [Mark Maintenance]
  - Filter: By status (All/Available/Occupied/Maintenance)
- [ ] **Task 23:** Create `EditBerthDialog`
  - TextBox: Berth name, location, max_length, max_draft, base_rate
  - ComboBox: Status (Available/Maintenance/Damaged)
  - TextBox: Notes (maintenance reason, etc.)
- [ ] **Task 24:** Create `ScheduleWindow`
  - Visual timeline/Gantt chart untuk semua berths
  - X-axis: Date range (this week/month)
  - Y-axis: Berth names
  - Bars: Ship assignments dengan color by status

---

### **Phase 4: Visual Enhancements & Charts**
**Estimasi:** 4-6 jam

- [ ] **Task 25:** DataGridView row coloring
  - Status-based colors di semua list views
  - Alternate row background untuk readability
- [ ] **Task 26:** Add filter controls
  - Ship type filter (Container, Tanker, etc.)
  - Date range picker (ETA between...)
  - Status filter (Pending/Approved/Rejected)
  - Search textbox (ship name/IMO)
- [ ] **Task 27:** Install LiveCharts library
  ```bash
  dotnet add package LiveChartsCore.SkiaSharpView.WinForms --version 2.0.0-rc2
  ```
- [ ] **Task 28:** Create `BerthOccupancyChart`
  - Pie chart: Available vs Occupied vs Maintenance
  - Real-time data dari BerthManagementService
  - Click-to-filter (show only occupied berths)
- [ ] **Task 29:** Create `PerformanceChart`
  - Line chart: Daily revenue (last 30 days)
  - Column chart: Berth utilization by month
  - Data source: Historical berth_assignments
- [ ] **Task 30:** Create `AnalyticsWindow`
  - Combine all charts dalam satu dashboard
  - Date range selector untuk historical analysis
  - Export chart as image

---

### **Phase 5: Export & PDF**
**Estimasi:** 3-4 jam

- [ ] **Task 31:** Implement PDF export
  - Install iTextSharp: `dotnet add package iTextSharp.LGPLv2.Core`
  - Create `PdfExportService`
  - Template: Invoice dengan logo, header, line items, total
- [ ] **Task 32:** Add export button to MyInvoicesWindow
  - Button: [Download as PDF]
  - Save file dialog
  - Auto-open PDF after generation

---

### **Phase 6: Polish & Testing**
**Estimasi:** 4-6 jam

- [ ] **Task 33:** Create seed data script
  - 3 users (1 ShipOwner, 1 Operator, 1 HarborMaster)
  - 5 berths (different sizes)
  - 10 ships (distributed across 2 owners)
  - 5 docking requests (mix of Pending/Approved)
  - 3 active assignments
- [ ] **Task 34:** Update Program.cs
  - Route based on UserRole:
    - ShipOwner → MyShipsWindow
    - Operator → OperatorDashboard
    - HarborMaster → OperatorDashboard (with extra permissions)
- [ ] **Task 35:** Input validation polish
  - Required field indicators (red asterisk)
  - Real-time validation feedback
  - Disable submit button until valid
- [ ] **Task 36:** Delete confirmation dialogs
  - "Are you sure you want to delete ship X?"
  - Permission check: Only owner can delete their ships
- [ ] **Task 37:** Loading indicators
  - Progress spinner during async operations
  - Disable buttons during loading
  - Success/error toast notifications
- [ ] **Task 38:** End-to-end testing
  - Test complete workflow manually
  - Document bugs & edge cases
- [ ] **Task 39:** Screenshots
  - Capture setiap screen untuk documentation
  - Annotate key features
- [ ] **Task 40:** Demo video
  - Record 2-3 menit video
  - Narasi: "Ship owner requests docking → Operator approves → Invoice generated"
- [ ] **Task 41:** Unit tests
  - xUnit project setup
  - Tests untuk:
    - `PortService.TryAllocateBerth()`
    - `BerthAssignmentRepository.HasScheduleCollision()`
    - `PricingService.CalculateBerthCost()`
    - `DockingRequestService.ApproveRequest()`

---

## 📊 PROJECT STATISTICS

### **Code Metrics (Current)**
- **Total Files:** 35+ C# files
- **Lines of Code:** ~3,500+ LOC
- **Models:** 8
- **Repositories:** 8
- **Services:** 4
- **Presenters:** 4
- **Views:** 4 Forms + 4 Interfaces

### **Completion Status**
```
Phase 0 (Initial):        ████████████████████ 100% COMPLETE
Phase 1 (Foundation):     ████░░░░░░░░░░░░░░░░  20% (Task 1/5 done)
Phase 2 (Services):       ░░░░░░░░░░░░░░░░░░░░   0%
Phase 3A (Ship Owner UI): ░░░░░░░░░░░░░░░░░░░░   0%
Phase 3B (Operator UI):   ░░░░░░░░░░░░░░░░░░░░   0%
Phase 4 (Charts):         ░░░░░░░░░░░░░░░░░░░░   0%
Phase 5 (Export):         ░░░░░░░░░░░░░░░░░░░░   0%
Phase 6 (Polish):         ░░░░░░░░░░░░░░░░░░░░   0%

Overall Progress:         ██████░░░░░░░░░░░░░░  28%
```

### **Timeline**
- **Start Date:** 2025-01-11 (Initial implementation)
- **Refactoring Start:** 2025-01-12
- **Estimated Completion:** 2025-01-19 (7-8 days dari sekarang)
- **Target Demo Date:** TBD

---

## 🔑 KEY ARCHITECTURAL DECISIONS

### **1. Supabase vs EF Core**
**Decision:** Tetap menggunakan Supabase (PostgreSQL cloud)
**Reason:**
- ✅ Already implemented & working
- ✅ No need to manage local database
- ✅ Built-in authentication (future use)
- ✅ Real-time subscriptions (future use)
- ❌ Migration ke EF Core butuh complete rewrite

### **2. MVP Pattern**
**Decision:** Model-View-Presenter dengan interface-based Views
**Reason:**
- ✅ Clean separation of concerns
- ✅ Testable presenters (without UI dependency)
- ✅ Reusable business logic
- ✅ Clear data flow: View → Presenter → Service → Repository

### **3. Request-Approval Workflow**
**Decision:** Ship owners submit requests, operators approve (not direct allocation)
**Reason:**
- ✅ More realistic harbor operations
- ✅ Better control for port authority
- ✅ Audit trail (who approved what)
- ✅ Prevents conflicts from concurrent allocations

### **4. Ownership Model**
**Decision:** Ships belong to specific users (owners)
**Reason:**
- ✅ Multi-tenant support
- ✅ Permission control (edit own ships only)
- ✅ Personalized dashboards
- ✅ Invoice tracking per owner

### **5. Dynamic Pricing**
**Decision:** Multiplier-based pricing stored in database
**Reason:**
- ✅ No code changes needed for price updates
- ✅ Flexible pricing rules
- ✅ Historical pricing tracking
- ✅ Ship type & size-based differentiation

---

## 🚨 KNOWN ISSUES & LIMITATIONS

### **Security (Development Mode)**
- ⚠️ **Plain-text passwords** - BCrypt hashing commented out
- ⚠️ **Hard-coded credentials** - Supabase URL & API key in code
- ⚠️ **No HTTPS enforcement** - Development only

### **Features Not Yet Implemented**
- ❌ Actual arrival/departure time recording
- ❌ Status updates beyond "Scheduled"
- ❌ Email notifications
- ❌ Payment processing integration
- ❌ Historical analytics/reporting
- ❌ Berth availability calendar view
- ❌ Ship tracking integration (MarineTraffic API)

### **UI/UX**
- ⚠️ No responsive design (fixed window sizes)
- ⚠️ Limited error messages (some are technical)
- ⚠️ No auto-refresh (manual refresh required)
- ⚠️ No keyboard shortcuts

---

## 📚 DOCUMENTATION FILES

### **Created Documentation**
1. `CLAUDE.md` - Project overview & architecture guide
2. `PROJECT_STATUS.md` - This file (progress tracking)
3. `database_migration_001.sql` - Database schema migration

### **Planned Documentation**
4. `README.md` - GitHub front page dengan screenshots
5. `SETUP.md` - Installation & configuration guide
6. `API.md` - Service layer documentation
7. `DEMO_SCRIPT.md` - Step-by-step demo guide

---

## 🎯 SUCCESS CRITERIA

### **Minimum Viable Product (MVP)**
- [x] User authentication & authorization
- [x] Ship registration dengan ownership
- [ ] Docking request submission (Ship Owner)
- [ ] Request approval workflow (Operator)
- [ ] Automatic berth allocation
- [ ] Schedule conflict detection
- [ ] Invoice generation
- [ ] Basic dashboard

### **Demo Ready**
- [ ] MVP features +
- [ ] Visual status indicators (colors)
- [ ] Charts (berth occupancy)
- [ ] PDF invoice export
- [ ] Seed data untuk demo flow
- [ ] Screenshots & demo video

### **Production Ready** (Optional)
- [ ] Demo Ready +
- [ ] Unit tests (80%+ coverage)
- [ ] Input validation complete
- [ ] Error handling comprehensive
- [ ] Security hardening (BCrypt, env variables)
- [ ] User manual / help documentation

---

## 👥 TEAM ROLES

**Developer 1 (Primary):** Full-stack development
**Developer 2 (AI Assistant - Claude):** Code generation, architecture guidance, debugging

---

## 📞 CONTACT & RESOURCES

- **Supabase Project:** https://vpltuzpbzqksvuxsuwle.supabase.co
- **GitHub Repo:** (TBD)
- **Documentation:** See files listed above

---

**END OF STATUS REPORT**

*Document akan di-update setelah setiap phase completion*
