using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BelajarAksara.Managers
{
  // "Resep" 1 tombol modal: label yang ditampilkan + aksi yang dijalankan saat diklik
  public class ModalButtonData
  {
    public string Label;
    public Action OnClick;

    public ModalButtonData(string label, Action onClick)
    {
      Label = label;
      OnClick = onClick;
    }
  }

  public class ModalManager : MonoBehaviour
  {
    public static ModalManager Instance { get; private set; }

    [Header("References")]
    public GameObject modalPanel;          // GameObject ConfirmModal
    public TextMeshProUGUI modalMessage;
    public Transform buttonContainer;      // BtnGroup, tempat tombol di-generate
    public GameObject buttonPrefab;         // Prefab 1 tombol modal

    private void Awake()
    {
      if (Instance != null && Instance != this)
      {
        Destroy(gameObject);
        return;
      }
      Instance = this;
      DontDestroyOnLoad(transform.root.gameObject);
      modalPanel.SetActive(false);
    }

    public void Show(string message, params ModalButtonData[] buttons)
    {
      Debug.Log($"[ModalManager] Show dipanggil. modalPanel={modalPanel}, modalMessage={modalMessage}, buttonContainer={buttonContainer}, buttonPrefab={buttonPrefab}");
      modalMessage.text = message;

      // Bersihkan tombol-tombol lama sebelum bikin yang baru
      foreach (Transform child in buttonContainer)
      {
        Destroy(child.gameObject);
      }

      // Bikin tombol sesuai jumlah yang diminta pemanggil
      foreach (ModalButtonData buttonData in buttons)
      {
        GameObject btnInstance = Instantiate(buttonPrefab, buttonContainer);
        Button btn = btnInstance.GetComponent<Button>();
        TextMeshProUGUI btnText = btnInstance.GetComponentInChildren<TextMeshProUGUI>();

        btnText.text = buttonData.Label;

        // Simpan referensi lokal supaya closure di bawah ini benar
        // (penting: jangan pakai variable loop langsung di lambda)
        Action onClickAction = buttonData.OnClick;
        btn.onClick.AddListener(() =>
        {
          Hide();
          onClickAction?.Invoke();
        });
      }

      modalPanel.SetActive(true);
    }

    public void Hide()
    {
      modalPanel.SetActive(false);
    }
  }
}
