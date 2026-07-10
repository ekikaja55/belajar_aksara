using BelajarAksara.Core;
using BelajarAksara.Managers;
using BelajarAksara.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace BelajarAksara.UI
{
  public class PraPlaying3UI : MonoBehaviour
  {
    [Header("Navigation")]
    public Button btnBack;
    void Start()
    {
      btnBack.onClick.AddListener(OnBackClicked);
    }

    private void OnBackClicked()
    {
      AudioManager.Instance.PlayBtnClick();
      SceneLoader.Instance.LoadScene(Constants.SCENE_PRAPLAYING_2_TUTORCONFIRM);
    }
  }
}
