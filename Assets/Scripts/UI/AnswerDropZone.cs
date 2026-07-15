using UnityEngine;
using UnityEngine.EventSystems;

namespace BelajarAksara.UI
{
  public class AnswerDropZone : MonoBehaviour, IDropHandler
  {
    public System.Action<DraggableAnswer> onAnswerDropped;

    public void OnDrop(PointerEventData eventData)
    {
      GameObject droppedObject = eventData.pointerDrag;
      if (droppedObject == null) return;

      DraggableAnswer draggable = droppedObject.GetComponent<DraggableAnswer>();
      if (draggable == null) return;

      onAnswerDropped?.Invoke(draggable);
    }
  }
}
