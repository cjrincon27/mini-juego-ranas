using UnityEngine;
using UnityEngine.UI;

public class ActivationManager : MonoBehaviour
{
    public static ActivationManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Se mantiene entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método para activar un ítem por ID
    public void ActivateItem(string itemID)
    {
        ItemConfig item = FindItemByID(itemID); // 🔹 Busca el ítem en la escena actual
        if (item != null)
        {
            // Cambiar color a blanco
            Image imageComponent = item.GetComponent<Image>();
            if (imageComponent != null)
            {
                imageComponent.color = Color.white;
            }

            // Activar AudioSource
            AudioSource audioSource = item.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.enabled = true;
            }

            // Activar DragAndDropUI
            DragAndDropUI dragAndDrop = item.GetComponent<DragAndDropUI>();
            if (dragAndDrop != null)
            {
                dragAndDrop.enabled = true;
            }

            Debug.Log($"Ítem {itemID} activado correctamente.");
        }
        else
        {
            Debug.LogWarning($"El ítem con ID {itemID} no existe en la escena actual.");
        }
    }

    // 🔹 Método para encontrar un ítem por ID en la escena actual
    private ItemConfig FindItemByID(string itemID)
    {
        ItemConfig[] allItems = FindObjectsOfType<ItemConfig>(); // Busca todos los ítems en la escena
        foreach (ItemConfig item in allItems)
        {
            if (item.itemID == itemID)
            {
                return item;
            }
        }
        return null; // Si no encuentra el ítem
    }
}
