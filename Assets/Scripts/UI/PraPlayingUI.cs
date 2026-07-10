using UnityEngine;
using UnityEngine.UI;
using BelajarAksara.Core;
using BelajarAksara.Managers;
using BelajarAksara.Utils;

namespace BelajarAksara.UI
{
  public class PraPlaying1UI : MonoBehaviour
  {
    [System.Serializable]
    public class LevelButtonRefs
    {
      public Button button;
      public GameObject lockIcon;
      public int levelNumber;
    }

    [Header("Level Buttons")]
    public LevelButtonRefs level1;
    public LevelButtonRefs level2;
    // public LevelButtonRefs level3;

    [Header("Navigation")]
    public Button btnBack;

    private void Start()
    {
      // Selalu mulai sesi baru dengan skor & lives fresh dan level 1 by default
      GameManager.Instance.StartNewSession(1);
      btnBack.onClick.AddListener(OnBackClicked);

      SetupLevelButton(level1);
      SetupLevelButton(level2);
      // SetupLevelButton(level3);
    }

    private void SetupLevelButton(LevelButtonRefs levelBtn)
    {
      bool isUnlocked = GameManager.Instance.IsLevelUnlocked(levelBtn.levelNumber);

      // Toggle icon gembok sesuai status
      levelBtn.lockIcon.SetActive(!isUnlocked);

      // Tombol TETAP interactable, sesuai strategi UX kita --
      // supaya anak-anak dapat feedback jelas saat klik level terkunci,
      // bukan cuma "diam" tanpa respon
      int capturedLevel = levelBtn.levelNumber; // capture supaya aman dipakai di lambda
      levelBtn.button.onClick.AddListener(() => OnLevelClicked(capturedLevel));
    }

    private void OnLevelClicked(int levelNumber)
    {
      AudioManager.Instance.PlayBtnClick();
      bool isUnlocked = GameManager.Instance.IsLevelUnlocked(levelNumber);

      if (isUnlocked)
      {
        GameManager.Instance.StartNewSession(levelNumber);
        NavigateToLevel(levelNumber);
      }
      else
      {
        int requiredLevel = levelNumber - 1;
        ToastManager.Instance.Show($"Selesaikan Level {requiredLevel} dulu ya!");
      }
    }

    private void NavigateToLevel(int levelNumber)
    {
      switch (levelNumber)
      {
        case 1:
          SceneLoader.Instance.LoadScene(Constants.SCENE_PRAPLAYING_2_TUTORCONFIRM);
          break;
        case 2:
          SceneLoader.Instance.LoadScene(Constants.SCENE_INGAME_LEVEL_2);
          break;
        // case 3:
        //   SceneLoader.Instance.LoadScene(Constants.SCENE_INGAME_LEVEL_3);
        //   break;
      }
    }

    private void OnBackClicked()
    {
      AudioManager.Instance.PlayBtnClick();
      SceneLoader.Instance.LoadScene(Constants.SCENE_MAIN_MENU);
    }
  }
}
