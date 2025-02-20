using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject panelInfo;
    public TMP_Text textoNombre;
    public TMP_Text textoColor;
    public Button botonGuardar;
    public Button botonAudioNombre;
    public Button botonAudioColor;

    private AudioSource audioSource;
    private AudioClip clipNombre;
    private AudioClip clipColor;

    private RectTransform panelRectTransform;
    private RectTransform canvasRectTransform;

    private void Awake()
    {
        if (instance == null) instance = this;
        audioSource = GetComponent<AudioSource>();
        panelRectTransform = panelInfo.GetComponent<RectTransform>();
        canvasRectTransform = GetComponent<RectTransform>();
    }

    public void MostrarPanel(string nombre, string color, AudioClip audioNombre, AudioClip audioColor)
    {
        // Posicionar el panel en la posici�n del mouse
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            Input.mousePosition,
            null,
            out mousePosition
        );

        // Obtener tama�o del panel y del canvas
        Vector2 panelSize = panelRectTransform.rect.size;
        Vector2 canvasSize = canvasRectTransform.rect.size;

        // Calcular los l�mites de movimiento
        float halfPanelWidth = panelSize.x / 2;
        float halfPanelHeight = panelSize.y / 2;

        // Calcular posiciones l�mite
        Vector2 minPosition = new Vector2(
            -canvasSize.x / 2 + halfPanelWidth,
            -canvasSize.y / 2 + halfPanelHeight
        );
        Vector2 maxPosition = new Vector2(
            canvasSize.x / 2 - halfPanelWidth,
            canvasSize.y / 2 - halfPanelHeight
        );

        // Ajustar posici�n manteniendo el punto de clic lo m�s cerca posible
        Vector2 adjustedPosition = new Vector2(
            Mathf.Clamp(mousePosition.x, minPosition.x, maxPosition.x),
            Mathf.Clamp(mousePosition.y, minPosition.y, maxPosition.y)
        );

        panelRectTransform.anchoredPosition = adjustedPosition;

        panelInfo.SetActive(true);
        textoNombre.text = nombre;
        textoColor.text = color;

        // Asigna los clips temporalmente para usarlos en m�todos sin par�metros
        clipNombre = audioNombre;
        clipColor = audioColor;

        botonAudioNombre.onClick.RemoveAllListeners();
        botonAudioNombre.onClick.AddListener(ReproducirNombre);

        botonAudioColor.onClick.RemoveAllListeners();
        botonAudioColor.onClick.AddListener(ReproducirColor);

        botonGuardar.onClick.RemoveAllListeners();
        botonGuardar.onClick.AddListener(() => GuardarInformacion(nombre, color));
    }

    // Los dem�s m�todos permanecen igual
    private void ReproducirAudio(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    private void ReproducirNombre()
    {
        ReproducirAudio(clipNombre);
    }

    private void ReproducirColor()
    {
        ReproducirAudio(clipColor);
    }

    private void GuardarInformacion(string nombre, string color)
    {
        DatosGlobales.GuardarDatos(nombre, color);
        ReproducirAudio(clipNombre);
    }

    public void OcultarPanel()
    {
        panelInfo.SetActive(false);
    }
}