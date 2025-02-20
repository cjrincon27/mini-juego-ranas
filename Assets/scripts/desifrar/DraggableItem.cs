using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;
    public string itemId;
    private bool habilitado;

    [Header("Configuración visual")]
    public Image objetoImagen; // Referencia al objeto Image hijo
    public Sprite spriteHabilitado; // Sprite para el estado habilitado
    public Sprite spriteDeshabilitado; // Sprite para el estado deshabilitado

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        startPosition = rectTransform.position;

        // Verificar si el objeto está habilitado según DatosGlobales
        habilitado = DatosGlobales.nombresGuardados.Contains(itemId);

        // Actualizar el sprite al estado inicial
        ActualizarSprite();
        Debug.Log($"DraggableItem inicializado con ID: {itemId}, Habilitado: {habilitado}");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!habilitado) return; // Evitar arrastre si está deshabilitado
        Debug.Log($"Comenzando a arrastrar item: {itemId}");
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!habilitado) return; // Evitar arrastre si está deshabilitado

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out pos);

        rectTransform.position = canvas.transform.TransformPoint(pos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!habilitado) return; // Evitar lógica si está deshabilitado
        Debug.Log($"Finalizando arrastre de item: {itemId}");
        canvasGroup.blocksRaycasts = true;
    }

    public void ReturnToStart()
    {
        Debug.Log($"Devolviendo item {itemId} a posición inicial");
        rectTransform.position = startPosition;
    }

    // Método para actualizar el estado habilitado/deshabilitado
    public void ActualizarEstado()
    {
        habilitado = DatosGlobales.nombresGuardados.Contains(itemId);
        ActualizarSprite();
        Debug.Log($"Estado del item {itemId} actualizado a: {habilitado}");
    }

    // Cambiar el sprite del objeto según el estado
    private void ActualizarSprite()
    {
        if (objetoImagen != null)
        {
            objetoImagen.sprite = habilitado ? spriteHabilitado : spriteDeshabilitado;
        }
    }

    public void OnDestroy()
    {
        Debug.Log($"Draggable {itemId} destruido");
    }
}
