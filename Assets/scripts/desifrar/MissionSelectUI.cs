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
    public GameObject dropTextStepPrefab;     // Prefab para un paso de misión "drop+texto" (Legacy Text)
    public Image missionPanelBackground;      // Fondo del panel de misión

    private List<MissionData> missions;

    private Color completedColor = new Color(0f, 0.792f, 0.063f, 1f);  // #00CA10
    private Color notCompletedColor = new Color(1f, 0f, 0f, 1f);       // #FF0000

    private void Start()
    {
        // Obtener todas las misiones guardadas en el inventario de misiones
        missions = MissionInventoryManager.Instance.GetAllMissions();
        PopulateDropdown();

        // Asignar el listener para detectar cambios en el Dropdown
        missionDropdown.onValueChanged.AddListener(OnMissionSelected);
    }

    // Llena el Dropdown con los nombres de las misiones y cambia el color de las completadas
    private void PopulateDropdown()
    {
        missionDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        for (int i = 0; i < missions.Count; i++)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(missions[i].missionName);
            options.Add(option);
        }

        missionDropdown.AddOptions(options);

        // Si hay misiones, mostrar la primera por defecto
        if (missions.Count > 0)
        {
            OnMissionSelected(0);
        }

        UpdateDropdownColors();
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

        // Cambiar color del panel según el estado de la misión
        missionPanelBackground.color = selectedMission.completada ? completedColor : notCompletedColor;

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

        // Actualizar colores del Dropdown
        UpdateDropdownColors();
    }

    // Cambia el color del texto en el Dropdown si la misión está completada
    private void UpdateDropdownColors()
    {
        for (int i = 0; i < missionDropdown.options.Count; i++)
        {
            string colorTag = missions[i].completada ? "<color=#00CA10>" : "<color=#FF0000>";
            missionDropdown.options[i].text = colorTag + missions[i].missionName + "</color>";
        }

        // Refrescar visualmente el Dropdown
        missionDropdown.RefreshShownValue();
    }

    // Método público para refrescar la UI (se puede llamar desde un botón)
    public void RefreshUI()
    {
        int currentIndex = missionDropdown.value;
        OnMissionSelected(currentIndex);
        UpdateDropdownColors();
    }
}
