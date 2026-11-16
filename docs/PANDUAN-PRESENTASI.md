# Panduan presentasi Kamera dan Perspektif

Gunakan PPT **Kamera dan Perspektif** milik Kelompok 10 sebagai pengantar konsep. Beralih ke Unity sekali setelah slide 11, lalu kembali ke slide 12 untuk merangkum. Latih alur di bawah selama kurang lebih enam menit di resolusi yang akan dipakai di kelas.

## Alur demo

| Waktu | Tindakan | Yang dijelaskan |
| --- | --- | --- |
| 0:00–0:45 | Perkenalkan proyek lewat slide 1–4, lalu jelaskan alur koordinat pada slide 5. | Unity merender scene 3D dari sudut pandang sebuah komponen `Camera`. Posisi, rotasi, dan skala setiap `GameObject` ada pada `Transform`. |
| 0:45–1:30 | Ringkas slide 6–11, terutama rumus perspektif pada slide 8. | Proyeksi mengubah posisi 3D ke bidang gambar 2D. Tanyakan kubus mana yang akan terlihat paling besar ketika semua sisi kubus sama panjang. |
| 1:30–2:30 | Buka Unity pada mode **Bandingkan**. Tunjukkan kubus dekat, tengah, dan jauh serta garis jalan. | Kubus memiliki sisi 2 unit. Pada perspektif, kubus yang lebih dalam menempati bagian layar yang lebih kecil dan garis jalan tampak menuju titik hilang. Pada ortografis, lebar proyeksi ketiga kubus sama saat dilihat lurus. |
| 2:30–3:30 | Coba FOV 40° dan 70° sambil menahan jarak kamera. Tekan R lalu ubah jarak dari 10 ke 16 unit dengan FOV tetap. | FOV mengubah sudut cakupan. Menggeser kamera mengubah kedalaman relatif setiap objek, sehingga ukuran relatifnya juga berubah dalam tampilan perspektif. |
| 3:30–4:20 | Dari posisi awal, turunkan **Far** ke sekitar 20 unit, lalu kembalikan. Naikkan **Near** ke sekitar 12 unit lalu reset. | Kubus jauh hilang saat melewati batas jauh. Kubus dekat hilang ketika seluruh kubus berada sebelum batas dekat. Bandingkan kedua viewport. |
| 4:20–5:15 | Ganti dari **Depan** ke **Serong**, **Tinggi**, dan **Rendah**. Kembali ke **Depan**. | Depan menunjukkan titik hilang arah jalan. Serong memunculkan dua arah titik hilang. Tinggi menambahkan kemiringan vertikal sehingga ketiga sumbu memiliki titik hilang, yang dapat berada di luar gambar. |
| 5:15–6:00 | Tampilkan pengukuran dengan `M` jika tertutup, lalu kembali ke slide 12–14. | Ringkas perbedaan proyeksi dan persilakan audiens bertanya. |

Untuk visual pendukung slide, ambil satu tangkapan tab **Game** pada posisi awal yang memperlihatkan kedua proyeksi. Tempatkan di slide 6 atau tampilkan sebagai cadangan demo.

## Cara menjelaskan program

**Scene dan objek 3D.** Scene adalah wadah untuk kamera, objek, dan lampu. `DemoWorld` menyusun model 3D dari mesh primitif. `Transform` menempatkan kubus pada kedalaman yang berbeda dan mempertahankan sisi tiap kubus sebesar 2 unit. Material menentukan respons permukaan terhadap cahaya. Tekstur prosedural memberi pola permukaan yang juga membantu mata mengikuti arah dan kedalaman. Lampu menonjolkan bentuk dan orientasi permukaan.

**Kamera dan viewport.** `CameraPerspectiveDemo` merangkai dunia, pilihan kamera, dan antarmuka. `CameraRig` menyamakan posisi dan rotasi dua kamera, memakai proyeksi yang berbeda, lalu menempatkan hasil render di viewport masing-masing. `DemoHud` meneruskan perubahan pengguna dan memberikan umpan balik berupa angka serta label. [Dokumentasi Camera](https://docs.unity.com/en-us/engine/6000.0/manual/cameras/birp/class-camera)

**Transformasi dan pengukuran.** Pada slide 5, matriks Model membawa titik dari ruang objek ke dunia, matriks View ke ruang kamera, lalu Projection ke clip space. Unity menangani pemetaan setiap kamera. `ProjectionMeasurement` memakai `WorldToViewportPoint` untuk memproyeksikan delapan sudut kubus. Nilai X dan Y relatif terhadap viewport, sedangkan Z menyatakan kedalaman dalam unit dunia sepanjang arah pandang. Jika semua sudut masuk viewport dan batas clipping, program mengalikan rentang X dengan lebar panel dalam piksel. Angka tersebut menyatakan lebar kotak pembatas proyeksi dengan mengabaikan oklusi. [Dokumentasi WorldToViewportPoint](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/camera/worldtoviewportpoint)

**Rumus perspektif.** Untuk segmen sejajar bidang gambar dengan lebar `w` pada kedalaman positif `d`, lebar proyeksinya `f × w / d` pada model pinhole. Dengan FOV dan pose awal tetap, kedalaman pusat kubus adalah sekitar 10, 17, dan 24 unit. Gunakan angka ini untuk menjelaskan tren. Ketebalan kubus membuat tiap sudutnya berada pada kedalaman yang sedikit berbeda.

## Simulasi sebelum kelas

| Uji di tab Game | Hasil yang diharapkan |
| --- | --- |
| Buka scene, klik Play, dan lihat **Console** | Scene, kontrol, serta kedua proyeksi terlihat. Periksa bahwa Console bebas error baru. |
| Jalankan mode perbandingan dari posisi awal | Kubus dekat terlihat lebih besar pada perspektif. Pada ortografis, ketiga lebar proyeksi sama. |
| Ubah FOV lalu uji jarak secara terpisah | Perubahan sudut memengaruhi tampilan perspektif. Perubahan jarak mengubah rasio ukuran kubus dekat dan jauh. |
| Kecilkan Far, kembalikan, lalu besarkan Near | Kubus keluar dari render sesuai batas clipping. Kembalikan nilai agar semua kubus terlihat. |
| Coba empat pose lalu tekan `R` | Sudut pandang berubah dan **Atur ulang** mengembalikan pengaturan awal. |
| Tekan `Tab`, `I`, dan `M` | Mode proyeksi berganti, petunjuk dapat dibuka dan ditutup, dan pengukuran dapat ditampilkan atau disembunyikan. |
| Uji dengan rasio 16:9 | Teks kontrol, layar perbandingan, dan angka dapat dibaca dari posisi audiens. |

## Latihan tanya jawab

- **Kenapa ukuran kubus jauh mengecil pada perspektif?** Koordinat gambar dibagi kedalaman `d`. Pada FOV tetap, segmen berukuran sama dengan kedalaman dua kali lipat memiliki lebar proyeksi setengahnya.
- **Apa yang membedakan perspektif dan ortografis?** Sinar proyeksi perspektif bertemu di pusat kamera. Sinar proyeksi ortografis sejajar, sehingga skala proyeksi tetap terhadap kedalaman untuk orientasi tetap.
- **Apa beda FOV dan menggeser kamera?** FOV mengubah sudut cakupan dari posisi yang sama. Pergeseran kamera mengubah posisi pengamatan dan kedalaman tiap kubus, sehingga rasio ukuran objek dekat dan jauh ikut berubah.
- **Mengapa slider FOV tidak mengubah ukuran pada viewport ortografis?** Unity memakai `orthographicSize` untuk setengah tinggi area yang terlihat secara ortografis, sementara `fieldOfView` mengatur sudut vertikal pada kamera perspektif. [FOV](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/camera/fieldofview) · [Orthographic size](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/camera/orthographicsize)
- **Apa arti Near dan Far?** Keduanya merupakan batas dekat dan jauh volume pandang. Geometri yang melewati salah satu bidang bisa terlihat terpotong. [Dokumentasi clipping planes](https://docs.unity.com/en-us/engine/6000.0/manual/cameras/birp/class-camera)
- **Mengapa slide menyebut −Z sedangkan Unity menempatkan kubus ke arah +Z?** Arah `Transform.forward` merupakan sumbu +Z lokal objek kamera. Matriks `worldToCameraMatrix` Unity mengikuti konvensi ruang kamera yang mengarah ke −Z, seperti yang digunakan dalam pembahasan slide. Nilai `d` dalam rumus adalah kedalaman positif. [Dokumentasi worldToCameraMatrix](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/camera/worldtocameramatrix)
