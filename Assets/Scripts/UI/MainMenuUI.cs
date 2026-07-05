using UnityEngine;
using UnityEngine.UI;
using BelajarAksara.Core;
using BelajarAksara.Utils;

namespace BelajarAksara.UI
{
  public class MainMenuUI : MonoBehaviour
  {
    [Header("Menu Utama")]
    public Button btnStart;
    public Button btnHighscore;
    public Button btnExit;

    [Header("Menu Pendukung")]
    public Button btnSettings;
    public Button btnTutorial;
    public Button btnAbout;

    private void Start()
    {
      btnStart.onClick.AddListener(OnStartClicked);
      btnHighscore.onClick.AddListener(OnHighscoreClicked);
      btnExit.onClick.AddListener(OnExitClicked);
      btnSettings.onClick.AddListener(OnSettingsClicked);
      btnTutorial.onClick.AddListener(OnTutorialClicked);
      btnAbout.onClick.AddListener(OnAboutClicked);
    }

    private void OnStartClicked()
    {
      // Sesuai alur draft: Mulai -> masuk ke PraPlaying (pilih level),
      // bukan langsung ke Ingame, tapi refer ke Constants.cs
      SceneLoader.Instance.LoadScene(Constants.SCENE_PRAPLAYING_1_SELECT_LEVEL);
    }

    private void OnHighscoreClicked()
    {
      SceneLoader.Instance.LoadScene(Constants.SCENE_HIGHSCORE);
    }

    private void OnExitClicked()
    {
      // Application.Quit() TIDAK akan terlihat efeknya kalau ditest
      // di dalam Unity Editor (Play mode) -- ini normal, cuma bekerja
      // di build .exe yang sudah jadi.
      Application.Quit();

#if UNITY_EDITOR
      // Baris ini HANYA jalan saat di Unity Editor, supaya kamu bisa
      // lihat efek "keluar" pas testing (menghentikan Play mode).
      // Baris ini otomatis TIDAK ikut ke build .exe final
      UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnSettingsClicked()
    {
      SceneLoader.Instance.LoadScene(Constants.SCENE_SETTINGS);
    }

    private void OnTutorialClicked()
    {
      SceneLoader.Instance.LoadScene(Constants.SCENE_PRAPLAYING_2_TUTORIAL);
    }

    private void OnAboutClicked()
    {
      SceneLoader.Instance.LoadScene(Constants.SCENE_ABOUT);
    }
  }
}
