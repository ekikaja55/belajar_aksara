using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BelajarAksara.Core;
using BelajarAksara.Data;
using BelajarAksara.Managers;
using BelajarAksara.Utils;

namespace BelajarAksara.UI
{
  public class IngameUI : MonoBehaviour
  {
    [Header("Header Info")]
    public TMPro.TextMeshProUGUI textScore;
    public TMPro.TextMeshProUGUI textLives;

    [Header("Question")]
    public Image imageQuestion; // ImageQuestion, sprite huruf besar

    [Header("Answer Choices")]
    public DraggableAnswer answerChoice1;
    public DraggableAnswer answerChoice2;
    public DraggableAnswer answerChoice3;
    public TMPro.TextMeshProUGUI answerChoice1Text;
    public TMPro.TextMeshProUGUI answerChoice2Text;
    public TMPro.TextMeshProUGUI answerChoice3Text;

    [Header("Drop Zone")]
    public AnswerDropZone dropZone;

    [Header("Navigation")]
    public Button btnExit;
    public Button btnHint;

    [Header("UI Control")]
    public CanvasGroup answerPanelGroup;
    public float startDelay = 0.5f; // Jeda sebelum pemain bisa berinteraksi

    [Header("Level Config")]
    public int levelNumber = 1; // diisi beda-beda per scene (1, 2, atau 3)

    // Index soal yang sedang aktif, dari 0 s.d. total soal - 1
    private int _currentQuestionIndex = 0;
    private AksaraQuestion[] _questions;

    private void Start()
    {

      _questions = AksaraDatabase.GetQuestionsByLevel(levelNumber);

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

      AksaraQuestion currentQuestion = _questions[_currentQuestionIndex];

      // Tampilkan sprite soal
      imageQuestion.sprite = LoadHurufSprite(currentQuestion.HurufPenyusun[0]);

      // Ambil 3 pilihan jawaban: 3 huruf berikutnya dari antrian
      // (termasuk jawaban benar untuk soal ini)
      List<string> choices = GetNextThreeChoices();

      SetupAnswerChoice(answerChoice1, answerChoice1Text, choices[0]);
      SetupAnswerChoice(answerChoice2, answerChoice2Text, choices[1]);
      SetupAnswerChoice(answerChoice3, answerChoice3Text, choices[2]);
    }

    private List<string> GetNextThreeChoices()
    {
      List<string> result = new List<string>();

      for (int i = 0; i < 3; i++)
      {
        int index = _currentQuestionIndex + i;

        // Kalau index melebihi batas, cycle balik ke awal array
        // (modulo), supaya selalu dapat 3 pilihan valid
        int safeIndex = index % _questions.Length;
        result.Add(_questions[safeIndex].HurufPenyusun[0]);
      }

      return result;
    }

    private void SetupAnswerChoice(DraggableAnswer draggable, TMPro.TextMeshProUGUI text, string letter)
    {
      draggable.letterValue = letter;
      text.text = letter;
      draggable.ResetToOriginalPosition();
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

    // ----- JAWABAN -----

    private bool _isProcessingAnswer = false;

    private void OnAnswerDropped(DraggableAnswer droppedAnswer)
    {
      if (_isProcessingAnswer) return; // abaikan kalau masih proses jawaban sebelumnya
      _isProcessingAnswer = true;

      AksaraQuestion currentQuestion = _questions[_currentQuestionIndex];
      string correctAnswer = currentQuestion.HurufPenyusun[0];
      bool isCorrect = droppedAnswer.letterValue == correctAnswer;

      if (isCorrect)
      {
        AudioManager.Instance.PlayCorrectAnswer();
        droppedAnswer.SnapToDropZone(dropZone.transform);
        ModalManager.Instance.Show(
            "Jawaban benar! Skor bertambah!",
            new ModalButtonData("Lanjut", () =>
            {
              AudioManager.Instance.PlayBtnClick();
              HandleCorrectAnswer();
              ModalManager.Instance.Hide();
            })
        );
      }
      else
      {
        AudioManager.Instance.PlayWrongAnswer();
        droppedAnswer.SnapToDropZone(dropZone.transform);
        ModalManager.Instance.Show(
            "Jawaban salah! Coba lagi.",
            new ModalButtonData("Oke", () =>
            {
              AudioManager.Instance.PlayBtnClick();
              HandleWrongAnswer(droppedAnswer);
              ModalManager.Instance.Hide();
            })
        );
      }

      _isProcessingAnswer = false;
    }

    private void HandleCorrectAnswer()
    {

      GameManager.Instance.AddScore(Constants.SCORE_LEVEL_1);
      UpdateHeaderUI();

      _currentQuestionIndex++;

      if (_currentQuestionIndex >= _questions.Length)
      {
        // Soal habis -> level selesai
        OnLevelComplete();
      }
      else
      {
        RenderCurrentQuestion();
      }
    }

    private void HandleWrongAnswer(DraggableAnswer wrongAnswer)
    {
      GameManager.Instance.LoseLife();

      UpdateHeaderUI();

      // Balikkan item yang salah ke posisi asalnya
      wrongAnswer.ResetToOriginalPosition();

      if (GameManager.Instance.IsGameOver())
      {
        OnGameOver();
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
          "Jika kamu buka hint, skoremu berkurang 50 loh. Yakin?",
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
      // Kurangi skor 50, tapi jangan sampai minus
      int currentScore = GameManager.Instance.CurrentScore;
      int newScore = Mathf.Max(0, currentScore - 50);
      int actualDeduction = currentScore - newScore;

      GameManager.Instance.AddScore(-actualDeduction);
      UpdateHeaderUI();

      ShowHint();
    }



    private void ShowHint()
    {
      // TODO: logic hint sesungguhnya (misal highlight jawaban benar
      // di salah satu AnswerChoice, atau kasih petunjuk visual lain)
      AksaraQuestion currentQuestion = _questions[_currentQuestionIndex];
      ModalManager.Instance.Show(
          $"Huruf ini dimulai dengan '{currentQuestion.HurufPenyusun[0][0]}'",
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
      AudioManager.Instance.PlayBtnClick();
      GameManager.Instance.UnlockNextLevel();
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
          GameManager.Instance.SessionStartLevel, // level_mulai
          GameManager.Instance.CurrentLevel         // level_akhir
      );
      SQLiteService.SaveHighscore(entry);
    }
  }
}
