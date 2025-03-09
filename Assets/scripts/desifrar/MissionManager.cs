using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    [Header("Misiones configurables en el Inspector")]
    public List<MissionData> missions; // Lista de misiones a definir

    // Variable estática para evitar recrear misiones si ya se han creado
    private static bool missionsCreated = false;

    private void Start()
    {
        if (MissionInventoryManager.Instance == null)
        {
            Debug.LogError("❌ No se encontró MissionInventoryManager en la escena.");
            return;
        }

        // Solo guardamos las misiones si aún no han sido creadas
        if (!missionsCreated)
        {
            SaveMissionsToInventory();
            missionsCreated = true;
        }

        // Se destruye para no volver a crear las misiones
        Destroy(gameObject);
    }

    void SaveMissionsToInventory()
    {
        foreach (var mission in missions)
        {
            MissionInventoryManager.Instance.AddMission(mission);
        }
        Debug.Log("✅ Todas las misiones han sido guardadas en el inventario de misiones.");
    }
}
