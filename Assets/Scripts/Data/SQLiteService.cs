using System;
using System.Collections.Generic;
using System.IO;
using SQLite;
using UnityEngine;

namespace BelajarAksara.Data
{
  // ===== TABLE DEFINITIONS =====
  // Class-class ini merepresentasikan TABEL di database.
  // Atribut [Table], [PrimaryKey], dst adalah cara SQLite-net
  // tahu cara bikin struktur tabel otomatis dari class C# biasa
  // (gaya ORM - Object Relational Mapping), jadi kamu nggak perlu
  // nulis CREATE TABLE manual pakai SQL string.

  [Table("Highscore")]
  public class HighscoreRow
  {
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Waktu { get; set; }
    public int Score { get; set; }
    public int LevelMulai { get; set; }
    public int LevelAkhir { get; set; }
  }

  [Table("Settings")]
  public class SettingsRow
  {
    // Settings cuma butuh 1 baris data (single row), makanya
    // Id di-hardcode 1, bukan AutoIncrement. Ini trik umum
    // untuk tabel yang isinya cuma "1 baris config global".
    [PrimaryKey]
    public int Id { get; set; }

    public float BgMusicVolume { get; set; }
    public float SfxVolume { get; set; }
  }


  // ===== SERVICE CLASS =====
  // Ini "wrapper"-nya. Semua script lain (UIManager, SettingsUI,
  // HighscoreUI, dll) berinteraksi lewat class ini, BUKAN langsung
  // ke SQLiteConnection. Tujuannya: kalau nanti mau ganti cara
  // simpan data (misal dari SQLite ke sistem lain), kamu cuma
  // perlu ubah isi class ini, script lain nggak perlu diubah.
  public static class SQLiteService
  {
    // Connection di-cache di sini (static), supaya nggak buka-tutup
    // koneksi database berulang kali tiap kali ada query.
    private static SQLiteConnection _connection;

    private const string DB_NAME = "belajar_aksara.db";

    // ----- SETUP / INIT -----

    // WAJIB dipanggil sekali di awal (misal dari GameManager.Awake())
    // sebelum method lain di class ini dipakai.
    public static void Init()
    {
      if (_connection != null)
      {
        // Sudah pernah di-init sebelumnya, skip biar nggak dobel buka koneksi
        return;
      }

      // Application.persistentDataPath itu folder khusus per-platform
      // yang PASTI writable (bisa ditulis) di semua device, termasuk
      // Android. Beda dengan StreamingAssets yang read-only di Android.
      // Makanya database "kerja" kita taruh di sini, bukan di StreamingAssets.
      string dbPath = Path.Combine(Application.persistentDataPath, DB_NAME);

      _connection = new SQLiteConnection(dbPath);

      // CreateTable otomatis bikin tabel KALAU BELUM ADA.
      // Kalau tabel sudah ada (misal user main lagi, bukan pertama kali),
      // baris ini aman dipanggil berkali-kali, nggak akan menghapus data lama.
      _connection.CreateTable<HighscoreRow>();
      _connection.CreateTable<SettingsRow>();

      Debug.Log("[SQLiteService] Database initialized at: " + dbPath);

      // Pastikan ada 1 baris default di tabel Settings kalau ini pertama
      // kali app dijalankan (biar SettingsUI nggak error nyari data kosong)
      EnsureDefaultSettings();
    }


    // ----- HIGHSCORE -----

    // Simpan 1 entry highscore baru. Dipanggil dari PostIngameUI
    // saat kondisi trigger post-game terpenuhi (lives habis / tamat game).
    public static void SaveHighscore(HighscoreEntry entry)
    {
      HighscoreRow row = new HighscoreRow
      {
        Waktu = entry.Waktu,
        Score = entry.Score,
        LevelMulai = entry.LevelMulai,
        LevelAkhir = entry.LevelAkhir
      };

      // Insert = tambah baris baru, Id otomatis ke-generate oleh AutoIncrement
      _connection.Insert(row);
    }

    // Ambil semua data highscore, diurutkan dari score tertinggi.
    // Dipanggil dari HighscoreUI saat scene Highscore dibuka.
    public static List<HighscoreEntry> GetAllHighscores()
    {
      List<HighscoreRow> rows = _connection.Table<HighscoreRow>()
                                             .OrderByDescending(r => r.Score)
                                             .ToList();

      List<HighscoreEntry> result = new List<HighscoreEntry>();
      foreach (HighscoreRow row in rows)
      {
        result.Add(new HighscoreEntry(row.Waktu, row.Score, row.LevelMulai, row.LevelAkhir));
      }

      return result;
    }


    // ----- SETTINGS -----

    // Cek apakah baris settings sudah ada, kalau belum, buat default-nya.
    // Ini dipanggil otomatis dari Init(), jadi kamu nggak perlu panggil manual.
    private static void EnsureDefaultSettings()
    {
      SettingsRow existing = _connection.Table<SettingsRow>()
                                          .Where(r => r.Id == 1)
                                          .FirstOrDefault();

      if (existing == null)
      {
        SettingsRow defaultRow = new SettingsRow
        {
          Id = 1,
          BgMusicVolume = 1.0f,
          SfxVolume = 1.0f
        };
        _connection.Insert(defaultRow);
      }
    }

    // Ambil settings yang tersimpan. Dipanggil saat AudioManager
    // pertama kali jalan, dan saat SettingsUI dibuka.
    public static SettingsData GetSettings()
    {
      SettingsRow row = _connection.Table<SettingsRow>()
                                    .Where(r => r.Id == 1)
                                    .FirstOrDefault();

      if (row == null)
      {
        // Fallback pengaman, seharusnya nggak pernah terjadi
        // karena EnsureDefaultSettings() sudah dipanggil di Init()
        return new SettingsData();
      }

      return new SettingsData(row.BgMusicVolume, row.SfxVolume);
    }

    // Simpan perubahan settings. Dipanggil saat user klik tombol
    // "simpan" di scene Settings.
    public static void SaveSettings(SettingsData data)
    {
      SettingsRow row = new SettingsRow
      {
        Id = 1,
        BgMusicVolume = data.BgMusicVolume,
        SfxVolume = data.SfxVolume
      };

      // InsertOrReplace: kalau baris dengan Id=1 sudah ada, DI-UPDATE.
      // Kalau belum ada, DI-INSERT. Ini yang bikin kita nggak perlu
      // mikirin manual "apakah harus insert atau update".
      _connection.InsertOrReplace(row);
    }


    // ----- CLEANUP -----

    // Dipanggil saat aplikasi ditutup (dari GameManager.OnApplicationQuit()
    // misalnya), supaya koneksi database ditutup dengan rapi.
    public static void Close()
    {
      if (_connection != null)
      {
        _connection.Close();
        _connection = null;
      }
    }
  }
}
