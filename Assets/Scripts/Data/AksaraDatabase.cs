using System;
using System.Collections.Generic;

namespace BelajarAksara.Data
{
// class AksaraQuestion untuk class questionnya
  [Serializable]
  public class AksaraQuestion
  {
    public string[] HurufPenyusun;

    public AksaraQuestion(params string[] hurufPenyusun)
    {
      HurufPenyusun = hurufPenyusun;
    }

    public string JawabanGabungan
    {
      // misal ["ha","na"] -> "hana"
      get { return string.Join("", HurufPenyusun); }
    }
  }

// untuk class question nya
  public static class AksaraDatabase
  {
    // LEVEL 1 - 18 huruf tunggal
    public static readonly AksaraQuestion[] Level1 = new AksaraQuestion[]
    {
            new AksaraQuestion("ha"),
            new AksaraQuestion("na"),
            new AksaraQuestion("ca"),
            new AksaraQuestion("ra"),
            new AksaraQuestion("ka"),
            new AksaraQuestion("da"),
            new AksaraQuestion("ta"),
            new AksaraQuestion("sa"),
            new AksaraQuestion("wa"),
            new AksaraQuestion("la"),
            new AksaraQuestion("ma"),
            new AksaraQuestion("ga"),
            new AksaraQuestion("ba"),
            new AksaraQuestion("nga"),
            new AksaraQuestion("pa"),
            new AksaraQuestion("ja"),
            new AksaraQuestion("ya"),
            new AksaraQuestion("nya")
    };

    // LEVEL 2 - 9 kombinasi 2 huruf
    public static readonly AksaraQuestion[] Level2 = new AksaraQuestion[]
    {
            new AksaraQuestion("ha", "na"),
            new AksaraQuestion("ca", "ra"),
            new AksaraQuestion("ka", "da"),
            new AksaraQuestion("ta", "sa"),
            new AksaraQuestion("wa", "la"),
            new AksaraQuestion("ma", "ga"),
            new AksaraQuestion("ba", "nga"),
            new AksaraQuestion("pa", "ja"),
            new AksaraQuestion("ya", "nya")
    };

    // LEVEL 3 - 6 kombinasi 3 huruf
    // public static readonly AksaraQuestion[] Level3 = new AksaraQuestion[]
    // {
    //         new AksaraQuestion("ha", "na", "ca"),
    //         new AksaraQuestion("ra", "ka", "da"),
    //         new AksaraQuestion("ta", "sa", "wa"),
    //         new AksaraQuestion("la", "ma", "ga"),
    //         new AksaraQuestion("ba", "nga", "pa"),
    //         new AksaraQuestion("ja", "ya", "nya")
    // };

    public static AksaraQuestion[] GetQuestionsByLevel(int level)
    {
      switch (level)
      {
        case 1: return Level1;
        case 2: return Level2;
        // case 3: return Level3;
        default: return new AksaraQuestion[0];
      }
    }

    public static List<string> GetAllSingleLetters()
    {
      List<string> letters = new List<string>();
      foreach (AksaraQuestion q in Level1)
      {
        letters.Add(q.HurufPenyusun[0]);
      }
      return letters;
    }
  }
}
