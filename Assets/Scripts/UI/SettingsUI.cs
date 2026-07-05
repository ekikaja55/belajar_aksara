using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using BelajarAksara.Core;
using BelajarAksara.Data;
using BelajarAksara.Utils;

namespace BelajarAksara.UI
{
  public class SettingsUI : MonoBehaviour
  {
    [Header("Sliders")]
    public Slider sliderBgm;
    public Slider sliderSfx;

    [Header("Main Buttons")]
    public Button btnSave;
    public Button btnBack;

    [Header("Confirm Modal")]
    public GameObject confirmModal;
    public Button btnSaveAndExit;
    public Button btnExitWithoutSave;
    public Button btnCancel;

    [Header("Toast Notification")]
    public GameObject toastNotification;
    public TextMeshProUGUI toastMessage;

    // Menyimpan nilai slider TERAKHIR YANG SUDAH DI-SAVE,
    // dipakai untuk membandingkan apakah ada perubahan yang belum disimpan
    private float _savedBgmValue;
    private float _savedSfxValue;

    // Referensi coroutine toast yang sedang berjalan, biar bisa
    // di-stop kalau toast baru muncul sebelum yang lama selesai
    private Coroutine _toastCoroutine;

    private void Start()
    {
      // 1. Load data settings tersimpan dari SQLite, isi ke slider
      LoadSettingsToUI();

      // 2. Daftarkan semua listener tombol
      btnSave.onClick.AddListener(OnSaveClicked);
      btnBack.onClick.AddListener(OnBackClicked);

      btnSaveAndExit.onClick.AddListener(OnModalSaveAndExit);
      btnExitWithoutSave.onClick.AddListener(OnModalExitWithoutSave);
      btnCancel.onClick.AddListener(OnModalCancel);

      // 3. Pastikan modal & toast tersembunyi di awal
      confirmModal.SetActive(false);
      toastNotification.SetActive(false);
    }

    private void LoadSettingsToUI()
    {
      SettingsData data = SQLiteService.GetSettings();

      sliderBgm.value = data.BgMusicVolume;
      sliderSfx.value = data.SfxVolume;

      // Simpan nilai ini sebagai "baseline" pembanding nanti
      _savedBgmValue = data.BgMusicVolume;
      _savedSfxValue = data.SfxVolume;
    }

    // Cek: apakah nilai slider SEKARANG beda dari nilai yang tersimpan terakhir?
    private bool HasUnsavedChanges()
    {
      // Pakai toleransi kecil (epsilon) karena float tidak selalu pas 100% sama
      float epsilon = 0.001f;
      bool bgmChanged = Mathf.Abs(sliderBgm.value - _savedBgmValue) > epsilon;
      bool sfxChanged = Mathf.Abs(sliderSfx.value - _savedSfxValue) > epsilon;

      return bgmChanged || sfxChanged;
    }

    private void SaveCurrentSettings()
    {
      SettingsData data = new SettingsData(sliderBgm.value, sliderSfx.value);
      SQLiteService.SaveSettings(data);

      // Update baseline setelah save berhasil
      _savedBgmValue = sliderBgm.value;
      _savedSfxValue = sliderSfx.value;

      // TODO nanti: kalau AudioManager sudah ada, panggil di sini
      // untuk langsung apply volume baru ke BGM/SFX yang sedang main
    }

    // ----- MAIN BUTTONS -----

    private void OnSaveClicked()
    {
      SaveCurrentSettings();
      ShowToast("Perubahan disimpan!");
    }

    private void OnBackClicked()
    {
      if (HasUnsavedChanges())
      {
        // Ada perubahan belum disimpan -> tampilkan modal konfirmasi
        confirmModal.SetActive(true);
      }
      else
      {
        // Tidak ada perubahan -> langsung kembali ke menu
        SceneLoader.Instance.LoadScene(Constants.SCENE_MAIN_MENU);
      }
    }

    // ----- MODAL ACTIONS -----

    private void OnModalSaveAndExit()
    {
      SaveCurrentSettings();
      confirmModal.SetActive(false);
      SceneLoader.Instance.LoadScene(Constants.SCENE_MAIN_MENU);
    }

    private void OnModalExitWithoutSave()
    {
      confirmModal.SetActive(false);
      SceneLoader.Instance.LoadScene(Constants.SCENE_MAIN_MENU);
    }

    private void OnModalCancel()
    {
      // Cuma tutup modal, tetap di scene Settings, TIDAK reset slider
      confirmModal.SetActive(false);
    }

    // ----- TOAST NOTIFICATION -----

    private void ShowToast(string message)
    {
      // Kalau ada toast yang masih jalan, hentikan dulu biar nggak numpuk/tabrakan
      if (_toastCoroutine != null)
      {
        StopCoroutine(_toastCoroutine);
      }

      toastMessage.text = message;
      toastNotification.SetActive(true);
      _toastCoroutine = StartCoroutine(HideToastAfterDelay(2f));
    }

    private IEnumerator HideToastAfterDelay(float delay)
    {
      yield return new WaitForSeconds(delay);
      toastNotification.SetActive(false);
      _toastCoroutine = null;
    }
  }
}
