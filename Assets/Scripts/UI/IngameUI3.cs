using System.Collections.Generic;
using BelajarAksara.Core;
using BelajarAksara.Data;
using BelajarAksara.Managers;
using BelajarAksara.Utils;
using UnityEngine;
using UnityEngine.UI;
namespace BelajarAksara.UI
{
  public class IngameUI3 : MonoBehaviour
  {
    [Header("Header Info")]
    public TMPro.TextMeshProUGUI labelScore;
    public TMPro.TextMeshProUGUI labelLives;

    [Header("Navigations")]
    public Button btnExit;
    public Button btnHint;

    [Header("Question")]
    public Image imageQuestion1;
    public Image imageQuestion2;
    public Image imageQuestion3;

    [Header("Answer Choices")]
    public DraggableAnswer[] answerChoices;
    public TMPro.TextMeshProUGUI[] answerChoiceTexts;

    [Header("Drop Zone")]
    public AnswerDropZone dropZone;
    public TMPro.TextMeshProUGUI titleDropBox;

    private const int LEVEL_NUMBER = 3;

    private int _currentQuestionIndex = 0;
    private AksaraQuestion[] _questions;

    private string _slot1Value = null;
    private DraggableAnswer _slot1Draggable = null;

    private string _slot2Value = null;
    private DraggableAnswer _slot2Draggable = null;

    private string _slot3Value = null;
    private DraggableAnswer _slot3Draggable = null;

    private bool _isProcessingAnswer = false;
    private int _step_count = 0;

    void Start()
    {
      _questions = AksaraDatabase.GetQuestionsByLevel(LEVEL_NUMBER);
      btnExit.onClick.AddListener(OnExitClicked);
      btnHint.onClick.AddListener(OnHintClicked);

      dropZone.onAnswerDropped = OnAnswerDropped;
      RenderCurrentQuestion();
      UpdateHeaderUI();
      _step_count = 0;
    }

    private void UpdateHeaderUI()
    {
      labelLives.text = GameManager.Instance.CurrentLives.ToString();
      labelScore.text = GameManager.Instance.CurrentScore.ToString();
    }

    private void RenderCurrentQuestion()
    {
      if (_currentQuestionIndex >= _questions.Length)
      {
        Debug.LogWarning("RenderCurrentQuestion dipanggil padahal soal sudah habis!");
        return;
      }

      ResetSlots();

      AksaraQuestion currentQuestion = _questions[_currentQuestionIndex];

      imageQuestion1.sprite = LoadHurufSprite(currentQuestion.HurufPenyusun[0]);
      imageQuestion2.sprite = LoadHurufSprite(currentQuestion.HurufPenyusun[1]);
      imageQuestion3.sprite = LoadHurufSprite(currentQuestion.HurufPenyusun[2]);

      List<string> choices = GetSixChoices(currentQuestion);

      for (int i = 0; i < answerChoices.Length; i++)
      {
        string letter = choices[i];
        answerChoices[i].letterValue = letter;
        answerChoiceTexts[i].text = letter;
        answerChoices[i].ResetToOriginalPosition();
      }
    }

    private List<string> GetSixChoices(AksaraQuestion currentQuestion)
    {
      List<string> result = new List<string>
            {
                currentQuestion.HurufPenyusun[0],
                currentQuestion.HurufPenyusun[1],
                currentQuestion.HurufPenyusun[2],
            };

      AksaraQuestion[] level1Letters = AksaraDatabase.Level1;
      int pengecoh = 0;
      int idx = 0;

      while (pengecoh < 4 && idx < level1Letters.Length)
      {
        string candidate = level1Letters[idx].HurufPenyusun[0];
        if (candidate != currentQuestion.HurufPenyusun[0] && candidate != currentQuestion.HurufPenyusun[1])
        {
          result.Add(candidate);
          pengecoh++;
        }
        idx++;
      }

      return result;
    }

    private Sprite LoadHurufSprite(string huruf)
    {
      string path = Constants.ASSET_HURUF_PREFIX + huruf + Constants.ASSET_HURUF_SUFFIX;
      Sprite sprite = Resources.Load<Sprite>(path);

      if (sprite == null)
      {
        Debug.LogWarning($"Sprite tidak ditemukan untuk huruf '{huruf}' di path: {path}");
      }

      return sprite;
    }

    private void ResetSlots()
    {
      // Kalau ada draggable yang sudah menempel di slot 1, balikkan ke posisi asal
      if (_slot1Draggable != null) _slot1Draggable.ResetToOriginalPosition();
      if (_slot2Draggable != null) _slot2Draggable.ResetToOriginalPosition();

      _slot1Value = null;
      _slot1Draggable = null;

      _slot2Value = null;
      _slot2Draggable = null;

      titleDropBox.text = "Taruh Disini";
    }

    private void OnAnswerDropped(DraggableAnswer droppedAnswer)
    {
      if (_isProcessingAnswer) return;
      _isProcessingAnswer = true;

      AksaraQuestion currenQuestion = _questions[_currentQuestionIndex];

      switch (_step_count)
      {
        case 1:
          ProcessFirstDrop(droppedAnswer, currenQuestion);
          break;
        case 2:
          ProcessSecondDrop(droppedAnswer, currenQuestion);
          break;
        case 3:
          ProcessThirdDrop(droppedAnswer, currenQuestion);
          break;
      }
      _isProcessingAnswer = false;
    }

    // 3 drop answer pake flag??
    private void ProcessFirstDrop(DraggableAnswer droppedAnswer, AksaraQuestion currentQuestion)
    {
      bool isCorrect = droppedAnswer.letterValue == currentQuestion.HurufPenyusun[0];

      if (isCorrect)
      {
        AudioManager.Instance.PlayCorrectAnswer();
        _slot1Value = droppedAnswer.letterValue;
        _slot1Draggable = droppedAnswer;

        droppedAnswer.SnapToDropZone(dropZone.transform);
        titleDropBox.text = _slot1Value;

        ModalManager.Instance.Show(
          "Benar! Lanjutkan dengan huruf kedua.",
          new ModalButtonData("Lanjut", () =>
          {
            AudioManager.Instance.PlayBtnClick();
          })
        );

      }
      else
      {
        HandleWrongDrop(droppedAnswer);
      }
    }
    private void ProcessSecondDrop(DraggableAnswer droppedAnswer, AksaraQuestion currentQuestion) { }
    private void ProcessThirdDrop(DraggableAnswer droppedAnswer, AksaraQuestion currentQuestion) { }

    private void HandleWrongDrop(DraggableAnswer wrongAnswer) { }

    private void OnHintClicked()
    {
      AudioManager.Instance.PlayBtnClick();
      ModalManager.Instance.Show(
        $"Jika kamu buka hint, score berkurang '{Constants.HINT_LEVEL_3}' loh. Yakin?",
        new ModalButtonData("Ya, Buka Hint", () =>
        {
          AudioManager.Instance.PlayBtnClick();
          OnConfirmHint();
        }),
        new ModalButtonData("Batal", () =>
        {
          AudioManager.Instance.PlayBtnClick();
          ModalManager.Instance.Hide();
        })
      );
    }

    private void OnConfirmHint()
    {
      int currentScore = GameManager.Instance.CurrentScore;
      int newScore = Mathf.Max(0, currentScore - Constants.HINT_LEVEL_3);
      int finalScore = currentScore - newScore;

      GameManager.Instance.AddScore(-finalScore);
      UpdateHeaderUI();
      AksaraQuestion currentQuestion = _questions[_currentQuestionIndex];
      ModalManager.Instance.Show(
        $"Huruf pertama dimulai dengan '{currentQuestion.HurufPenyusun[0][0]}'",
        new ModalButtonData("Oke", () =>
        {
          AudioManager.Instance.PlayBtnClick();
          ModalManager.Instance.Hide();
        })
      );

    }

    private void OnExitClicked()
    {
      AudioManager.Instance.PlayBtnClick();
      ModalManager.Instance.Show(
        "Yakin ingin keluar? Progress belajar tidak akan disimpan.",
        new ModalButtonData("Ya, keluar", () =>
        {
          AudioManager.Instance.PlayBtnClick();
          SceneLoader.Instance.LoadScene(Constants.SCENE_MAIN_MENU);
        }),
        new ModalButtonData("Batal", () =>
        {
          AudioManager.Instance.PlayBtnClick();
          ModalManager.Instance.Hide();
        })
      );
    }

    private void OnLevelComplete()
    {
      SceneLoader.Instance.LoadScene(Constants.SCENE_POSTINGAME_ENDGAME);
    }

    private void OnGameOver()
    {
      SaveScoreToHighscore();
      SceneLoader.Instance.LoadScene(Constants.SCENE_POSTINGAME_GAMEOVER);
    }

    private void SaveScoreToHighscore()
    {
      string waktu = System.DateTime.Now.ToString("dd-MM-yyyy");
      HighscoreEntry entry = new HighscoreEntry(
          waktu,
          GameManager.Instance.CurrentScore,
          GameManager.Instance.SessionStartLevel,
          GameManager.Instance.CurrentLevel
      );
      SQLiteService.SaveHighscore(entry);
    }
  }
}
