using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        DragAndDropUI droppedItem = eventData.pointerDrag?.GetComponent<DragAndDropUI>();

        if (droppedItem != null)
        {
            droppedItem.SetEnZonaDrop(true, transform); // Indica que está en una DropZone
            droppedItem.transform.SetParent(transform, false); // Asigna el DropZone como nuevo padre sin cambiar escala
            droppedItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Centra el objeto en el DropZone
        }
    }
}
