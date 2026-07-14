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

    // State per slot: nilai huruf yang sudah benar + reference draggable-nya
    // (dipakai untuk "unhide" balik kalau ada slot berikutnya yang salah)
    private string[] _slotValues = new string[3];
    private DraggableAnswer[] _slotDraggables = new DraggableAnswer[3];

    private bool _isProcessingAnswer = false;
    private int _stepCount = 0; // 0 = belum ada drop, 1/2/3 = drop ke berapa yang SEDANG diproses

    void Start()
    {
      _questions = AksaraDatabase.GetQuestionsByLevel(LEVEL_NUMBER);
      btnExit.onClick.AddListener(OnExitClicked);
      btnHint.onClick.AddListener(OnHintClicked);

      dropZone.onAnswerDropped = OnAnswerDropped;
      RenderCurrentQuestion();
      UpdateHeaderUI();
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

      while (pengecoh < 3 && idx < level1Letters.Length)
      {
        string candidate = level1Letters[idx].HurufPenyusun[0];
        bool isPartOfAnswer = candidate == currentQuestion.HurufPenyusun[0]
                            || candidate == currentQuestion.HurufPenyusun[1]
                            || candidate == currentQuestion.HurufPenyusun[2];

        if (!isPartOfAnswer)
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
      for (int i = 0; i < 3; i++)
      {
        if (_slotDraggables[i] != null)
        {
          _slotDraggables[i].ResetToOriginalPosition();
        }
        _slotValues[i] = null;
        _slotDraggables[i] = null;
      }

      _stepCount = 0;
      titleDropBox.text = "Taruh Disini";
    }

    private void OnAnswerDropped(DraggableAnswer droppedAnswer)
    {
      if (_isProcessingAnswer) return;
      _isProcessingAnswer = true;

      AksaraQuestion currentQuestion = _questions[_currentQuestionIndex];

      // Drop yang sedang diproses adalah step berikutnya dari yang terakhir
      int stepToProcess = _stepCount + 1;
      bool isCorrect = droppedAnswer.letterValue == currentQuestion.HurufPenyusun[stepToProcess - 1];

      if (isCorrect)
      {
        ProcessCorrectDrop(droppedAnswer, stepToProcess);
      }
      else
      {
        HandleWrongDrop(droppedAnswer);
      }

      _isProcessingAnswer = false;
    }

    private void ProcessCorrectDrop(DraggableAnswer droppedAnswer, int step)
    {
      AudioManager.Instance.PlayCorrectAnswer();

      int slotIndex = step - 1; // step 1 -> index 0, step 2 -> index 1, dst
      _slotValues[slotIndex] = droppedAnswer.letterValue;
      _slotDraggables[slotIndex] = droppedAnswer;

      droppedAnswer.SnapToDropZone(dropZone.transform);

      // Update tampilan drop box dengan huruf-huruf yang sudah benar sejauh ini
      titleDropBox.text = string.Join("", _slotValues, 0, step);

      _stepCount = step;

      if (step < 3)
      {
        string stepLabel = step == 1 ? "kedua" : "ketiga";
        ModalManager.Instance.Show(
            $"Benar! Lanjutkan dengan huruf {stepLabel}.",
            new ModalButtonData("Lanjut", () =>
            {
              AudioManager.Instance.PlayBtnClick();
            })
        );
      }
      else
      {
        // Step 3 selesai -> jawaban lengkap benar
        ModalManager.Instance.Show(
            "Jawaban benar! Skor bertambah!",
            new ModalButtonData("Lanjut", () =>
            {
              AudioManager.Instance.PlayBtnClick();
              HandleQuestionComplete();
            })
        );
      }
    }

    private void HandleWrongDrop(DraggableAnswer wrongAnswer)
    {
      AudioManager.Instance.PlayWrongAnswer();

      GameManager.Instance.LoseLife();
      UpdateHeaderUI();

      wrongAnswer.ResetToOriginalPosition();
      ResetSlots();

      if (GameManager.Instance.IsGameOver())
      {
        ModalManager.Instance.Show(
            "Nyawa habis!",
            new ModalButtonData("Oke", () =>
            {
              AudioManager.Instance.PlayBtnClick();
              OnGameOver();
            })
        );
      }
      else
      {
        ModalManager.Instance.Show(
            "Salah! Jawaban direset, coba lagi dari awal.",
            new ModalButtonData("Oke", () =>
            {
              AudioManager.Instance.PlayBtnClick();
            })
        );
      }
    }

    private void HandleQuestionComplete()
    {
      GameManager.Instance.AddScore(Constants.SCORE_LEVEL_3);
      UpdateHeaderUI();

      _currentQuestionIndex++;

      if (_currentQuestionIndex >= _questions.Length)
      {
        OnLevelComplete();
      }
      else
      {
        RenderCurrentQuestion();
      }
    }

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
          })
      );
    }

    private void OnConfirmHint()
    {
      int currentScore = GameManager.Instance.CurrentScore;
      int newScore = Mathf.Max(0, currentScore - Constants.HINT_LEVEL_3);
      int actualDeduction = currentScore - newScore;

      GameManager.Instance.AddScore(-actualDeduction);
      UpdateHeaderUI();

      AksaraQuestion currentQuestion = _questions[_currentQuestionIndex];

      // Hint dinamis sesuai step yang sedang berjalan
      int hintStepIndex = Mathf.Min(_stepCount, 2); // step 0/1/2 -> index huruf yang relevan
      string[] stepLabels = { "pertama", "kedua", "ketiga" };

      string hintMessage = $"Huruf {stepLabels[hintStepIndex]} dimulai dengan '{currentQuestion.HurufPenyusun[hintStepIndex][0]}'";

      ModalManager.Instance.Show(
          hintMessage,
          new ModalButtonData("Oke", () =>
          {
            AudioManager.Instance.PlayBtnClick();
          })
      );
    }

    private void OnExitClicked()
    {
      AudioManager.Instance.PlayBtnClick();
      ModalManager.Instance.Show(
          "Yakin ingin keluar? Progress belajar tidak akan disimpan.",
          new ModalButtonData("Ya, Keluar", () =>
          {
            AudioManager.Instance.PlayBtnClick();
            SceneLoader.Instance.LoadScene(Constants.SCENE_MAIN_MENU);
          }),
          new ModalButtonData("Batal", () =>
          {
            AudioManager.Instance.PlayBtnClick();
          })
      );
    }

    private void OnLevelComplete()
    {
      AudioManager.Instance.PlayLevelCompleted();
      SaveScoreToHighscore();
      SceneLoader.Instance.LoadScene(Constants.SCENE_POSTINGAME_ENDGAME);
    }

    private void OnGameOver()
    {
      AudioManager.Instance.PlayGameOver();
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
