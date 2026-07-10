using BelajarAksara.Core;
using BelajarAksara.Data;
using BelajarAksara.Managers;
using BelajarAksara.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace BelajarAksara.UI
{
  public class PostIngameUI2 : MonoBehaviour
  {
    [Header("Navigations")]
    public Button btnNextLevel;
    public Button btnExit;

    [Header("Level Info")]
    public TMPro.TextMeshProUGUI titleLabel;
    public TMPro.TextMeshProUGUI levelLabel;
    public TMPro.TextMeshProUGUI scoreLabel;

    void Start()
    {
      AudioManager.Instance.PlayLevelCompleted();
      titleLabel.text = $"LEVEL {GameManager.Instance.CurrentLevel} SELESAI!";
      levelLabel.text = GameManager.Instance.CurrentLevel.ToString();
      scoreLabel.text = GameManager.Instance.CurrentScore.ToString();

      btnNextLevel.onClick.AddListener(OnNextLevelClicked);
      btnExit.onClick.AddListener(OnExitClicked);
    }

    private void OnNextLevelClicked()
    {
      AudioManager.Instance.PlayBtnClick();
      int nextLevel = GameManager.Instance.CurrentLevel + 1;
      GameManager.Instance.AdvanceToLevel(nextLevel); // <-- update CurrentLevel, skor/lives tetap

      switch (nextLevel)
      {
        case 2:
          SceneLoader.Instance.LoadScene(Constants.SCENE_INGAME_LEVEL_2);
          break;
        // case 3:
        //   SceneLoader.Instance.LoadScene(Constants.SCENE_INGAME_LEVEL_3);
        //   break;
      }
    }

    private void OnExitClicked()
    {
      AudioManager.Instance.PlayBtnClick();
      ModalManager.Instance.Show(
          "Keluar dari permainan? Progres akhirmu akan tersimpan.",
          new ModalButtonData("Ya", OnExitConfirmed),
          new ModalButtonData("Tidak", () => {
              AudioManager.Instance.PlayBtnClick();
              ModalManager.Instance.Hide();
          })
      );
    }

    private void OnExitConfirmed()
    {
      AudioManager.Instance.PlayBtnClick();
      SaveScoreToHighscore();
      SceneLoader.Instance.LoadScene(Constants.SCENE_MAIN_MENU);
    }

    private void SaveScoreToHighscore()
    {
      string waktu = System.DateTime.Now.ToString("dd-MM-yyyy");
      HighscoreEntry entry = new HighscoreEntry(
          waktu,
          GameManager.Instance.CurrentScore,
          GameManager.Instance.SessionStartLevel,
          GameManager.Instance.CurrentLevel
      );
      SQLiteService.SaveHighscore(entry);
    }
  }
}
