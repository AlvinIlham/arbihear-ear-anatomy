# AR-Telinga

Aplikasi **Augmented Reality (AR)** berbasis Android untuk media pembelajaran anatomi telinga manusia. Aplikasi ini menampilkan model 3D telinga yang interaktif, video proses pendengaran, serta fitur kuis untuk menguji pemahaman pengguna.

---

## Daftar Isi

- [Fitur](#fitur)
- [Teknologi](#teknologi)
- [Prasyarat](#prasyarat)
- [Struktur Proyek](#struktur-proyek)
- [Cara Menjalankan](#cara-menjalankan)
- [Alur Aplikasi](#alur-aplikasi)
- [Deskripsi Scene](#deskripsi-scene)
- [Deskripsi Script](#deskripsi-script)
- [Build ke Android](#build-ke-android)

---

## Fitur

| Fitur | Keterangan |
|---|---|
| **AR Tracking** | Menampilkan model 3D telinga di atas marker menggunakan Vuforia |
| **Model 3D Interaktif** | Zoom (in/out) dan rotasi objek telinga dengan tombol tahan |
| **Animasi Telinga** | Play / Pause animasi proses telinga-full |
| **Video Pendengaran** | Memutar video *Proses Pendengaran* lewat tombol seru (!), toggle buka/tutup |
| **Kuis Pilihan Ganda** | Soal dengan 3 opsi (A/B/C), ikon benar/salah, dan navigasi antar soal |
| **Kuis Mencocokkan** | Drag & drop garis koneksi antar node dengan validasi jawaban |
| **Navigasi Scene** | Perpindahan halaman dan tombol keluar aplikasi |

---

## Teknologi

| Komponen | Versi |
|---|---|
| **Unity** | 2022.3.62f3 |
| **Platform Target** | Android |
| **AR SDK** | Vuforia Engine 11.1.3 |
| **UI** | Unity UGUI + TextMesh Pro 3.0.7 |
| **Video** | Unity VideoPlayer (UnityEngine.Video) |
| **Animasi** | Unity Animator |
| **Scripting** | C# (.NET) |

---

## Prasyarat

- Unity **2022.3.x LTS** dengan modul **Android Build Support** (termasuk Android SDK & NDK)
- **Vuforia Engine 11.1.3** (sudah disertakan di `Packages/com.ptc.vuforia.engine-11.1.3.tgz`)
- Android device dengan kamera (untuk fitur AR)
- Vuforia License Key (didaftarkan di [developer.vuforia.com](https://developer.vuforia.com))

---

## Struktur Proyek

```
AR-Telinga/
├── Assets/
│   ├── 3D Model/          # Model FBX/OBJ anatomi telinga
│   ├── Common/            # Aset bersama (material, prefab umum)
│   ├── Editor/            # Script dan tool khusus Editor
│   ├── Quiz/              # Aset untuk scene quiz
│   ├── Resources/         # Aset yang dimuat secara dinamis
│   ├── SamplesResources/  # Aset contoh / referensi
│   ├── Scenes/            # Semua scene Unity
│   ├── Script/            # Semua C# script
│   ├── StreamingAssets/   # Aset streaming (database Vuforia)
│   ├── TextMesh Pro/      # Font & shader TMP
│   ├── UIUX/              # Sprite, ikon, dan aset UI/UX
│   ├── Video/             # File video (Proses Pendengaran.mp4)
│   ├── Line_Material.mat  # Material untuk LineRenderer
│   └── Line_Prefab.prefab # Prefab garis koneksi
├── Packages/
│   └── manifest.json      # Daftar dependensi package
├── ProjectSettings/       # Pengaturan project Unity
└── QCAR/                  # Data tracking Vuforia
```

---

## Cara Menjalankan

1. **Clone / buka proyek** di Unity Hub dengan Unity versi `2022.3.x LTS`.
2. Tunggu Unity mengimpor semua aset dan package.
3. Masukkan **Vuforia License Key** di:
   `Edit → Project Settings → Vuforia Configuration → App License Key`
4. Buka scene awal:
   `Assets/Scenes/tampilan_awal.unity`
5. Klik tombol **Play** di Unity Editor untuk menguji di Editor, atau lakukan [Build ke Android](#build-ke-android).

---

## Alur Aplikasi

```
tampilan_awal
     ↓
menu_utama
  ├──→ petunjuk          (cara penggunaan)
  ├──→ tentang1 → tentang2   (info aplikasi)
  ├──→ AR                (scene AR utama)
  │       ├─ Zoom In / Zoom Out
  │       ├─ Rotate kiri / kanan
  │       ├─ Play / Pause animasi
  │       └─ Tombol (!) → VideoPanel (Proses Pendengaran.mp4)
  └──→ quiz              (pilih jenis kuis)
          ├──→ quiz_objektif   (pilihan ganda)
          └──→ match_quiz      (mencocokkan)
```

---

## Deskripsi Scene

| Scene | Fungsi |
|---|---|
| `tampilan_awal` | Splash screen / layar pembuka |
| `menu_utama` | Menu navigasi utama |
| `petunjuk` | Halaman petunjuk penggunaan aplikasi |
| `tentang1` / `tentang2` | Halaman informasi tentang aplikasi |
| `AR` | Scene utama AR — menampilkan model 3D telinga interaktif dan video |
| `quiz` | Halaman pilihan jenis kuis |
| `quiz_objektif` | Kuis pilihan ganda (A/B/C) tentang anatomi telinga |
| `match_quiz` | Kuis mencocokkan dengan drag & drop garis |
| `Quiz Test` | Scene pengujian / sandbox kuis |
| `uji_coba` | Scene pengujian fitur umum |

---

## Deskripsi Script

### `ObjectController.cs`
Mengontrol semua interaksi pada model 3D di scene AR.

| Field (Inspector) | Fungsi |
|---|---|
| `telingaLuar/Tengah/Dalam/Full` | Referensi GameObject model telinga |
| `btnZoomIn / btnZoomOut` | Tahan tombol → zoom objek |
| `btnRotate1 / btnRotate2` | Tahan tombol → rotasi sumbu Y |
| `btnPlay` | Klik → toggle play/pause animasi |
| `btnSeru` | Klik → toggle tampil/sembunyikan video pendengaran |
| `videoPanel` | Panel UI yang berisi VideoPlayer |
| `videoPlayer` | Komponen VideoPlayer untuk memutar video |
| `btnCloseVideo` | Tombol X untuk menutup panel video |

---

### `QuizManager.cs`
Mengelola soal-soal pilihan ganda.

- Mendukung banyak soal (`Soal[]`) masing-masing dengan panel, 3 opsi (A/B/C), ikon benar/salah, dan tombol navigasi.
- Klik jawaban → tampilkan ikon benar/salah selama 1 detik, lalu ikon hilang.
- Tombol **Next / Previous** untuk navigasi antar soal.
- Tombol **Kembali** untuk kembali ke scene yang dikonfigurasi.

---

### `LevelManager.cs`
Utilitas navigasi scene.

```csharp
LoadToScene("nama_scene"); // pindah ke scene tertentu
QuitGame();               // keluar aplikasi
```

---

### `ConnectionManager.cs` + `UINode.cs` + `ConnectionLine.cs` + `UILine.cs`
Sistem mencocokkan untuk `match_quiz`:

- Pengguna **drag** dari satu node ke node lain untuk membuat garis koneksi.
- `ConnectionManager` memvalidasi pasangan berdasarkan `correctAnswers` yang dikonfigurasi di Inspector.
- Ikon ✅/❌ ditampilkan berdasarkan hasil validasi.
- Garis divisualisasikan dengan `LineRenderer` (dunia) atau `UILine` (UI canvas).

---

### `Draggable.cs` + `DropTarget.cs`
Sistem drag & drop sederhana berbasis `LineRenderer`:

- Drag item → garis menempel dari posisi awal ke posisi kursor.
- Drop ke `DropTarget` yang benar → garis dikunci ke target.

---

### `Card.cs`
Komponen kartu interaktif dengan highlight saat diklik (`IPointerDownHandler`).

---

## Build ke Android

1. Buka `File → Build Settings`.
2. Pilih platform **Android**, klik **Switch Platform**.
3. Pastikan semua scene sudah ditambahkan di daftar **Scenes In Build**:
   - `tampilan_awal`, `menu_utama`, `petunjuk`, `tentang1`, `tentang2`, `AR`, `quiz`, `quiz_objektif`, `match_quiz`
4. Di `Player Settings`:
   - **Company Name / Product Name**: sesuaikan
   - **Minimum API Level**: Android 8.0 (API 26) atau lebih tinggi (rekomendasi Vuforia)
   - **Scripting Backend**: IL2CPP
   - **Target Architectures**: ARM64
5. Klik **Build** atau **Build And Run**.

---

## Lisensi

Proyek ini dibuat untuk keperluan pendidikan / tugas akhir. Silakan sesuaikan lisensi sesuai kebutuhan institusi atau pengembang.
