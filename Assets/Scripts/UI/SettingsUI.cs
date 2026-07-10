using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Wajib ditambahkan untuk EventTrigger
using BelajarAksara.Managers;
using BelajarAksara.Data;
using BelajarAksara.Core;
using BelajarAksara.Utils;

namespace BelajarAksara.UI
{
  public class SettingsUI : MonoBehaviour
  {
    [Header("UI Elements")]
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Button btnSave;
    public Button btnKeluar;

    // Backup nilai asli
    private float _originalBgmVolume;
    private float _originalSfxVolume;

    private void Start()
    {
      SettingsData savedSettings = SQLiteService.GetSettings();

      _originalBgmVolume = savedSettings.BgMusicVolume;
      _originalSfxVolume = savedSettings.SfxVolume;

      bgmSlider.value = _originalBgmVolume;
      sfxSlider.value = _originalSfxVolume;

      // 1. Mendaftarkan fungsi ubah volume secara real-time
      bgmSlider.onValueChanged.AddListener(PreviewBgmVolume);
      sfxSlider.onValueChanged.AddListener(PreviewSfxVolume);

      // 2. Mendaftarkan fungsi pemutaran suara HANYA saat jari dilepas
      SetupSfxPreviewTrigger();

      btnSave.onClick.AddListener(OnSaveClicked);
      btnKeluar.onClick.AddListener(OnKeluarClicked);
    }

    // ----- PREVIEW VOLUME (Hanya mengubah angka, BUKAN memutar suara) -----
    private void PreviewBgmVolume(float volume)
    {
      if (AudioManager.Instance != null)
        AudioManager.Instance.bgmSource.volume = volume;
    }

    private void PreviewSfxVolume(float volume)
    {
      if (AudioManager.Instance != null)
        AudioManager.Instance.sfxSource.volume = volume;
    }

    // ----- EVENT TRIGGER (Untuk menghindari suara menumpuk/earrape) -----
    private void SetupSfxPreviewTrigger()
    {
      // Tambahkan komponen EventTrigger ke sfxSlider lewat kode
      EventTrigger trigger = sfxSlider.gameObject.GetComponent<EventTrigger>();
      if (trigger == null)
        trigger = sfxSlider.gameObject.AddComponent<EventTrigger>();

      // Buat event khusus untuk PointerUp (saat layar/mouse dilepas)
      EventTrigger.Entry entry = new EventTrigger.Entry
      {
        eventID = EventTriggerType.PointerUp
      };

      entry.callback.AddListener((data) => { PlaySfxPreviewSound(); });
      trigger.triggers.Add(entry);
    }

    private void PlaySfxPreviewSound()
    {
      // Suara baru diputar satu kali setelah pemain selesai menggeser
      if (AudioManager.Instance != null)
        AudioManager.Instance.PlayBtnClick();
    }

    // ----- PENYIMPANAN -----
    private void OnSaveClicked()
    {
      SettingsData newData = new SettingsData
      {
        BgMusicVolume = bgmSlider.value,
        SfxVolume = sfxSlider.value
      };
      SQLiteService.SaveSettings(newData);

      _originalBgmVolume = bgmSlider.value;
      _originalSfxVolume = sfxSlider.value;

      ModalManager.Instance.Show(
          "Pengaturan berhasil disimpan!",
          new ModalButtonData("Oke", () => { ModalManager.Instance.Hide(); })
      );
    }

    // ----- KELUAR & VALIDASI PERUBAHAN -----
    private void OnKeluarClicked()
    {
      // Cek apakah nilai slider saat ini berbeda dari nilai asli
      // (Menggunakan Mathf.Abs karena membandingkan tipe float langsung kadang tidak akurat)
      bool isBgmChanged = Mathf.Abs(bgmSlider.value - _originalBgmVolume) > 0.01f;
      bool isSfxChanged = Mathf.Abs(sfxSlider.value - _originalSfxVolume) > 0.01f;

      if (isBgmChanged || isSfxChanged)
      {
        // Trigger modal karena ada yang belum disave
        ModalManager.Instance.Show(
            "Ada perubahan yang belum disimpan. Yakin ingin keluar?",
            new ModalButtonData("Ya, Keluar", ProceedExit),
            new ModalButtonData("Batal", () => { ModalManager.Instance.Hide(); })
        );
      }
      else
      {
        // Tidak ada perubahan, langsung keluar dengan aman
        ProceedExit();
      }
    }

    private void ProceedExit()
    {
      // REVERT: Kembalikan volume AudioManager ke nilai aslinya
      if (AudioManager.Instance != null)
      {
        AudioManager.Instance.bgmSource.volume = _originalBgmVolume;
        AudioManager.Instance.sfxSource.volume = _originalSfxVolume;
      }

      SceneLoader.Instance.LoadScene(Constants.SCENE_MAIN_MENU);
    }
  }
}
