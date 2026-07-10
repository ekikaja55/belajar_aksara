using UnityEngine;
using System.Collections.Generic;

namespace BelajarAksara.Managers
{
  public class AudioManager : MonoBehaviour
  {
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("BGM")]
    public AudioClip bgmMainTheme;

    [Header("SFX")]
    public AudioClip sfxBtnClick;
    public AudioClip sfxCorrectAnswer;
    public AudioClip sfxWrongAnswer;
    public AudioClip sfxLevelCompleted;
    public AudioClip sfxGameOver;

    private void Awake()
    {
      if (Instance != null && Instance != this)
      {
        Destroy(gameObject);
        return;
      }
      Instance = this;
      DontDestroyOnLoad(transform.root.gameObject);
    }

    private void Start()
    {
      PlayBgm();
      ApplySavedVolumes();
    }

    // ----- BGM -----

    public void PlayBgm()
    {
      if (bgmSource.clip != bgmMainTheme)
      {
        bgmSource.clip = bgmMainTheme;
      }

      if (!bgmSource.isPlaying)
      {
        bgmSource.loop = true;
        bgmSource.Play();
      }
    }

    public void StopBgm()
    {
      bgmSource.Stop();
    }

    // ----- SFX -----

    public void PlayBtnClick() => PlaySfx(sfxBtnClick);
    public void PlayCorrectAnswer() => PlaySfx(sfxCorrectAnswer);
    public void PlayWrongAnswer() => PlaySfx(sfxWrongAnswer);
    public void PlayLevelCompleted() => PlaySfx(sfxLevelCompleted);
    public void PlayGameOver() => PlaySfx(sfxGameOver);

    private void PlaySfx(AudioClip clip)
    {
      if (clip == null) return;
      sfxSource.PlayOneShot(clip);
    }

    // ----- VOLUME -----

    public void SetBgmVolume(float volume)
    {
      bgmSource.volume = volume;
    }

    public void SetSfxVolume(float volume)
    {
      sfxSource.volume = volume;
    }

    private void ApplySavedVolumes()
    {
      Data.SettingsData settings = Data.SQLiteService.GetSettings();
      SetBgmVolume(settings.BgMusicVolume);
      SetSfxVolume(settings.SfxVolume);
    }
  }
}
