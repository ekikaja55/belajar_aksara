using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BelajarAksara.Managers
{
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
    public GameObject modalPanel;
    public TextMeshProUGUI modalMessage;
    public Transform buttonContainer;
    public GameObject buttonPrefab;

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

      foreach (Transform child in buttonContainer)
      {
        Destroy(child.gameObject);
      }

      foreach (ModalButtonData buttonData in buttons)
      {
        GameObject btnInstance = Instantiate(buttonPrefab, buttonContainer);
        Button btn = btnInstance.GetComponent<Button>();
        TextMeshProUGUI btnText = btnInstance.GetComponentInChildren<TextMeshProUGUI>();

        btnText.text = buttonData.Label;

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
