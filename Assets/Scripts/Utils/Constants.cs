namespace BelajarAksara.Utils
{
  public static class Constants
  {
    // SCENE NAMES
    // Kostanta ini buat pindah scene secara dinamis, biar ga hardcode di script lain
    public const string SCENE_MAIN_MENU = "00_MainMenu";
    public const string SCENE_HIGHSCORE = "01_Highscore";
    public const string SCENE_SETTINGS = "02_Settings";
    public const string SCENE_ABOUT = "03_About";
    public const string SCENE_PRAPLAYING_1_SELECT_LEVEL = "04_PraPlaying1_SelectLevel";
    public const string SCENE_PRAPLAYING_2_TUTORIAL = "05_PraPlaying2_Tutorial";  
    public const string SCENE_PRAPLAYING_3_DICTIONARY = "06_PraPlaying3_Dictionary";
    public const string SCENE_INGAME_LEVEL_1 = "07_Ingame_Level1";
    public const string SCENE_INGAME_LEVEL_2 = "08_Ingame_Level2";
    public const string SCENE_INGAME_LEVEL_3 = "09_Ingame_Level3";
    public const string SCENE_POSTINGAME_GAMEOVER = "10_PostIngame1_GameOver";
    public const string SCENE_POSTINGAME_NEXTLEVEL = "11_PostIngame2_NextLevel";
    public const string SCENE_POSTINGAME_ENDGAME = "12_PostIngame3_EndGame";

    // ASSET PREFIX
    public const string ASSET_HURUF_PREFIX = "asset_";

    //  GAMEPLAY CONSTANTS
    public const int STARTING_LIVES = 5;

    public const int SCORE_LEVEL_1 = 100;
    public const int SCORE_LEVEL_2 = 300;
    public const int SCORE_LEVEL_3 = 600;

    public const int TOTAL_LEVELS = 3;

    // PLAYERPREFS KEYS (sementara, sebelum pindah ke SQLite)
    public const string PREF_UNLOCKED_LEVEL = "UnlockedLevel";
    public const string PREF_BGM_VOLUME = "BgmVolume";
    public const string PREF_SFX_VOLUME = "SfxVolume";
  }
}
