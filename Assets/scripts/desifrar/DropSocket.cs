// DropSocket.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropSocket : MonoBehaviour, IDropHandler
{
    public string acceptedItemId;
    public DraggableItem currentDraggable { get; private set; }
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Debug.Log($"Socket inicializado. Acepta ID: {acceptedItemId}");
    }

    /*   public void OnDrop(PointerEventData eventData)
       {
           Debug.Log($"Drop detectado en socket que acepta: {acceptedItemId}");

           if (eventData.pointerDrag != null)
           {
               DraggableItem draggable = eventData.pointerDrag.GetComponent<DraggableItem>();

               if (draggable != null)
               {
                   Debug.Log($"Draggable encontrado con ID: {draggable.itemId}");

                   // Si hay un elemento previo, devolverlo a su posición inicial
                   if (currentDraggable != null && currentDraggable != draggable)
                   {
                       Debug.Log($"Devolviendo draggable previo ({currentDraggable.itemId}) a su posición inicial");
                       currentDraggable.ReturnToStart();
                   }

                   // Aceptar cualquier draggable y posicionarlo
                   Debug.Log($"Posicionando draggable {draggable.itemId} en socket");
                   draggable.GetComponent<RectTransform>().position = rectTransform.position;
                   currentDraggable = draggable;
                   Debug.Log($"Socket actualizado - Acepta: {acceptedItemId}, Tiene: {currentDraggable.itemId}");
               }
           }
       }*/
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"Drop detectado en socket que acepta: {acceptedItemId}");

        if (eventData.pointerDrag != null)
        {
            DraggableItem draggable = eventData.pointerDrag.GetComponent<DraggableItem>();

            if (draggable != null)
            {
                Debug.Log($"Draggable encontrado con ID: {draggable.itemId}");

                // Asignar el draggable al socket
                EstablecerDraggable(draggable);
            }
            else
            {
                Debug.LogError("El objeto arrastrado no tiene el componente DraggableItem");
            }
        }
        else
        {
            Debug.LogWarning("No se detectó ningún objeto arrastrado");
        }
    }


    public bool ValidarMatch()
    {
        if (currentDraggable == null)
        {
            Debug.Log($"Socket {acceptedItemId}: No hay draggable");
            return false;
        }

        bool isMatch = currentDraggable.itemId == acceptedItemId;
        Debug.Log($"Validando Socket que acepta '{acceptedItemId}' con item '{currentDraggable.itemId}' -> {(isMatch ? "CORRECTO" : "INCORRECTO")}");
        return isMatch;
    }

    public void EstablecerDraggable(DraggableItem draggable)
    {
        // Si hay un elemento previo, devolverlo a su posición inicial
        if (currentDraggable != null && currentDraggable != draggable)
        {
            Debug.Log($"Devolviendo draggable previo ({currentDraggable.itemId}) a su posición inicial");
            currentDraggable.ReturnToStart();
        }

        // Asignar y posicionar el nuevo draggable
        Debug.Log($"Posicionando draggable {draggable.itemId} en socket");
        draggable.GetComponent<RectTransform>().position = rectTransform.position;
        currentDraggable = draggable;
        Debug.Log($"Socket actualizado - Acepta: {acceptedItemId}, Tiene: {currentDraggable.itemId}");
    }

    public void LimpiarSocket()
    {
        if (currentDraggable != null)
        {
            currentDraggable.ReturnToStart();
            currentDraggable = null;
        }
    }
}