using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BelajarAksara.UI
{
  public class DraggableAnswer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
  {
    [HideInInspector]
    public string letterValue;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Canvas _rootCanvas;

    private Vector2 _originalAnchoredPosition;
    private Transform _originalParent;

    private void Awake()
    {
      _rectTransform = GetComponent<RectTransform>();
      _canvasGroup = GetComponent<CanvasGroup>();
      _rootCanvas = GetComponentInParent<Canvas>();

      // Capture posisi asli SEKALI di awal, bukan cuma pas mulai drag
      _originalParent = transform.parent;
      _originalAnchoredPosition = _rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
      // Hapus baris capture posisi di sini, karena sudah dilakukan di Awake()
      transform.SetParent(_rootCanvas.transform, true);
      _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
      _rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      _canvasGroup.blocksRaycasts = true;

      if (transform.parent == _rootCanvas.transform)
      {
        SnapBack();
      }
    }

    private void SnapBack()
    {
      transform.SetParent(_originalParent, true);
      _rectTransform.anchoredPosition = _originalAnchoredPosition;
    }

    public void SnapToDropZone(Transform dropZoneTransform)
    {
      transform.SetParent(dropZoneTransform, true);
      _rectTransform.anchoredPosition = new Vector2(3000, 3000); // Center in the drop zone
    }

    public void ResetToOriginalPosition()
    {
      SnapBack();
    }
  }
}
