using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Instancia Singleton
    public static InventoryManager Instance;

    // Lista para almacenar ítems
    private List<ItemManager.ItemData> storedItems = new List<ItemManager.ItemData>();

    private void Awake()
    {
        // Asegurarse de que solo exista una instancia de InventoryManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 🔹 Persiste entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método para agregar un ítem al inventario
    public void AddItem(ItemManager.ItemData item)
    {
        // Verificar si el ítem ya existe en el inventario
        if (!storedItems.Exists(i => i.itemID == item.itemID))
        {
            storedItems.Add(item);
        }
        else
        {
            Debug.Log($"⚠ Ítem {item.itemID} ya existe en el inventario.");
        }
    }

    // Método para obtener la lista de ítems almacenados
    public List<ItemManager.ItemData> GetStoredItems()
    {
        if (storedItems.Count == 0)
        {
            Debug.LogWarning("⚠ No hay ítems almacenados en el inventario.");
        }
        else
        {
            Debug.Log($"📋 Inventario cargado con {storedItems.Count} ítems.");
        }
        return storedItems;
    }

    // Método para buscar un ítem por su id (nombre)
    public ItemManager.ItemData GetItemByID(string id)
    {
        ItemManager.ItemData foundItem = storedItems.Find(item => item.itemID == id);
        if (foundItem == null)
        {
            Debug.LogWarning($"⚠ No se encontró el ítem con id: {id}");
        }
        return foundItem;
    }
}
