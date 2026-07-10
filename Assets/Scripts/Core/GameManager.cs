using UnityEngine;
using BelajarAksara.Utils;
using BelajarAksara.Data;

namespace BelajarAksara.Core
{
  public class GameManager : MonoBehaviour
  {
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int SessionStartLevel = 1;  // level_mulai: di-set SEKALI di awal sesi (PraPlaying1), TIDAK PERNAH berubah lagi
    public int CurrentLevel = 1;        // level_akhir/level SEKARANG: berubah tiap kali pindah ke level berikutnya
    public int CurrentScore = 0;
    public int CurrentLives = Constants.STARTING_LIVES;

    [Header("Level Management")]
    public int HighestUnlockedLevel = 1;

    // adalah singleton, jadi di Awake() kita pastiin cuma ada 1 instance GameManager di scene mana pun
    // Awake() itu bawaan Unity, dipanggil sebelum Start(), dan dipanggil sekali saat object diinisialisasi
    private void Awake()
    {
      // logicnya adalah jika Instance sudah ada dan bukan this, berarti ada GameManager lain di scene, jadi destroy this
      // ibarat kaya bilang "Eh, udah ada GameManager di scene, jadi kamu ga perlu ada lagi, bye!"
      if (Instance != null && Instance != this)
      {
        Destroy(gameObject);
        return;
      }

      // kalo belum ada instance, berarti ini GameManager pertama yang muncul, jadi kita set Instance ke this
      Instance = this;

      // ini adalah function bawaan unity buat mencegah game manager ini YANG ADA DI MAIN MENU KE DESTROY
      DontDestroyOnLoad(gameObject);

      // Inisialisasi database SQLite sekali di awal, sebelum
      // scene manapun butuh baca/tulis data highscore/settings
      SQLiteService.Init();

      LoadProgress();
    }

    private void OnApplicationQuit()
    {
      // Tutup koneksi database dengan rapi saat app ditutup
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
      CurrentLevel = newLevel;
    }
  }
}
