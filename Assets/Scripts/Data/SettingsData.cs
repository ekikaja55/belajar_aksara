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
      BgMusicVolume = 0.5f;
      SfxVolume = 0.5f;
    }

    public SettingsData(float bgMusicVolume, float sfxVolume)
    {
      BgMusicVolume = bgMusicVolume;
      SfxVolume = sfxVolume;
    }
  }
}
