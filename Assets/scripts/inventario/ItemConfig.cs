using UnityEngine;
using UnityEngine.UI;

public class ItemConfig : MonoBehaviour
{
    [Header("Datos del Ítem")]
    public string itemID;
    public Sprite itemImage;
    public AudioClip itemAudio;
    public float tiempoCarnada;
    public int tipoRanaAtrae;
    public float porcentajeAumento;
    public bool mostrar; // 🔹 Nuevo booleano agregado

    private Image imageComponent;
    private AudioSource audioSource;

    void Awake()
    {
        imageComponent = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
    }

    public void UpdateItemVisuals()
    {
        if (imageComponent != null && itemImage != null)
        {
            imageComponent.sprite = itemImage;
        }

        if (audioSource != null && itemAudio != null)
        {
            audioSource.clip = itemAudio;
        }

    }
}
