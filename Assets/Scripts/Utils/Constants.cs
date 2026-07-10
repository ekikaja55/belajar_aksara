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
    public const string SCENE_TUTORIAL_MAIN_MENU = "14_Tutorial_Main_Menu";

    public const string SCENE_PRAPLAYING_1_SELECT_LEVEL = "04_PraPlaying1_SelectLevel";
    public const string SCENE_PRAPLAYING_2_TUTORCONFIRM = "05_PraPlaying2_TutorConfirm";
    public const string SCENE_PRAPLAYING_3_TUTORIAL = "06_PraPlaying3_Tutorial";
    public const string SCENE_PRAPLAYING_4_DICTIONARY = "07_PraPlaying4_Dictionary";

    public const string SCENE_INGAME_LEVEL_1 = "08_Ingame_Level1";
    public const string SCENE_INGAME_LEVEL_2 = "09_Ingame_Level2";

    // level 3 belum dipakai, jadi sementara dikomen dulu
    // public const string SCENE_INGAME_LEVEL_3 = "10_Ingame_Level3";

    public const string SCENE_POSTINGAME_GAMEOVER = "11_PostIngame1_GameOver";
    public const string SCENE_POSTINGAME_NEXTLEVEL = "12_PostIngame2_NextLevel";
    public const string SCENE_POSTINGAME_ENDGAME = "13_PostIngame3_EndGame";

    // ASSET PREFIX
    public const string ASSET_HURUF_PREFIX = "Asset_Aksara_";
    public const string ASSET_HURUF_SUFFIX = "_wbg";

    //  GAMEPLAY CONSTANTS
    public const int STARTING_LIVES = 5;

    public const int SCORE_LEVEL_1 = 100;
    public const int SCORE_LEVEL_2 = 300;
    // public const int SCORE_LEVEL_3 = 600;

    //level 2 jadi level terakhir dulu, level 3 belum dipakai, jadi sementara dikomen dulu
    public const int TOTAL_LEVELS = 2;

    // PLAYERPREFS KEYS (sementara, sebelum pindah ke SQLite)
    public const string PREF_UNLOCKED_LEVEL = "UnlockedLevel";
    public const string PREF_BGM_VOLUME = "BgmVolume";
    public const string PREF_SFX_VOLUME = "SfxVolume";
  }
}
