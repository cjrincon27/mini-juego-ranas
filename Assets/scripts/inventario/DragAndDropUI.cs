using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private Vector2 startPosition; // Posición inicial antes de arrastrar
    private Transform originalParent; // Para evitar que se salga del Scroll View

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
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;  
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;  // Restaura la opacidad
        canvasGroup.blocksRaycasts = true;  // Vuelve a detectar colisiones

        if (!EstaEnZonaCorrecta())
        {
            // Si no se suelta en la zona correcta, vuelve a su posición inicial
            rectTransform.anchoredPosition = startPosition;
            transform.SetParent(originalParent, true); // Regresa al `Content` del Scroll View
        }
    }

    private bool EstaEnZonaCorrecta()
    {
        // Aquí puedes programar una validación si el objeto está en una zona válida
        return false; // Por defecto, regresa siempre a su posición
    }
}
