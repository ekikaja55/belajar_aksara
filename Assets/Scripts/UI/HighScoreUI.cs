using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BelajarAksara.Core;
using BelajarAksara.Data;
using BelajarAksara.Utils;
using BelajarAksara.Managers;

namespace BelajarAksara.UI
{
  public class HighscoreUI : MonoBehaviour
  {
    [Header("List Rendering")]
    public Transform contentParent;
    public GameObject rowPrefab;

    [Header("Empty State")]
    public GameObject emptyStateText;
    public GameObject scrollView;

    [Header("Navigation")]
    public Button btnBack;

    private void Start()
    {
      btnBack.onClick.AddListener(OnBackClicked);
      LoadAndRenderHighscores();
    }

    private void InsertDummyDataForTesting()
    {
      SQLiteService.SaveHighscore(new HighscoreEntry("08-09-2026", 8100, 1, 3));
      SQLiteService.SaveHighscore(new HighscoreEntry("07-09-2026", 5200, 1, 2));
      SQLiteService.SaveHighscore(new HighscoreEntry("05-09-2026", 3400, 1, 1));
      SQLiteService.SaveHighscore(new HighscoreEntry("03-09-2026", 9600, 2, 3));
      SQLiteService.SaveHighscore(new HighscoreEntry("01-09-2026", 1800, 1, 1));
    }

    private void LoadAndRenderHighscores()
    {
      List<HighscoreEntry> allScores = SQLiteService.GetAllHighscores();

      if (allScores.Count == 0)
      {
        emptyStateText.SetActive(true);
        scrollView.SetActive(false);
        return;
      }

      emptyStateText.SetActive(false);
      scrollView.SetActive(true);

      ClearExistingRows();

      foreach (HighscoreEntry entry in allScores)
      {
        GameObject rowInstance = Instantiate(rowPrefab, contentParent);
        HighscoreRowUI rowUI = rowInstance.GetComponent<HighscoreRowUI>();

        rowUI.SetData(entry.Waktu, entry.Score, entry.LevelMulai, entry.LevelAkhir);
      }
    }

    private void ClearExistingRows()
    {
      foreach (Transform child in contentParent)
      {
        Destroy(child.gameObject);
      }
    }

    private void OnBackClicked()
    {
      AudioManager.Instance.PlayBtnClick();
      SceneLoader.Instance.LoadScene(Constants.SCENE_MAIN_MENU);
    }
  }
}
