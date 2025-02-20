using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager instance;
    public Texture2D cursorInteractivo;

    private void Awake()
    {
        if (instance == null) instance = this;
    }
}
