using UnityEngine;
using BelajarAksara.Utils;
using BelajarAksara.Data;

namespace BelajarAksara.Core
{
  public class GameManager : MonoBehaviour
  {
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int SessionStartLevel = 1;
    public int CurrentLevel = 1;
    public int CurrentScore = 0;
    public int CurrentLives = Constants.STARTING_LIVES;

    [Header("Level Management")]
    public int HighestUnlockedLevel = 1;

    private void Awake()
    {
      if (Instance != null && Instance != this)
      {
        Destroy(gameObject);
        return;
      }

      Instance = this;

      DontDestroyOnLoad(gameObject);

      SQLiteService.Init();

      LoadProgress();
    }

    private void OnApplicationQuit()
    {
      SQLiteService.Close();
    }

    public void StartNewSession(int level)
    {
      CurrentLevel = level;
      SessionStartLevel = level;
      CurrentScore = 0;
      CurrentLives = Constants.STARTING_LIVES;
    }

    public void AddScore(int amount)
    {
      CurrentScore += amount;
    }

    public void LoseLife()
    {
      CurrentLives = Mathf.Max(0, CurrentLives - 1);
    }

    public bool IsGameOver()
    {
      return CurrentLives <= 0;
    }

    public void UnlockNextLevel()
    {
      if (CurrentLevel + 1 > HighestUnlockedLevel)
      {
        HighestUnlockedLevel = CurrentLevel + 1;
        SaveProgress();
      }
    }

    public bool IsLevelUnlocked(int level)
    {
      return level <= HighestUnlockedLevel;
    }

    public void ResetProgress()
    {
      HighestUnlockedLevel = 1;
      SaveProgress();
    }

    private void SaveProgress()
    {
      PlayerPrefs.SetInt(Constants.PREF_UNLOCKED_LEVEL, HighestUnlockedLevel);
      PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
      HighestUnlockedLevel = PlayerPrefs.GetInt(Constants.PREF_UNLOCKED_LEVEL, 1);
    }

    public void AdvanceToLevel(int newLevel)
    {
      Debug.Log($"Advancing to level {newLevel}");
      CurrentLevel = newLevel;
    }
  }
}
