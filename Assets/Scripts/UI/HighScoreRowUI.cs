using UnityEngine;
using TMPro;

namespace BelajarAksara.UI
{
  // Script ini nempel di Prefab HighScoreRowTemplate.
  // Tugasnya cuma nyimpen referensi 4 Text di row ini,
  // supaya HighscoreUI.cs bisa ngisi teksnya lewat 1 fungsi SetData()
  public class HighscoreRowUI : MonoBehaviour
  {
    public TextMeshProUGUI textWaktu;
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textLevelMulai;
    public TextMeshProUGUI textLevelAkhir;

    public void SetData(string waktu, int score, int levelMulai, int levelAkhir)
    {
      textWaktu.text = waktu;
      textScore.text = score.ToString();
      textLevelMulai.text = levelMulai.ToString();
      textLevelAkhir.text = levelAkhir.ToString();
    }
  }
}
