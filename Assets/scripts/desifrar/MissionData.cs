using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionStep
{
    // Indica si este paso es "drop + texto" (true) o "solo texto" (false)
    public bool isDrop;
    // El texto que se mostrará en este paso
    public string stepText;
}

[System.Serializable]
public class MissionData
{
    // Nombre de la misión (ej. "Misión Uno")
    public string missionName;
    // Descripción de la misión
    public string description;
    // Estado de la misión (true = completada, false = no completada)
    public bool completada;
    // Lista de pasos que componen la misión
    public List<MissionStep> steps = new List<MissionStep>();
}
