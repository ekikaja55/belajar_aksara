using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BelajarAksara.Core;
using BelajarAksara.Data;
using BelajarAksara.Managers;
using BelajarAksara.Utils;

namespace BelajarAksara.UI
{
  public class IngameUI2 : MonoBehaviour
  {
    [Header("Header Info")]
    public TMPro.TextMeshProUGUI textScore;
    public TMPro.TextMeshProUGUI textLives;

    [Header("Question")]
    public Image imageQuestion1;
    public Image imageQuestion2;

    [Header("Answer Choices")]
    public DraggableAnswer[] answerChoices; // 6 pilihan
    public TMPro.TextMeshProUGUI[] answerChoiceTexts; // 6 text, urutan sama dengan answerChoices

    [Header("Drop Zone")]
    public AnswerDropZone dropZone;
    public TMPro.TextMeshProUGUI titleDropBox; // feedback teks di drop box

    [Header("Navigation")]
    public Button btnExit;
    public Button btnHint;

    private const int LEVEL_NUMBER = 2;

    private int _currentQuestionIndex = 0;
    private AksaraQuestion[] _questions;

    // State slot untuk 2 huruf yang sedang di-drop
    private string _slot1Value = null;
    private DraggableAnswer _slot1Draggable = null;

    private bool _isProcessingAnswer = false;

    private void Start()
    {
      _questions = AksaraDatabase.GetQuestionsByLevel(LEVEL_NUMBER);

      btnExit.onClick.AddListener(OnExitClicked);
      btnHint.onClick.AddListener(OnHintClicked);

      dropZone.onAnswerDropped = OnAnswerDropped;

      RenderCurrentQuestion();
      UpdateHeaderUI();
    }

    // ----- RENDER SOAL -----

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

      List<string> choices = GetSixChoices(currentQuestion);

      for (int i = 0; i < answerChoices.Length; i++)
      {
        string letter = choices[i];
        answerChoices[i].letterValue = letter;
        answerChoiceTexts[i].text = letter;
        answerChoices[i].ResetToOriginalPosition();
      }
    }

    // 6 pilihan: 2 huruf jawaban benar (HurufPenyusun[0] & [1]) +
    // 4 huruf pengecoh diambil dari huruf-huruf lain di Level 1
    // (tidak random urutannya -- diambil berurutan dari AksaraDatabase.Level1,
    private List<string> GetSixChoices(AksaraQuestion currentQuestion)
    {
      List<string> result = new List<string>
            {
                currentQuestion.HurufPenyusun[0],
                currentQuestion.HurufPenyusun[1]
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

    // ----- JAWABAN (2 SLOT) -----

    private void OnAnswerDropped(DraggableAnswer droppedAnswer)
    {
      if (_isProcessingAnswer) return;
      _isProcessingAnswer = true;

      AksaraQuestion currentQuestion = _questions[_currentQuestionIndex];

      if (_slot1Value == null)
      {
        // Ini drop PERTAMA
        ProcessFirstDrop(droppedAnswer, currentQuestion);
      }
      else
      {
        // Ini drop KEDUA
        ProcessSecondDrop(droppedAnswer, currentQuestion);
      }

      _isProcessingAnswer = false;
    }

    private void ProcessFirstDrop(DraggableAnswer droppedAnswer, AksaraQuestion currentQuestion)
    {
      bool isCorrect = droppedAnswer.letterValue == currentQuestion.HurufPenyusun[0];

      if (isCorrect)
      {
        AudioManager.Instance.PlayCorrectAnswer();
        _slot1Value = droppedAnswer.letterValue;
        _slot1Draggable = droppedAnswer;

        droppedAnswer.SnapToDropZone(dropZone.transform);
        titleDropBox.text = _slot1Value; // tampilkan huruf pertama yang sudah benar

        ModalManager.Instance.Show(
        "Benar! Lanjutkan dengan huruf kedua.",
            new ModalButtonData("Lanjut", () =>
            {
              AudioManager.Instance.PlayBtnClick();
              ModalManager.Instance.Hide();
            })
        );
      }
      else
      {
        HandleWrongDrop(droppedAnswer);
      }
    }

    private void ProcessSecondDrop(DraggableAnswer droppedAnswer, AksaraQuestion currentQuestion)
    {
      bool isCorrect = droppedAnswer.letterValue == currentQuestion.HurufPenyusun[1];

      if (isCorrect)
      {
        AudioManager.Instance.PlayCorrectAnswer();
        droppedAnswer.SnapToDropZone(dropZone.transform);

        ModalManager.Instance.Show(
            "Jawaban benar! Skor bertambah!",
            new ModalButtonData("Lanjut", () =>
            {
              HandleCorrectAnswer();
              ModalManager.Instance.Hide();
            })
        );
      }
      else
      {
        HandleWrongDrop(droppedAnswer);
      }
    }

    private void HandleWrongDrop(DraggableAnswer wrongAnswer)
    {
      GameManager.Instance.LoseLife();
      UpdateHeaderUI();

      wrongAnswer.ResetToOriginalPosition();

      // Reset SEMUA slot, termasuk huruf pertama yang sudah benar sebelumnya
      ResetSlots();

      if (GameManager.Instance.IsGameOver())
      {
        AudioManager.Instance.PlayGameOver();
        ModalManager.Instance.Show(
            "Nyawa habis!",
            new ModalButtonData("Oke", () =>
            {
              AudioManager.Instance.PlayBtnClick();
              ModalManager.Instance.Hide();
              OnGameOver();
            })
        );
      }
      else
      {
        AudioManager.Instance.PlayWrongAnswer();
        ModalManager.Instance.Show(
            "Salah! Jawaban direset, coba lagi dari awal.",
            new ModalButtonData("Oke", () =>
            {
              AudioManager.Instance.PlayBtnClick();
              ModalManager.Instance.Hide();
            })
        );
      }
    }

    private void ResetSlots()
    {
      // Kalau ada draggable yang sudah menempel di slot 1, balikkan ke posisi asal
      if (_slot1Draggable != null)
      {
        _slot1Draggable.ResetToOriginalPosition();
      }

      _slot1Value = null;
      _slot1Draggable = null;
      titleDropBox.text = "Taruh Disini";
    }

    private void HandleCorrectAnswer()
    {
      GameManager.Instance.AddScore(Constants.SCORE_LEVEL_2);
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

    // ----- UI UPDATE -----

    private void UpdateHeaderUI()
    {
      textScore.text = GameManager.Instance.CurrentScore.ToString();
      textLives.text = GameManager.Instance.CurrentLives.ToString();
    }

    // ----- HINT -----

    private void OnHintClicked()
    {
      AudioManager.Instance.PlayBtnClick();
      ModalManager.Instance.Show(
        $"Jika kamu buka hint, score berkurang '{Constants.HINT_LEVEL_2}'  loh. Yakin?",
          new ModalButtonData("Ya, Buka Hint", OnConfirmHint),
          new ModalButtonData("Batal", () =>
          {
            AudioManager.Instance.PlayBtnClick();
            ModalManager.Instance.Hide();
          })
      );
    }

    private void OnConfirmHint()
    {
      AudioManager.Instance.PlayBtnClick();
      int currentScore = GameManager.Instance.CurrentScore;
      int newScore = Mathf.Max(0, currentScore - Constants.HINT_LEVEL_2);
      int actualDeduction = currentScore - newScore;

      GameManager.Instance.AddScore(-actualDeduction);
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

    // ----- EXIT -----

    private void OnExitClicked()
    {
      AudioManager.Instance.PlayBtnClick();
      ModalManager.Instance.Show(
          "Yakin ingin keluar? Progress belajar tidak akan disimpan.",
          new ModalButtonData("Ya, Keluar", OnConfirmExit),
          new ModalButtonData("Batal", () =>
          {
            AudioManager.Instance.PlayBtnClick();
            ModalManager.Instance.Hide();
          })
      );
    }

    private void OnConfirmExit()
    {
      AudioManager.Instance.PlayBtnClick();
      SceneLoader.Instance.LoadScene(Constants.SCENE_MAIN_MENU);
    }

    // ----- LEVEL TRANSITIONS -----

    private void OnLevelComplete()
    {
      SaveScoreToHighscore();
      SceneLoader.Instance.LoadScene(Constants.SCENE_POSTINGAME_NEXTLEVEL);
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
