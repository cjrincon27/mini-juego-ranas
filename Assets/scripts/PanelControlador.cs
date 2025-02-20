using UnityEngine;
using UnityEngine.UI;

public class PanelControlador : MonoBehaviour
{
    public GameObject depuracionPanel; // Panel de depuración que queremos abrir y cerrar
    private bool panelVisible = false; // Estado actual del panel

    public void ToggleDepuracionPanel()
    {
        panelVisible = !panelVisible; // Cambia el estado
        depuracionPanel.SetActive(panelVisible); // Muestra u oculta el panel
    }
}
