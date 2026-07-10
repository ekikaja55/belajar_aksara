using UnityEngine;
using UnityEngine.EventSystems;

namespace BelajarAksara.UI
{
  // Nempel di DropBox
  public class AnswerDropZone : MonoBehaviour, IDropHandler
  {
    // Event ini "dipancarkan" ke IngameUI setiap kali ada item di-drop ke sini,
    // supaya IngameUI yang menentukan logic (jawaban benar/salah)
    public System.Action<DraggableAnswer> onAnswerDropped;

    public void OnDrop(PointerEventData eventData)
    {
      GameObject droppedObject = eventData.pointerDrag;
      if (droppedObject == null) return;

      DraggableAnswer draggable = droppedObject.GetComponent<DraggableAnswer>();
      if (draggable == null) return;

      // Kabari dulu ke listener SEBELUM SnapToDropZone,
      // supaya listener (IngameUI) yang menentukan apakah item
      // ini "boleh menempel" atau harus di-reset balik
      onAnswerDropped?.Invoke(draggable);
    }
  }
}
