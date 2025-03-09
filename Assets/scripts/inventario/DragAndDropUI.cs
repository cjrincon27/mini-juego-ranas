using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private Vector2 startPosition; // Posición inicial antes de arrastrar
    private Transform originalParent; // Guarda el padre original
    private bool enZonaDrop = false; // Indica si el objeto fue soltado en una zona válida

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();  
        startPosition = rectTransform.anchoredPosition; // Guarda la posición inicial
        originalParent = transform.parent; // Guarda el padre original
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;  // Hace el ítem semitransparente
        canvasGroup.blocksRaycasts = false;  // Evita que interfiera con otros elementos
        transform.SetParent(canvas.transform, true); // Lo saca del Scroll View para moverlo libremente
        enZonaDrop = false; // Reinicia el estado
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;  
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;  // Restaura la opacidad
        canvasGroup.blocksRaycasts = true;  // Vuelve a detectar colisiones

        if (!enZonaDrop)
        {
            // Si no se soltó en una DropZone, vuelve a su posición inicial
            rectTransform.anchoredPosition = startPosition;
            transform.SetParent(originalParent, true);
        }
    }

    // Método para que DropZone indique si el objeto fue soltado en una zona válida
    public void SetEnZonaDrop(bool estado, Transform newParent)
    {
        enZonaDrop = estado;
        if (estado)
        {
            transform.SetParent(newParent, true); // Cambia el padre al DropZone
        }
    }
}
