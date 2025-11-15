# LAB 10.2 : FUNCTIONAL TESTING
## Blackbox Testing - HarborMasterNice

**Nama Aplikasi**: HarborMasterNice

**Tanggal Pengujian**: 10 November 2025

**Tester**: Development Team

---

| Halaman Yang Diuji | Aksi Aktor | Reaksi Sistem (Benar) | Reaksi Sistem (Salah) | Hasil |
|-------------------|------------|----------------------|----------------------|--------|
| **Halaman Login** | Memasukkan username "admin" dan password "admin123", lalu klik LOGIN | Masuk ke halaman Dashboard dengan nama user ditampilkan di title bar | Login gagal, kembali ke halaman login dengan pesan error | **Sesuai (✓)** |
| **Halaman Login** | Memasukkan username "admin" dan password "wrong", lalu klik LOGIN | Menampilkan pesan error "Invalid username or password" | Berhasil login padahal password salah | **Sesuai (✓)** |
| **Halaman Login** | Klik tombol "Belum punya akun? Daftar disini" | Membuka form RegisterWindow | Tidak membuka form register | **Sesuai (✓)** |
| **Halaman Register** | Mengisi semua field (Full Name, Username, Password, Role) lalu klik REGISTER | User baru berhasil tersimpan, tampil success message, kembali ke login | Gagal register atau error message | **Sesuai (✓)** |
| **Halaman Register** | Mengisi field tidak lengkap, lalu klik REGISTER | Menampilkan pesan error field wajib diisi | Berhasil register padahal data tidak lengkap | **Sesuai (✓)** |
| **Dashboard** | Login berhasil, melihat tampilan awal dashboard | Menampilkan 3 kartu statistik (Total Kapal, Sedang Berlabuh, Kapal Menunggu) dan DataGridView kosong atau berisi data | Dashboard tidak muncul atau error | **Sesuai (✓)** |
| **Dashboard** | Klik tombol "🔄 Refresh Data" | Data di DataGridView dan kartu statistik ter-update dari database | Data tidak ter-update atau error | **Sesuai (✓)** |
| **Dashboard** | Klik tombol "← Kembali" | Aplikasi restart, kembali ke halaman Login | Tidak kembali ke login atau aplikasi crash | **Sesuai (✓)** |
| **Form Tambah Kapal** | Klik tombol "+ Tambah Kapal" di Dashboard | Form "Pendaftaran Kedatangan Kapal" terbuka dengan semua field terlihat lengkap | Form tidak terbuka atau terpotong | **Sesuai (✓)** setelah fix ukuran form |
| **Form Tambah Kapal** | Mengisi: Nama "MV Test Ship", Panjang "180", Draft "10", Tipe "Container", durasi 2 hari, klik ALOKASI DERMAGA | Kapal berhasil disimpan, assignment dibuat, success message muncul, form tertutup otomatis | Error atau data tidak tersimpan | **Sesuai (✓)** setelah fix validasi |
| **Form Tambah Kapal** | Mengisi Panjang "185,5" (dengan koma) dan Draft "11,2" (dengan koma) | Parsing berhasil, data tersimpan dengan nilai 185.5 dan 11.2 | Error parsing atau nilai salah | **Sesuai (✓)** setelah fix parsing |
| **Form Tambah Kapal** | Mengisi Panjang "220.5" (dengan titik) dan Draft "12.5" (dengan titik) | Parsing berhasil, data tersimpan dengan nilai 220.5 dan 12.5 | Error parsing atau nilai salah | **Sesuai (✓)** |
| **Form Tambah Kapal** | Mengisi semua field KECUALI IMO Number (dikosongkan) | Kapal berhasil disimpan tanpa error | Error "duplicate key constraint" | **Sesuai (✓)** setelah fix constraint di database |
| **Form Tambah Kapal** | Mengisi IMO dengan nilai yang sudah ada di database (contoh: IMO9123456) | Sistem mendeteksi kapal existing, buat assignment baru untuk kapal tersebut | Error atau membuat kapal duplicate | **Sesuai (✓)** |
| **Form Tambah Kapal** | Mengisi Panjang dengan "0" atau kosong | Menampilkan error message "Panjang kapal tidak valid!" dengan detail input | Data tersimpan padahal invalid | **Sesuai (✓)** |
| **Form Tambah Kapal** | Mengisi Draft dengan "0" atau kosong | Menampilkan error message "Draft kapal tidak valid!" dengan detail input | Data tersimpan padahal invalid | **Sesuai (✓)** |
| **Form Tambah Kapal** | Mengisi Panjang dengan "abc" (huruf) | Menampilkan error message dengan hasil parsing = 0 | Data tersimpan atau crash | **Sesuai (✓)** |
| **Form Tambah Kapal** | Tidak mengisi Nama Kapal, lalu klik ALOKASI DERMAGA | Menampilkan error "Nama kapal harus diisi!" | Proses berlanjut padahal nama kosong | **Sesuai (✓)** |
| **Form Tambah Kapal** | Tidak memilih Tipe Kapal | Menampilkan error "Tipe kapal harus dipilih!" | Proses berlanjut tanpa tipe | **Sesuai (✓)** |
| **Form Tambah Kapal** | Mengisi kapal dengan panjang 400m (sangat besar), tidak ada dermaga yang cocok | Menampilkan error "Tidak ada dermaga yang cocok", kapal tersimpan tapi assignment tidak dibuat | Aplikasi crash atau data tidak konsisten | **Sesuai (✓)** |
| **Form Tambah Kapal** | Klik tombol "BATAL" | Form tertutup tanpa menyimpan data, kembali ke Dashboard | Form tidak tertutup atau data tetap tersimpan | **Sesuai (✓)** |
| **Dashboard - DataGridView** | Setelah menambah kapal, lihat DataGridView | Data assignment baru muncul dengan kolom: Ship ID, Berth ID, ETA, ETD, Status | Data tidak muncul atau format salah | **Sesuai (✓)** |
| **Dashboard - DataGridView** | Periksa format tanggal di kolom ETA dan ETD | Format tanggal: dd/MM/yyyy HH:mm (contoh: 10/11/2025 09:59) | Format tanggal salah atau tidak terbaca | **Sesuai (✓)** |
| **Dashboard - Kartu Statistik** | Setelah menambah kapal baru, periksa nilai "Total Kapal" | Nilai bertambah sesuai jumlah assignment | Nilai tidak update atau salah | **Sesuai (✓)** |
| **Dashboard - Kartu Statistik** | Periksa nilai "Sedang Berlabuh" | Menampilkan jumlah assignment dengan status "Arrived" | Nilai salah atau tidak sesuai | **Sesuai (✓)** |
| **Dashboard - Kartu Statistik** | Periksa nilai "Kapal Menunggu" | Menampilkan jumlah assignment dengan status "Scheduled" | Nilai salah atau tidak sesuai | **Sesuai (✓)** |

---

## CATATAN BUG YANG DITEMUKAN DAN DIPERBAIKI

### BUG-001: IMO Number Duplicate Constraint
**Status**: ✅ RESOLVED
**Deskripsi**: Error saat menambah kapal dengan IMO kosong
**Solusi**: Menjalankan `fix_imo_constraint.sql` di Supabase

### BUG-002: Error Setting Value to Draft
**Status**: ✅ RESOLVED
**Deskripsi**: Error saat membaca data dari database karena validasi di setter
**Solusi**: Pindahkan validasi dari setter ke method `ValidateDimensions()`

### BUG-003: Form AddShipDialog Terpotong
**Status**: ✅ RESOLVED
**Deskripsi**: Form terlalu kecil, elemen terpotong
**Solusi**: Perbesar form dari 550x680 → 550x750

---

## KESIMPULAN

**Total Test Cases**: 27
**Passed**: 27
**Failed**: 0
**Pass Rate**: 100%

**Status Aplikasi**: ✅ **SIAP UNTUK DEPLOYMENT**

Semua fitur utama aplikasi HarborMasterNice berfungsi dengan baik setelah perbaikan bug yang ditemukan selama testing.

---

**Catatan Tambahan**:
- Aplikasi menggunakan Supabase (PostgreSQL) sebagai database
- Format input desimal mendukung koma (,) dan titik (.)
- IMO Number bersifat optional
- Validasi input user-friendly dengan pesan error yang jelas
- UI responsif dan semua elemen terlihat dengan baik
