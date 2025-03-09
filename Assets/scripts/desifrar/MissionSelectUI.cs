using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionSelectUI : MonoBehaviour
{
    [Header("Dropdown de Misiones")]
    public TMP_Dropdown missionDropdown;  // Dropdown de misiones

    [Header("Panel de Detalle de Misión")]
    public TMP_Text missionNameText;          // Nombre de la misión (TMP_Text)
    public TMP_Text missionDescriptionText;   // Descripción de la misión (TMP_Text)
    public Transform missionStepsContainer;   // Contenedor donde se instancian los pasos
    public GameObject textStepPrefab;         // Prefab para un paso de misión "solo texto" (Legacy Text)
    public GameObject dropTextStepPrefab;       // Prefab para un paso de misión "drop+texto" (Legacy Text)

    private List<MissionData> missions;

    private void Start()
    {
        // Obtener todas las misiones guardadas en el inventario de misiones
        missions = MissionInventoryManager.Instance.GetAllMissions();
        PopulateDropdown();

        // Asignar el listener para detectar cambios en el Dropdown
        missionDropdown.onValueChanged.AddListener(OnMissionSelected);
    }

    // Llena el Dropdown con los nombres de las misiones
    private void PopulateDropdown()
    {
        missionDropdown.ClearOptions();
        List<string> options = new List<string>();

        foreach (var mission in missions)
        {
            options.Add(mission.missionName);
        }
        missionDropdown.AddOptions(options);

        // Si hay misiones, mostrar la primera por defecto
        if (missions.Count > 0)
        {
            OnMissionSelected(0);
        }
    }

    // Se llama cada vez que se selecciona una misión en el Dropdown
    private void OnMissionSelected(int index)
    {
        if (index < 0 || index >= missions.Count)
        {
            Debug.LogWarning("Índice de misión no válido.");
            return;
        }

        MissionData selectedMission = missions[index];

        // Actualizar los textos del panel de detalle (TMP_Text)
        missionNameText.text = selectedMission.missionName;
        missionDescriptionText.text = selectedMission.description;

        // Limpiar el contenedor de pasos
        foreach (Transform child in missionStepsContainer)
        {
            Destroy(child.gameObject);
        }

        // Instanciar los prefabs correspondientes para cada paso de la misión
        foreach (var step in selectedMission.steps)
        {
            GameObject stepPrefab = step.isDrop ? dropTextStepPrefab : textStepPrefab;
            GameObject stepInstance = Instantiate(stepPrefab, missionStepsContainer);
            
            // Obtener el componente Legacy Text para asignar el texto
            Text stepText = stepInstance.GetComponentInChildren<Text>();
            if (stepText != null)
            {
                stepText.text = step.stepText;
            }
        }
    }
}
