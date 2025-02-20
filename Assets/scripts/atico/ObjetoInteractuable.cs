using UnityEngine;

public class ObjetoInteractuable : MonoBehaviour
{
    public string nombreObjeto;
    public string colorObjeto;
    public AudioClip audioNombre;
    public AudioClip audioColor;

    private void OnMouseEnter()
    {
        // Cambiar el cursor al pasar sobre el objeto
        Cursor.SetCursor(CursorManager.instance.cursorInteractivo, Vector2.zero, CursorMode.Auto);
    }

    private void OnMouseExit()
    {
        // Restaurar el cursor al salir del objeto
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void OnMouseDown()
    {
        // Mostrar el panel con la información del objeto
        UIManager.instance.MostrarPanel(nombreObjeto, colorObjeto, audioNombre, audioColor);
    }
}
