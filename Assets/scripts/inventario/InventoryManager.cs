using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private List<ItemManager.ItemData> storedItems = new List<ItemManager.ItemData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 🔹 Persiste entre escenas
            Debug.Log("✅ InventoryManager creado y persistente entre escenas.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(ItemManager.ItemData item)
    {
        if (!storedItems.Exists(i => i.itemID == item.itemID))
        {
            storedItems.Add(item);
            Debug.Log($"📦 Ítem agregado al inventario: {item.itemID}");
        }
        else
        {
            Debug.Log($"⚠ Ítem {item.itemID} ya existe en el inventario.");
        }
    }

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
}
