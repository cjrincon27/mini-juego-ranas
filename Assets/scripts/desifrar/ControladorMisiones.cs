using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro; // Añade esta línea al inicio con los otros using

[System.Serializable]
public class Mision
{
    public string nombreMision;
    public string descripcionMision;
    public List<DropSocket> sockets = new List<DropSocket>();
    public List<DraggableItem> draggables = new List<DraggableItem>();
    public bool estaCompletada;
    public UnityEvent onMisionCompletada;
}

public class ControladorMisiones : MonoBehaviour
{
    [Header("Configuración de Misiones")]
    [SerializeField] private List<Mision> misiones = new List<Mision>();

    [Header("Referencias UI")]
    [SerializeField] private Transform panelSockets;
    [SerializeField] private Transform panelDraggables;
    [SerializeField] private TextMeshProUGUI textoNombreMision;  // Cambiado
    [SerializeField] private TextMeshProUGUI textoDescripcionMision;  // Cambiado
    [SerializeField] private Button botonSiguienteMision;

    [Header("UI de Validación")]
    [SerializeField] private Button botonValidar;
    [SerializeField] private TextMeshProUGUI textoRetroalimentacion;  // Cambiado

    [Header("Eventos")]
    public UnityEvent onMisionCargada;
    public UnityEvent onTodasMisionesCompletadas;

    private int indiceMisionActual = 0;
    private List<GameObject> objetosInstanciados = new List<GameObject>();

    public int TotalMisiones => misiones.Count;
    public int MisionActual => indiceMisionActual + 1;
    public bool TodasMisionesCompletadas => misiones.TrueForAll(m => m.estaCompletada);


    void Start()
    {
        InicializarUI();
        CargarMision(indiceMisionActual);
    }

    private void InicializarUI()
    {
        if (botonSiguienteMision != null)
        {
            botonSiguienteMision.onClick.AddListener(SiguienteMision);
            ActualizarEstadoBotonSiguiente();
        }

        if (botonValidar != null)
        {
            botonValidar.onClick.AddListener(ValidarRespuestas);
        }
    }

    public void ValidarRespuestas()
    {
        if (indiceMisionActual >= misiones.Count) return;

        Debug.Log("=== Iniciando validación de respuestas ===");
        Mision misionActual = misiones[indiceMisionActual];
        bool todasCorrectas = true;
        int socketIndex = 0;

        foreach (DropSocket socket in misionActual.sockets)
        {
            string draggableId = socket.currentDraggable != null ? socket.currentDraggable.itemId : "vacío";
            Debug.Log($"Socket {socketIndex} - Espera: {socket.acceptedItemId}, Tiene: {draggableId}");
            socketIndex++;
        }

        socketIndex = 0;
        foreach (DropSocket socket in misionActual.sockets)
        {
            if (!socket.ValidarMatch())
            {
                Debug.Log($"Socket {socketIndex} es incorrecto - Esperaba: {socket.acceptedItemId}, " +
                         $"Tiene: {(socket.currentDraggable != null ? socket.currentDraggable.itemId : "vacío")}");
                todasCorrectas = false;
                break;
            }
            socketIndex++;
        }

        Debug.Log($"Resultado final: {(todasCorrectas ? "TODOS CORRECTOS" : "HAY INCORRECTOS")}");

        if (todasCorrectas)
        {
            CompletarMisionActual();
            if (textoRetroalimentacion != null)
            {
                textoRetroalimentacion.text = "¡Correcto! Puedes continuar con la siguiente misión.";
                textoRetroalimentacion.color = new Color(0, 1, 0, 1);
            }
        }
        else
        {
            if (textoRetroalimentacion != null)
            {
                textoRetroalimentacion.text = "Algunas respuestas no son correctas. Inténtalo de nuevo.";
                textoRetroalimentacion.color = new Color(1, 0, 0, 1);
            }
        }
    }

    public void CargarMision(int indice)
    {
        try
        {
            ValidarIndiceMision(indice);
            LimpiarMisionActual();
            
            Mision misionActual = misiones[indice];
            ActualizarUI(misionActual);
            InstanciarElementosMision(misionActual);
            
            onMisionCargada?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error al cargar misión: {e.Message}");
        }
    }

    private void ValidarIndiceMision(int indice)
    {
        if (indice < 0 || indice >= misiones.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(indice), "Índice de misión fuera de rango.");
        }
    }

    private void ActualizarUI(Mision mision)
    {
        if (textoNombreMision != null)
        {
            textoNombreMision.text = $"Misión {MisionActual}/{TotalMisiones}: {mision.nombreMision}";
        }

        if (textoDescripcionMision != null)
        {
            textoDescripcionMision.text = mision.descripcionMision;
        }
    }

    private void InstanciarElementosMision(Mision mision)
    {
        Debug.Log("=== Instanciando elementos de la misión ===");

        Debug.Log("Instanciando Sockets:");
        foreach (DropSocket socket in mision.sockets)
        {
            GameObject socketInstanciado = Instantiate(socket.gameObject, panelSockets);
            DropSocket socketComponent = socketInstanciado.GetComponent<DropSocket>();
            Debug.Log($"Socket - Prefab ID: {socket.acceptedItemId}, Instancia ID: {socketComponent.acceptedItemId}");
            objetosInstanciados.Add(socketInstanciado);
        }

        Debug.Log("Instanciando Draggables:");
        foreach (DraggableItem draggable in mision.draggables)
        {
            GameObject draggableInstanciado = Instantiate(draggable.gameObject, panelDraggables);
            DraggableItem draggableComponent = draggableInstanciado.GetComponent<DraggableItem>();
            Debug.Log($"Draggable - Prefab ID: {draggable.itemId}, Instancia ID: {draggableComponent.itemId}");
            objetosInstanciados.Add(draggableInstanciado);
        }
    }

    private void LimpiarMisionActual()
    {
        if (textoRetroalimentacion != null)
        {
            textoRetroalimentacion.text = "";
        }

        foreach (GameObject objeto in objetosInstanciados)
        {
            if (objeto != null)
            {
                DropSocket socket = objeto.GetComponent<DropSocket>();
                if (socket != null)
                {
                    socket.LimpiarSocket();
                }
                Destroy(objeto);
            }
        }
        objetosInstanciados.Clear();
    }

    private void LimpiarSocketsIncorrectos()
    {
        if (indiceMisionActual >= misiones.Count) return;

        Mision misionActual = misiones[indiceMisionActual];
        foreach (DropSocket socket in misionActual.sockets)
        {
            if (!socket.ValidarMatch())
            {
                socket.LimpiarSocket();
            }
        }
    }

    public void SiguienteMision()
    {
        if (indiceMisionActual < misiones.Count - 1)
        {
            indiceMisionActual++;
            CargarMision(indiceMisionActual);
            ActualizarEstadoBotonSiguiente();
        }
        else if (TodasMisionesCompletadas)
        {
            onTodasMisionesCompletadas?.Invoke();
        }
    }

    private void ActualizarEstadoBotonSiguiente()
    {
        if (botonSiguienteMision != null)
        {
            botonSiguienteMision.interactable = indiceMisionActual < misiones.Count - 1;
        }
    }

    public void CompletarMisionActual()
    {
        if (indiceMisionActual < misiones.Count)
        {
            Mision misionActual = misiones[indiceMisionActual];
            misionActual.estaCompletada = true;
            misionActual.onMisionCompletada?.Invoke();
        }
    }

    public void ReiniciarMisiones()
    {
        foreach (Mision mision in misiones)
        {
            mision.estaCompletada = false;
        }
        indiceMisionActual = 0;
        CargarMision(indiceMisionActual);
    }
}