using UnityEngine;
using UnityEngine.UI;
using BelajarAksara.Core;
using BelajarAksara.Utils;
using BelajarAksara.Data;
using BelajarAksara.Managers;

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
      // GameManager.Instance.ResetProgress(); // Dummy: Reset progress setiap kali main menu dibuka, supaya testing lebih gampang
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
      AudioManager.Instance.PlayBtnClick();
      SceneLoader.Instance.LoadScene(Constants.SCENE_PRAPLAYING_1_SELECT_LEVEL);
    }

    private void OnHighscoreClicked()
    {
      AudioManager.Instance.PlayBtnClick();
      SceneLoader.Instance.LoadScene(Constants.SCENE_HIGHSCORE);
    }

    private void OnExitClicked()
    {
      AudioManager.Instance.PlayBtnClick();
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
      AudioManager.Instance.PlayBtnClick();
      SceneLoader.Instance.LoadScene(Constants.SCENE_SETTINGS);
    }

    private void OnTutorialClicked()
    {
      AudioManager.Instance.PlayBtnClick();
      SceneLoader.Instance.LoadScene(Constants.SCENE_TUTORIAL_MAIN_MENU);
    }

    private void OnAboutClicked()
    {
      AudioManager.Instance.PlayBtnClick();
      SceneLoader.Instance.LoadScene(Constants.SCENE_ABOUT);
    }
  }
}
