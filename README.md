# Kamera dan Perspektif

Laboratorium kamera 3D untuk presentasi Grafika Komputer, Kelompok 10. Bandingkan dua proyeksi pada scene yang sama, lalu ubah parameter kamera untuk melihat pengaruhnya pada ukuran objek di layar dan area yang terlihat.

## Menjalankan proyek

1. Pasang Unity Hub dan **Unity 6.0 LTS (6000.0.62f1)**. Versi editor tercatat di `ProjectSettings/ProjectVersion.txt`. [Unity Hub](https://docs.unity.com/en-us/hub) · [Unity 6000.0.62f1](https://unity.com/releases/editor/whats-new/6000.0.62f1)
2. Di Unity Hub, buka **Projects → Add → Add project from disk**. Pilih folder `KameraPerspektifUnity`, yaitu folder yang berisi `Assets`, `Packages`, dan `ProjectSettings`.
3. Tunggu proses impor dan kompilasi, lalu buka `Assets/Scenes/KameraPerspektif.unity` lewat panel **Project**.
4. Klik **Play** dan buka tab **Game**. Saat demo di proyektor, gunakan rasio 16:9 dan maksimalkan tab **Game** agar kedua tampilan serta panel kontrol mudah dibaca.

Proyek memakai Built-in Render Pipeline. Geometri, tekstur, dan antarmuka dibuat oleh script saat Play sehingga bisa dipelajari langsung dari source code.

## Percobaan yang tersedia

| Kontrol | Yang bisa diamati |
| --- | --- |
| Bandingkan, Perspektif, Ortografis (`Tab`) | Dua kamera dengan pose yang sama menampilkan jenis proyeksi yang berbeda. Mode Bandingkan menampilkan keduanya berdampingan. |
| FOV vertikal | Rentang 25–85°, awal 55°. Sudut yang lebih sempit memperbesar objek pada tampilan perspektif. Skala kamera ortografis tetap. |
| Jarak kamera | Rentang 6–22 unit, awal 10. Kamera bergerak mendekati atau menjauhi scene dan mengubah perbandingan ukuran proyeksi kubus dekat dan jauh. |
| Near dan Far | Batasi kedalaman yang dirender. Kubus bisa terpotong ketika bidang clipping melewati badannya atau hilang ketika seluruh kubus berada di luar batas. |
| Sudut pandang (`1`–`4`) | Pilih Depan, Serong, Tinggi, atau Rendah untuk melihat scene dari pose lain. |
| Pengukuran (`M`) | Tampilkan atau sembunyikan hasil ukur kedalaman dan ukuran proyeksi dalam piksel. |
| Petunjuk (`I`) | Buka atau tutup petunjuk di dalam demo. |
| Atur ulang (`R`) | Kembalikan parameter awal sebelum membandingkan variabel berikutnya. |

## Struktur kode

| Berkas | Tanggung jawab |
| --- | --- |
| `Assets/Scenes/KameraPerspektif.unity` | Scene yang dibuka untuk menjalankan demo. |
| `Assets/Scripts/CameraPerspectiveDemo.cs` | Menghubungkan scene, kamera, dan antarmuka saat runtime. |
| `Assets/Scripts/CameraRig.cs` | Mengatur pose, tipe proyeksi, FOV, dan bidang clipping kedua kamera. |
| `Assets/Scripts/DemoWorld.cs` | Membuat scene, kubus uji, material, tekstur, dan pencahayaan. |
| `Assets/Scripts/DemoHud.cs` | Menampilkan kontrol, legenda, dan hasil pengukuran. |
| `Assets/Scripts/DemoLayout.cs` | Menyelaraskan area gambar kedua kamera dengan panel antarmuka. |
| `Assets/Scripts/ProjectionMeasurement.cs` | Memproyeksikan delapan sudut setiap kubus untuk mengukur lebar proyeksi dalam piksel. |
| `Assets/Resources/WorldSurface.shader` | Menggabungkan warna dan tekstur material dengan pencahayaan Lambert. |

## Mendorong source code ke GitHub

Buat repo kosong di akun GitHub Anda. Jalankan perintah berikut dari folder `KameraPerspektifUnity` setelah mengganti `USERNAME` dan `REPO` dengan alamat repo Anda. Inisialisasi repo cukup sekali.

```sh
git init -b main
git add .
git status --short
git commit -m "Add Unity camera and perspective demo"
git remote add origin https://github.com/USERNAME/REPO.git
git push -u origin main
```

Saat memeriksa `git status`, pastikan berkas `.meta` dari Unity ikut tercatat bersama asetnya. Folder hasil impor, cache, dan build diabaikan lewat `.gitignore`. [Panduan GitHub](https://docs.github.com/en/migrations/importing-source-code/using-the-command-line-to-import-source-code/adding-locally-hosted-code-to-github)
