using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionSelectUI : MonoBehaviour
{
    [Header("Dropdown de Misiones")]
    public TMP_Dropdown missionDropdown;

    [Header("Panel de Detalle de Misión")]
    public TMP_Text missionNameText;
    public TMP_Text missionDescriptionText;
    public Transform missionStepsContainer;
    public GameObject textStepPrefab;
    public GameObject dropTextStepPrefab;
    public Image missionPanelBackground;

    private List<MissionData> missions;

    private Color completedColor = new Color(0f, 0.792f, 0.063f, 1f);
    private Color notCompletedColor = new Color(1f, 0f, 0f, 1f);

    private void Start()
    {
        if (MissionInventoryManager.Instance == null)
        {
            Debug.LogError("MissionInventoryManager.Instance es NULL. Asegúrate de que está en la escena.");
            return;
        }

        missions = MissionInventoryManager.Instance.GetAllMissions();

        if (missions == null)
        {
            Debug.LogError("GetAllMissions() devolvió NULL.");
            return;
        }

        if (missionDropdown == null)
        {
            Debug.LogError("missionDropdown no está asignado en el inspector.");
            return;
        }

        PopulateDropdown();
        missionDropdown.onValueChanged.AddListener(OnMissionSelected);
    }

    private void PopulateDropdown()
    {
        if (missions == null || missionDropdown == null)
        {
            Debug.LogError("PopulateDropdown() no puede ejecutarse porque 'missions' o 'missionDropdown' es NULL.");
            return;
        }

        missionDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach (var mission in missions)
        {
            options.Add(new TMP_Dropdown.OptionData(mission.missionName));
        }

        missionDropdown.AddOptions(options);

        if (missions.Count > 0)
        {
            OnMissionSelected(0);
        }

        UpdateDropdownColors();
    }

    private void OnMissionSelected(int index)
    {
        if (missions == null || index < 0 || index >= missions.Count)
        {
            Debug.LogWarning("Índice de misión no válido o lista de misiones nula.");
            return;
        }

        MissionData selectedMission = missions[index];

        if (missionNameText != null)
            missionNameText.text = selectedMission.missionName;
        else
            Debug.LogWarning("missionNameText no está asignado en el inspector.");

        if (missionDescriptionText != null)
            missionDescriptionText.text = selectedMission.description;
        else
            Debug.LogWarning("missionDescriptionText no está asignado en el inspector.");

        if (missionPanelBackground != null)
            missionPanelBackground.color = selectedMission.completada ? completedColor : notCompletedColor;
        else
            Debug.LogWarning("missionPanelBackground no está asignado en el inspector.");

        if (missionStepsContainer == null)
        {
            Debug.LogWarning("missionStepsContainer no está asignado en el inspector.");
            return;
        }

        foreach (Transform child in missionStepsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var step in selectedMission.steps)
        {
            GameObject stepPrefab = step.isDrop ? dropTextStepPrefab : textStepPrefab;
            if (stepPrefab == null)
            {
                Debug.LogWarning("Uno de los prefabs de pasos de misión no está asignado en el inspector.");
                continue;
            }

            GameObject stepInstance = Instantiate(stepPrefab, missionStepsContainer);
            Text stepText = stepInstance.GetComponentInChildren<Text>();
            if (stepText != null)
            {
                stepText.text = step.stepText;
            }
        }

        UpdateDropdownColors();
    }

    private void UpdateDropdownColors()
    {
        if (missions == null || missionDropdown == null)
        {
            Debug.LogError("UpdateDropdownColors() no puede ejecutarse porque 'missions' o 'missionDropdown' es NULL.");
            return;
        }

        for (int i = 0; i < missionDropdown.options.Count; i++)
        {
            string colorTag = missions[i].completada ? "<color=#00CA10>" : "<color=#FF0000>";
            missionDropdown.options[i].text = colorTag + missions[i].missionName + "</color>";
        }

        missionDropdown.RefreshShownValue();
    }

    public void RefreshUI()
    {
        if (missionDropdown == null)
        {
            Debug.LogError("RefreshUI() no puede ejecutarse porque 'missionDropdown' es NULL.");
            return;
        }

        int currentIndex = missionDropdown.value;
        OnMissionSelected(currentIndex);
        UpdateDropdownColors();
    }
}
