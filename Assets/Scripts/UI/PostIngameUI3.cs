using BelajarAksara.Core;
using BelajarAksara.Managers;
using BelajarAksara.Utils;
using UnityEngine;
using UnityEngine.UI;
namespace BelajarAksara.UI
{
  public class PostIngameUI3 : MonoBehaviour
  {
    [Header("Navigations")]
    public Button btnExit;
    public Button btnHighscore;

    [Header("Level Info")]
    public TMPro.TextMeshProUGUI levelLabel;
    public TMPro.TextMeshProUGUI scoreLabel;

    void Start()
    {
      AudioManager.Instance.PlayLevelCompleted();
      levelLabel.text = GameManager.Instance.CurrentLevel.ToString();
      scoreLabel.text = GameManager.Instance.CurrentScore.ToString();

      btnExit.onClick.AddListener(() =>
      {
        AudioManager.Instance.PlayBtnClick();
        SceneLoader.Instance.LoadScene(Constants.SCENE_MAIN_MENU);
      });
      btnHighscore.onClick.AddListener(() =>
      {
        AudioManager.Instance.PlayBtnClick();
        SceneLoader.Instance.LoadScene(Constants.SCENE_HIGHSCORE);
      });
    }
  }
}
