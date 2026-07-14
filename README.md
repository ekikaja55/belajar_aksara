# Belajar Aksara

Tugas Ujian Akhir Semester — Game edukatif 2D untuk belajar Aksara Bali.

## Informasi Proyek

- **Judul Tugas**: Ujian Akhir Semester
- **Judul Game**: Belajar Aksara
- **Developer**: Alkindi Abdillah Kahfi
- **NIM**: 230030076
- **Jurusan**: Sistem Informasi Bisnis Malam

## Deskripsi

Belajar Aksara adalah game edukatif 2D yang dirancang untuk membantu pemain mempelajari aksara Bali melalui gameplay interaktif. Pemain akan mengenali huruf aksara tunggal, kombinasi dua huruf, dan kombinasi tiga huruf secara bertahap melalui tiga level permainan.

## Fitur Utama

- Menu utama dengan navigasi ke Mulai, Highscore, Pengaturan, dan Tentang
- Sistem highscore dan pengaturan yang tersimpan menggunakan SQLite
- Kamus aksara Bali sebagai referensi belajar sebelum bermain
- Tutorial penggunaan sebelum masuk ke sesi bermain
- Tiga level permainan dengan tingkat kesulitan bertahap:
  - Level 1: pengenalan 18 huruf aksara tunggal
  - Level 2: kombinasi dua huruf aksara
  - Level 3: kombinasi tiga huruf aksara
- Sistem skor dan nyawa (lives) selama permainan
- Fitur hint dengan konsekuensi pengurangan skor
- Progress belajar yang tersimpan antar sesi

## Teknologi

- **Engine**: Unity 6 (6000.3.19f1 LTS)
- **Render Pipeline**: Universal Render Pipeline (URP) 2D
- **Bahasa**: C# (.NET 9)
- **Database**: SQLite (highscore, pengaturan, dan progress pemain)

## Struktur Proyek (Folder Assets)

```
Assets/
├── Scenes/          Seluruh scene permainan, dari menu utama hingga post-ingame
├── Scripts/
│   ├── Core/        GameLoop, GameManager (singleton), SceneLoader
│   ├── Data/        Database aksara, model data, wrapper SQLite
│   ├── Managers/    Leveling, audio, dan UI manager
│   ├── UI/          Script UI tiap scene
│   └── Utils/       Konstanta dan helper
├── Prefabs/         Prefab UI dan elemen gameplay
├── Sprites/         Aset visual aksara, UI, dan ikon aplikasi
├── Fonts/           Font yang digunakan dalam game
├── Audio/           Musik latar dan efek suara
├── Resources/       Aset yang dimuat secara dinamis saat runtime
└── StreamingAssets/ Berkas database SQLite bawaan
```
## Alur Permainan

1. Pemain membuka menu utama dan dapat mengakses tutorial, kamus aksara, highscore, atau pengaturan.
2. Sebelum bermain, pemain memilih level dan disarankan menyelesaikan tutorial serta mempelajari kamus aksara.
3. Di dalam permainan, pemain menjawab pertanyaan "ini huruf apa?" dengan menyeret jawaban ke kotak yang tersedia.
4. Jawaban benar menambah skor sesuai level, jawaban salah mengurangi nyawa.
5. Permainan berakhir pada salah satu dari tiga kondisi: nyawa habis, level selesai dan berlanjut ke level berikutnya, atau seluruh level (hingga level 3) telah diselesaikan.

## Aset Pendukung

Aset yang digunakan dalam proyek ini (gambar, ikon, dan berkas pendukung lainnya) dapat diakses melalui tautan berikut:

[Google Drive - Aset Belajar Aksara](https://drive.google.com/drive/folders/1wS6tw5BXdValCS5hf-SOvzuG4DJoFMJH)

## Kontak

Instagram: [@kindyabdillah_](https://instagram.com/kindyabdillah_)


