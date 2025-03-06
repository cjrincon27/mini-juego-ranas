using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [Header("Configuración")]
    public List<ItemData> itemsData = new List<ItemData>(); // Lista de datos de ítems

    [System.Serializable]
    public class ItemData
    {
        public string itemID;
        public Sprite itemImage;
        public AudioClip itemAudio;
        public float tiempoCarnada;
        public int tipoRanaAtrae;
        public float porcentajeAumento;
        public bool mostrar; // 🔹 Nuevo booleano para controlar la visibilidad
    }

    // Bool estático para controlar si los ítems ya han sido guardados
    private static bool itemsCreados = false;

    private void Start()
    {
        Debug.Log("🔹 ItemManager Start: Intentando guardar ítems en el inventario...");
        
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("❌ InventoryManager.Instance es NULL. Asegúrate de que InventoryManager esté en la escena.");
            return;
        }

        // Verifica si los ítems ya fueron creados
        if (!itemsCreados)
        {
            SaveItemsToInventory();
            itemsCreados = true; // Marca los ítems como creados
            Debug.Log("✅ Todos los ítems han sido guardados en el inventario.");
        }
        else
        {
            Debug.Log("🔹 Los ítems ya han sido guardados previamente.");
        }

        Destroy(gameObject); // 🔹 Se destruye después de guardar los ítems
    }

    void SaveItemsToInventory()
    {
        foreach (var data in itemsData)
        {
            InventoryManager.Instance.AddItem(data);
        }
    }
}
