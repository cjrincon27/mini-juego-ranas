using UnityEngine;
using TMPro;

public class MissionPanelValidator : MonoBehaviour
{
    // TMP_Text para mostrar el mensaje de validación en la UI
    public TMP_Text validationMessageText;
    // TMP_Text que contiene el nombre de la misión (usado para buscar la misión en el inventario)
    public TMP_Text missionNameText;

    /// <summary>
    /// Valida todos los drops de esta misión y actualiza el mensaje en el TMP_Text.
    /// Además, si todo es correcto, busca la misión (usando el nombre mostrado) y la marca como completada.
    /// Llama a este método desde el botón de validación.
    /// </summary>
    public void ValidateMission()
    {
        // Obtener todos los validadores de drops que estén en los hijos de este panel
        MissionDropValidator[] dropValidators = GetComponentsInChildren<MissionDropValidator>();

        bool allValid = true;
        foreach (var validator in dropValidators)
        {
            if (!validator.IsValid())
            {
                allValid = false;
            }
        }

        if (allValid)
        {
            validationMessageText.text = "¡Todo es correcto! La misión se completó correctamente.";
            // Obtener el nombre de la misión desde el TMP_Text y actualizar el estado en el inventario
            string missionName = missionNameText.text;
            if (MissionInventoryManager.Instance != null)
            {
                MissionInventoryManager.Instance.MarkMissionAsCompleted(missionName);
                Debug.Log($"La misión '{missionName}' ha sido marcada como completada.");
            }
            else
            {
                Debug.LogWarning("No se encontró MissionInventoryManager.Instance.");
            }
        }
        else
        {
            validationMessageText.text = "Algunos items no son correctos. Vuelve a intentarlo.";
        }
    }
}
