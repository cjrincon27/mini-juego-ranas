using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Transform contentPanel; // 📌 Panel donde van los ítems en la UI
    public GameObject itemPrefab; // 📌 Prefab del ítem a instanciar

    private void Start()
    {
        Invoke(nameof(DisplayInventory), 0.2f); // ⏳ Espera antes de ejecutar
    }

    public void DisplayInventory()
    {
        List<ItemManager.ItemData> items = InventoryManager.Instance.GetStoredItems();

        if (items.Count == 0)
        {
            Debug.LogWarning("⚠ No hay ítems almacenados en el inventario.");
            return;
        }

        Debug.Log($"📋 Mostrando {items.Count} ítems en el inventario...");

        foreach (var itemData in items)
        {
            GameObject newItem = Instantiate(itemPrefab, contentPanel);
            ItemConfig itemConfig = newItem.GetComponent<ItemConfig>();
            Image itemImage = newItem.GetComponent<Image>();
            AudioSource audioSource = newItem.GetComponent<AudioSource>();
            DragAndDropUI dragAndDrop = newItem.GetComponent<DragAndDropUI>();
            Button itemButton = newItem.GetComponentInChildren<Button>(true); // Obtener el botón dentro del prefab

            if (itemConfig != null)
            {
                // ✅ Asignar valores desde el inventario
                itemConfig.itemID = itemData.itemID;
                itemConfig.itemImage = itemData.itemImage;
                itemConfig.itemAudio = itemData.itemAudio;
                itemConfig.tiempoCarnada = itemData.tiempoCarnada;
                itemConfig.tipoRanaAtrae = itemData.tipoRanaAtrae;
                itemConfig.porcentajeAumento = itemData.porcentajeAumento;
                itemConfig.mostrar = itemData.mostrar; // 🔹 Asigna el valor real de `mostrar`

                // 🔄 Actualizar los datos visuales y de audio
                itemConfig.UpdateItemVisuals();

                // 🔥 Modificaciones según `mostrar`
                if (itemData.mostrar)
                {
                    itemImage.color = Color.white; // Cambiar color a blanco
                    if (audioSource != null) audioSource.enabled = true; // Activar AudioSource
                    if (dragAndDrop != null) dragAndDrop.enabled = true; // Activar DragAndDropUI
                    if (itemButton != null) itemButton.gameObject.SetActive(true); // Activar botón
                }
                else
                {
                    itemImage.color = Color.black; // Mantener color en negro
                    if (audioSource != null) audioSource.enabled = false; // Mantener AudioSource desactivado
                    if (dragAndDrop != null) dragAndDrop.enabled = false; // Mantener DragAndDropUI desactivado
                    if (itemButton != null) itemButton.gameObject.SetActive(false); // Mantener botón desactivado
                }
                
                Debug.Log($"✅ Ítem asignado en UI: {itemData.itemID}, Mostrar: {itemConfig.mostrar}");
            }
            else
            {
                Debug.LogError($"❌ No se encontró ItemConfig en el prefab de {itemData.itemID}");
            }
        }
    }
}
