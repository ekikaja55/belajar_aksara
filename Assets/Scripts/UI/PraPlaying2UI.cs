using BelajarAksara.Core;
using BelajarAksara.Managers;
using BelajarAksara.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace BelajarAksara.UI
{
  public class PraPlaying2UI : MonoBehaviour
  {
    [Header("Navigation")]
    public Button btnBack;
    public Button btnNext;
    public Button btnTutorial;
    public Button btnDictionary;

    void Start()
    {
      btnBack.onClick.AddListener(OnBackClicked);
      btnNext.onClick.AddListener(OnNextClicked);
      btnTutorial.onClick.AddListener(OnTutorialClicked);
      btnDictionary.onClick.AddListener(OnDictionaryClicked);
    }

    private void OnBackClicked()
    {
      // Kembali ke PraPlaying1 (pilih level)
      AudioManager.Instance.PlayBtnClick();
      SceneLoader.Instance.LoadScene(Constants.SCENE_PRAPLAYING_1_SELECT_LEVEL);
    }

    private void OnNextClicked()
    {
      // Lanjut ke Ingame Level 1 (langsung main)
      AudioManager.Instance.PlayBtnClick();
      SceneLoader.Instance.LoadScene(Constants.SCENE_INGAME_LEVEL_1);
    }

    private void OnTutorialClicked()
    {
      // Lanjut ke PraPlaying3 (tutorial)
      AudioManager.Instance.PlayBtnClick();
      SceneLoader.Instance.LoadScene(Constants.SCENE_PRAPLAYING_3_TUTORIAL);
    }

    private void OnDictionaryClicked()
    {
      // Lanjut ke PraPlaying4 (dictionary)
      AudioManager.Instance.PlayBtnClick();
      SceneLoader.Instance.LoadScene(Constants.SCENE_PRAPLAYING_4_DICTIONARY);
    }

  }
}
