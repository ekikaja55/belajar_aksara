using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BelajarAksara.Core;
using BelajarAksara.Data;
using BelajarAksara.Utils;

namespace BelajarAksara.UI
{
  public class HighscoreUI : MonoBehaviour
  {
    [Header("List Rendering")]
    public Transform contentParent;      // "Content" di dalam ScrollView, tempat row di-instantiate
    public GameObject rowPrefab;          // Prefab HighScoreRowTemplate

    [Header("Empty State")]
    public GameObject emptyStateText;     // "Belum ada data yang tersimpan"
    public GameObject scrollView;         // HighscoreScrollView, ditoggle lawan dari emptyState

    [Header("Navigation")]
    public Button btnBack;

    private void Start()
    {
      btnBack.onClick.AddListener(OnBackClicked);
      // ===== DUMMY DATA UNTUK TESTING - HAPUS/COMMENT SEBELUM FINAL =====
      InsertDummyDataForTesting();
      LoadAndRenderHighscores();
    }

    private void InsertDummyDataForTesting()
    {
      SQLiteService.SaveHighscore(new HighscoreEntry("17:00 08-09-2026", 8100, 1, 3));
      SQLiteService.SaveHighscore(new HighscoreEntry("14:30 07-09-2026", 5200, 1, 2));
      SQLiteService.SaveHighscore(new HighscoreEntry("09:15 05-09-2026", 3400, 1, 1));
      SQLiteService.SaveHighscore(new HighscoreEntry("20:45 03-09-2026", 9600, 2, 3));
      SQLiteService.SaveHighscore(new HighscoreEntry("11:20 01-09-2026", 1800, 1, 1));
    }

    private void LoadAndRenderHighscores()
    {
      // Ambil semua data dari SQLite, sudah diurutkan dari score tertinggi
      // (urutan ini sudah di-handle di SQLiteService.GetAllHighscores())
      List<HighscoreEntry> allScores = SQLiteService.GetAllHighscores();

      if (allScores.Count == 0)
      {
        // Kosong -> tampilkan empty state, sembunyikan ScrollView
        emptyStateText.SetActive(true);
        scrollView.SetActive(false);
        return;
      }

      // Ada data -> tampilkan ScrollView, sembunyikan empty state
      emptyStateText.SetActive(false);
      scrollView.SetActive(true);

      // Bersihkan dulu row-row lama (kalau ada) sebelum render ulang,
      // supaya nggak numpuk/duplikat kalau LoadAndRenderHighscores()
      // dipanggil lebih dari sekali
      ClearExistingRows();

      // Loop semua data, bikin 1 GameObject row untuk tiap entry
      foreach (HighscoreEntry entry in allScores)
      {
        GameObject rowInstance = Instantiate(rowPrefab, contentParent);
        HighscoreRowUI rowUI = rowInstance.GetComponent<HighscoreRowUI>();

        rowUI.SetData(entry.Waktu, entry.Score, entry.LevelMulai, entry.LevelAkhir);
      }
    }

    private void ClearExistingRows()
    {
      // Hapus semua child yang sudah ada di dalam Content,
      // biar bersih sebelum di-render ulang
      foreach (Transform child in contentParent)
      {
        Destroy(child.gameObject);
      }
    }

    private void OnBackClicked()
    {
      SceneLoader.Instance.LoadScene(Constants.SCENE_MAIN_MENU);
    }
  }
}
