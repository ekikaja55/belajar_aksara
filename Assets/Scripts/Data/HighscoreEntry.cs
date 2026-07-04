using System;

namespace BelajarAksara.Data
{
  [Serializable]
  public class HighscoreEntry
  {
    public string Waktu;
    public int Score;
    public int LevelMulai;
    public int LevelAkhir;

    public HighscoreEntry(string waktu, int score, int levelMulai, int levelAkhir)
    {
      Waktu = waktu;
      Score = score;
      LevelMulai = levelMulai;
      LevelAkhir = levelAkhir;
    }
  }
}
