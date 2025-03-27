using UnityEngine;

[RequireComponent(typeof(Outline), typeof(Collider))]
public class ObjetoInteractuable : MonoBehaviour
{
    public string nombreObjeto;
    public string colorObjeto;
    public AudioClip audioNombre;
    public AudioClip audioColor;
    public Color colorResplandor = Color.yellow;
    public float outlineWidth = 5f;

    private Outline outline;
    private Collider objetoCollider;

    private void Start()
    {
        // Configurar Outline
        outline = GetComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = colorResplandor;
        outline.OutlineWidth = outlineWidth;
        outline.enabled = false;

        // Asegurar que el collider esté habilitado
        objetoCollider = GetComponent<Collider>();
        objetoCollider.enabled = true;
    }

    private void OnMouseEnter()
    {
        // Activar el resplandor al pasar el ratón
        outline.enabled = true;
        Cursor.SetCursor(CursorManager.instance.cursorInteractivo, Vector2.zero, CursorMode.Auto);
    }

    private void OnMouseExit()
    {
        // Desactivar el resplandor al salir
        outline.enabled = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void OnMouseDown()
    {
        // Mostrar el panel con la información del objeto
        UIManager.instance.MostrarPanel(nombreObjeto, colorObjeto, audioNombre, audioColor);
    }
}