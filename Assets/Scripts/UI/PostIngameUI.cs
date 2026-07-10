using BelajarAksara.Core;
using BelajarAksara.Managers;
using BelajarAksara.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace BelajarAksara.UI
{
  public class PostIngameUI : MonoBehaviour
  {
    [Header("Navigations")]
    public Button btnHighscore;

    [Header("Game Info")]
    public TMPro.TextMeshProUGUI labelScore;
    public TMPro.TextMeshProUGUI labelLevel;

    void Start()
    {
      labelLevel.text = GameManager.Instance.CurrentLevel.ToString();
      labelScore.text = GameManager.Instance.CurrentScore.ToString();
      
      AudioManager.Instance.PlayGameOver();
      btnHighscore.onClick.AddListener(OnHighscoreClicked);
    }

    private void OnHighscoreClicked()
    {
      AudioManager.Instance.PlayBtnClick();
      SceneLoader.Instance.LoadScene(Constants.SCENE_HIGHSCORE);
    }
  }
}
