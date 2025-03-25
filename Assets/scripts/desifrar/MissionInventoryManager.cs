using System.Collections.Generic;
using UnityEngine;

public class MissionInventoryManager : MonoBehaviour
{
    public static MissionInventoryManager Instance;

    private List<MissionData> storedMissions = new List<MissionData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persistir entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Agrega una misión si no existe ya una con el mismo nombre
    public void AddMission(MissionData mission)
    {
        if (!storedMissions.Exists(m => m.missionName == mission.missionName))
        {
            storedMissions.Add(mission);
            Debug.Log($"📦 Misión '{mission.missionName}' agregada al inventario.");
        }
        else
        {
            Debug.Log($"⚠ La misión '{mission.missionName}' ya existe en el inventario.");
        }
    }

    // Retorna la lista de misiones almacenadas
    public List<MissionData> GetAllMissions()
    {
        if (storedMissions.Count == 0)
        {
            Debug.LogWarning("⚠ No hay misiones en el inventario.");
        }
        else
        {
            Debug.Log($"📋 Inventario cargado con {storedMissions.Count} misiones.");
        }
        return storedMissions;
    }

    // Marca como completada la misión que coincide con el nombre
    public void MarkMissionAsCompleted(string missionName)
    {
        foreach (MissionData mission in storedMissions)
        {
            if (mission.missionName.Equals(missionName))
            {
                mission.completada = true;
                Debug.Log($"✅ Misión '{missionName}' marcada como completada.");
                return;
            }
        }
        Debug.LogWarning($"⚠ No se encontró la misión '{missionName}' en el inventario.");
    }

    // Método opcional para imprimir todas las misiones en la consola
    public void PrintAllMissions()
    {
        if (storedMissions.Count == 0)
        {
            Debug.Log("⚠ No hay misiones en el inventario.");
            return;
        }

        Debug.Log($"📋 Hay {storedMissions.Count} misiones en el inventario:");
        foreach (var mission in storedMissions)
        {
            Debug.Log($"- {mission.missionName} | Completada: {mission.completada}");
            for (int i = 0; i < mission.steps.Count; i++)
            {
                var step = mission.steps[i];
                string tipo = step.isDrop ? "[Drop+Texto]" : "[Texto]";
                Debug.Log($"   Paso {i + 1}: {tipo} => {step.stepText}");
            }
        }
    }
}
