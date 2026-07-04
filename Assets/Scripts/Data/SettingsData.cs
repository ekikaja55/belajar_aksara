using System;

namespace BelajarAksara.Data
{
  [Serializable]
  public class SettingsData
  {
    public float BgMusicVolume;
    public float SfxVolume;

    public SettingsData()
    {
      BgMusicVolume = 1.0f;
      SfxVolume = 1.0f;
    }

    public SettingsData(float bgMusicVolume, float sfxVolume)
    {
      BgMusicVolume = bgMusicVolume;
      SfxVolume = sfxVolume;
    }
  }
}
