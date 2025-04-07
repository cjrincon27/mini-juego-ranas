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
    public Image imagenObjetoUI;
    public Vector2 posicionFinal = new Vector2(-200, 200);

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

    public void MostrarPanel(string nombre, string color, AudioClip audioNombre, AudioClip audioColor, Sprite imagen = null)
    {
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            Input.mousePosition,
            null,
            out mousePosition
        );

        Vector2 panelSize = panelRectTransform.rect.size;
        Vector2 canvasSize = canvasRectTransform.rect.size;

        float halfPanelWidth = panelSize.x / 2;
        float halfPanelHeight = panelSize.y / 2;

        Vector2 minPosition = new Vector2(
            -canvasSize.x / 2 + halfPanelWidth,
            -canvasSize.y / 2 + halfPanelHeight
        );
        Vector2 maxPosition = new Vector2(
            canvasSize.x / 2 - halfPanelWidth,
            canvasSize.y / 2 - halfPanelHeight
        );

        Vector2 adjustedPosition = new Vector2(
            Mathf.Clamp(mousePosition.x, minPosition.x, maxPosition.x),
            Mathf.Clamp(mousePosition.y, minPosition.y, maxPosition.y)
        );

        panelRectTransform.anchoredPosition = adjustedPosition;

        panelInfo.SetActive(true);
        textoNombre.text = nombre;
        textoColor.text = color;

        if(imagen != null)
        {
            imagenObjetoUI.sprite = imagen;
            imagenObjetoUI.gameObject.SetActive(true);
        }
        else
        {
            imagenObjetoUI.gameObject.SetActive(false);
        }

        clipNombre = audioNombre;
        clipColor = audioColor;

        botonAudioNombre.onClick.RemoveAllListeners();
        botonAudioNombre.onClick.AddListener(ReproducirNombre);

        botonAudioColor.onClick.RemoveAllListeners();
        botonAudioColor.onClick.AddListener(ReproducirColor);

        botonGuardar.onClick.RemoveAllListeners();
        botonGuardar.onClick.AddListener(() => GuardarInformacion(nombre, color));
    }

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
        // Buscar el ítem en el inventario por su id (nombre)
        ItemManager.ItemData item = InventoryManager.Instance.GetItemByID(nombre);
        if (item != null)
        {
            item.mostrar = true;
            Debug.Log($"Se activó el ítem: {nombre} (mostrar = true)");
        }
        else
        {
            Debug.LogWarning($"No se encontró el ítem con id: {nombre}");
        }
        if (imagenObjetoUI.sprite != null)
        {
            LeanTween.scale(imagenObjetoUI.rectTransform, Vector3.one * 0.5f, 0.5f) // Escala a la mitad
                .setEase(LeanTweenType.easeInOutQuad);

            LeanTween.move(imagenObjetoUI.rectTransform, posicionFinal, 1f) // Mueve a las coordenadas
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(() => {
                    imagenObjetoUI.gameObject.SetActive(false); // Oculta al finalizar
                    imagenObjetoUI.rectTransform.localScale = Vector3.one; // Restablece escala
                });
        }
        // Reproducir el audio correspondiente
        ReproducirAudio(clipNombre);
    }

    public void OcultarPanel()
    {
        panelInfo.SetActive(false);
    }
}