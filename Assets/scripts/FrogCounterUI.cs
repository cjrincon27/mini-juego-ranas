using UnityEngine;
using TMPro; // Asegúrate de tener TextMeshPro importado en tu proyecto
using System.Collections.Generic;

/// <summary>
/// Maneja la visualización del contador de ranas capturadas en la UI
/// </summary>
public class FrogCounterUI : MonoBehaviour
{
    [System.Serializable]
    public class FrogTypeCounter
    {
        public string frogTypeName;    // Nombre del tipo de rana
        public TextMeshProUGUI counterText;  // Referencia al texto UI
        public int count;              // Contador para este tipo
    }

    [Header("Configuración de Contadores")]
    [Tooltip("Lista de contadores para cada tipo de rana")]
    [SerializeField] private List<FrogTypeCounter> frogCounters = new List<FrogTypeCounter>();

    // Singleton para fácil acceso
    public static FrogCounterUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Inicializar contadores
        InitializeCounters();
    }

    /// <summary>
    /// Inicializa todos los contadores a cero
    /// </summary>
    private void InitializeCounters()
    {
        foreach (var counter in frogCounters)
        {
            counter.count = 0;
            UpdateCounterDisplay(counter);
        }
    }

    /// <summary>
    /// Incrementa el contador para un tipo específico de rana
    /// </summary>
    /// <param name="frogType">El tipo de rana (índice)</param>
    public void IncrementCounter(int frogType)
    {
        if (frogType >= 1 && frogType <= frogCounters.Count)
        {
            frogCounters[frogType - 1].count++;
            UpdateCounterDisplay(frogCounters[frogType - 1]);
        }
        else
        {
            Debug.LogError($"Índice de tipo de rana inválido: {frogType}");
        }
    }

    /// <summary>
    /// Actualiza el texto mostrado para un contador específico
    /// </summary>
    private void UpdateCounterDisplay(FrogTypeCounter counter)
    {
        if (counter.counterText != null)
        {
            counter.counterText.text = $"{counter.frogTypeName}: {counter.count}";
        }
    }

    /// <summary>
    /// Reinicia todos los contadores a cero
    /// </summary>
    public void ResetAllCounters()
    {
        foreach (var counter in frogCounters)
        {
            counter.count = 0;
            UpdateCounterDisplay(counter);
        }
        Debug.Log("[FrogCounter] Todos los contadores reiniciados");
    }
}