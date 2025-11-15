# Old Implementation - OOP Demo

Folder ini berisi implementasi lama dari HarborMaster yang difokuskan untuk demonstrasi konsep Object-Oriented Programming (OOP).

## ⚠️ Status

**Folder ini di-exclude dari build** dan disimpan hanya sebagai **referensi**.

File-file di folder ini **TIDAK AKAN DI-COMPILE** saat build project utama.

## Perbedaan dengan Implementasi Baru

### Implementasi Lama (Folder Ini):
- **Tujuan**: Demo OOP concepts untuk pembelajaran
- **Pendekatan**: Class inheritance (CargoShip, PassengerShip, TankerShip extends Ship)
- **Pattern**: Object-oriented design
- **Database**: Minimal / tidak ada
- **Files**:
  - `ProgramExample.cs` - Contoh penggunaan OOP concepts
  - `HarborOperationsDemo.cs` - Demo workflow harbor operations
  - `Ship.cs`, `Berth.cs`, `Invoice.cs` - Models dengan inheritance
  - `Services.cs` - Service classes (DockingService, CargoHandlingService, dll)

### Implementasi Baru (Root Project):
- **Tujuan**: Production-ready application
- **Pendekatan**: Single model dengan database normalization
- **Pattern**: **MVP Architecture** (Model-View-Presenter)
- **Database**: Full Supabase/PostgreSQL integration
- **Structure**:
  - `Models/` - Data models (BaseModel dari Postgrest)
  - `Views/` - Windows Forms UI
  - `Presenters/` - UI logic mediators
  - `Services/` - Business logic
  - `Repositories/` - Data access layer
  - `Data/` - Database connection management

## Dokumentasi OOP

Untuk memahami konsep OOP yang diterapkan, lihat:
- `../README_OOP.md` - Overview konsep OOP
- `../OOP_IMPLEMENTATION_GUIDE.md` - Implementation guide
- `KONVERSI_WPF_TO_WINFORMS.md` - Konversi dari WPF ke Windows Forms

## Catatan

Implementasi baru (MVP) **tetap menggunakan prinsip OOP** seperti:
- **Encapsulation**: Models dengan validation, private fields
- **Separation of Concerns**: Layers (Models, Views, Presenters, Services, Repositories)
- **Abstraction**: Interfaces untuk Views (ILoginView, IMainView, dll)
- **Single Responsibility**: Setiap class punya tanggung jawab spesifik

Hanya saja implementasi baru menggunakan **architecture pattern yang lebih scalable** untuk production application.
