using UnityEngine;
using UnityEngine.UI;

public class MissionDropValidator : MonoBehaviour
{
    // En lugar de almacenar el expectedItemID en Awake, lo obtenemos en cada validación.
    private string GetExpectedItemID()
    {
        // Buscar todos los componentes Text en la jerarquía, incluyendo inactivos.
        Text[] texts = GetComponentsInChildren<Text>(true);
        Debug.Log($"[MissionDropValidator] Se encontraron {texts.Length} componentes Text en la jerarquía de {gameObject.name}.");
        foreach (Text t in texts)
        {
            Debug.Log($"[MissionDropValidator] Text encontrado en: {t.gameObject.name}, contenido: '{t.text}'");
            // Usamos el primer Text con contenido no vacío.
            if (!string.IsNullOrEmpty(t.text))
            {
                Debug.Log($"[MissionDropValidator] Valor esperado obtenido: '{t.text}' desde '{t.gameObject.name}'.");
                return t.text;
            }
        }
        Debug.LogWarning($"[MissionDropValidator] No se encontró un Text con contenido en la jerarquía de {gameObject.name}.");
        return "";
    }

    /// <summary>
    /// Recorre toda la jerarquía de hijos para encontrar un ItemConfig cuyo itemID
    /// coincida con el valor actual obtenido del Text (expectedItemID).
    /// </summary>
    public bool IsValid()
    {
        string expectedItemID = GetExpectedItemID();
        Debug.Log($"[MissionDropValidator] Valor esperado (actualizado): '{expectedItemID}'.");

        // Buscar TODOS los componentes ItemConfig en la jerarquía (incluyendo inactivos)
        ItemConfig[] itemConfigs = GetComponentsInChildren<ItemConfig>(true);
        Debug.Log($"[MissionDropValidator] Se encontraron {itemConfigs.Length} componentes ItemConfig en la jerarquía de {gameObject.name}.");
        if (itemConfigs.Length == 0)
        {
            Debug.LogWarning($"[Drop: {gameObject.name}] No se encontró ningún objeto con componente ItemConfig.");
            return false;
        }

        foreach (var itemConfig in itemConfigs)
        {
            Debug.Log($"[MissionDropValidator] Comparando ItemConfig en {itemConfig.gameObject.name} con itemID '{itemConfig.itemID}' contra expectedItemID '{expectedItemID}'.");
            if (itemConfig.itemID.Equals(expectedItemID))
            {
                Debug.Log($"[MissionDropValidator] Coincidencia encontrada en {itemConfig.gameObject.name}.");
                return true;
            }
        }

        Debug.LogWarning($"[Drop: {gameObject.name}] Ningún ItemConfig coincide con '{expectedItemID}'.");
        return false;
    }
}
