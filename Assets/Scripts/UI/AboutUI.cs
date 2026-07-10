using UnityEngine;
using UnityEngine.UI;
using BelajarAksara.Core;
using BelajarAksara.Utils;
using BelajarAksara.Managers;

namespace BelajarAksara.UI
{
  public class AboutUI : MonoBehaviour
  {
    public Button btnBack;

    private void Start()
    {
      btnBack.onClick.AddListener(OnBackClicked);
    }

    private void OnBackClicked()
    {
      AudioManager.Instance.PlayBtnClick();
      SceneLoader.Instance.LoadScene(Constants.SCENE_MAIN_MENU);
    }
  }
}
