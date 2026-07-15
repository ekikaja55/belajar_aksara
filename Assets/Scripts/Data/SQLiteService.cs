using System;
using System.Collections.Generic;
using System.IO;
using SQLite;
using UnityEngine;

namespace BelajarAksara.Data
{

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
    [PrimaryKey]
    public int Id { get; set; }

    public float BgMusicVolume { get; set; }
    public float SfxVolume { get; set; }
  }


  public static class SQLiteService
  {
    private static SQLiteConnection _connection;

    private const string DB_NAME = "belajar_aksara.db";

    public static void Init()
    {
      if (_connection != null)
      {
        return;
      }

      string dbPath = Path.Combine(Application.persistentDataPath, DB_NAME);

      _connection = new SQLiteConnection(dbPath);

      _connection.CreateTable<HighscoreRow>();
      _connection.CreateTable<SettingsRow>();

      Debug.Log("[SQLiteService] Database initialized at: " + dbPath);

      EnsureDefaultSettings();
    }


    public static void SaveHighscore(HighscoreEntry entry)
    {
      HighscoreRow row = new HighscoreRow
      {
        Waktu = entry.Waktu,
        Score = entry.Score,
        LevelMulai = entry.LevelMulai,
        LevelAkhir = entry.LevelAkhir
      };

      _connection.Insert(row);
    }

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
          BgMusicVolume = 0.5f,
          SfxVolume = 0.5f
        };
        _connection.Insert(defaultRow);
      }
    }

    public static SettingsData GetSettings()
    {
      SettingsRow row = _connection.Table<SettingsRow>()
                                    .Where(r => r.Id == 1)
                                    .FirstOrDefault();

      if (row == null)
      {
        return new SettingsData();
      }

      return new SettingsData(row.BgMusicVolume, row.SfxVolume);
    }

    public static void SaveSettings(SettingsData data)
    {
      SettingsRow row = new SettingsRow
      {
        Id = 1,
        BgMusicVolume = data.BgMusicVolume,
        SfxVolume = data.SfxVolume
      };

      _connection.InsertOrReplace(row);
    }


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
