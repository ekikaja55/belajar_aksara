using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace BelajarAksara.Managers
{
  // public enum ToastType { Info, Success, Error }
  public enum ToastPosition { Center, TopCenter }
  public class ToastManager : MonoBehaviour
  {
    public static ToastManager Instance { get; private set; }

    [Header("References")]
    public GameObject toastVisual;
    public Image toastBackground;      // Komponen Image di ToastManager, buat ganti warna
    public TextMeshProUGUI toastMessage;
    public RectTransform toastRectTransform;

    [Header("Position Presets")]
    public Vector2 centerPosition = new Vector2(0, 0);
    public Vector2 topCenterPosition = new Vector2(0, 350);

    // [Header("Colors per Type")]
    // public Color colorInfo = new Color(0.2f, 0.4f, 0.8f);
    // public Color colorSuccess = new Color(0.2f, 0.7f, 0.3f);
    // public Color colorError = new Color(0.8f, 0.2f, 0.2f);

    private Coroutine _activeCoroutine;

    private void Awake()
    {
      if (Instance != null && Instance != this)
      {
        Destroy(gameObject);
        return;
      }
      Instance = this;
      DontDestroyOnLoad(transform.root.gameObject);
      toastVisual.SetActive(false);
    }

    public void Show(string message, float duration = 2.5f, ToastPosition post = ToastPosition.TopCenter)
    {
      Debug.Log($"[ToastManager] Show dipanggil. toastVisual={toastVisual}, toastBackground={toastBackground}, toastMessage={toastMessage}");

      if (_activeCoroutine != null)
      {
        StopCoroutine(_activeCoroutine);
      }

      toastRectTransform.anchoredPosition = GetPositionValue(post);
      toastMessage.text = message;
      // toastBackground.color = GetColorForType(type);
      toastVisual.SetActive(true);

      _activeCoroutine = StartCoroutine(HideAfterDelay(duration));
    }


    private Vector2 GetPositionValue(ToastPosition position)
    {
      switch (position)
      {
        case ToastPosition.Center: return centerPosition;
        case ToastPosition.TopCenter: return topCenterPosition;
        default: return topCenterPosition;
      }
    }

    // private Color GetColorForType(ToastType type)
    // {
    //   switch (type)
    //   {
    //     case ToastType.Success: return colorSuccess;
    //     case ToastType.Error: return colorError;
    //     default: return colorInfo;
    //   }
    // }

    private IEnumerator HideAfterDelay(float delay)
    {
      yield return new WaitForSeconds(delay);
      toastVisual.SetActive(false);
      _activeCoroutine = null;
    }
  }
}
