using UnityEngine;
using UnityEngine.UI;

public class BlockManager : MonoBehaviour
{
    public static BlockManager instance;

    [Header("Configuración de Bloqueadores")]
    public GameObject[] bloqueadores; // Lista de bloqueadores en la escena

    [Header("Configuración de Sonidos")]
    public AudioSource audioSource;
    public AudioClip[] bloqueoSonidos; // Sonidos aleatorios

    [Header("Configuración de Mensajes")]
    public Text mensajeUI; // Texto de UI donde se mostrarán los mensajes
    public string[] mensajesBloqueo; // Mensajes aleatorios

    private float tiempoCooldown = 10f;  // Tiempo de espera entre sonidos (en segundos)
    private float siguienteTiempo = 0f;  // Marca el siguiente momento en el que se puede reproducir un sonido

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        //RevisarMisiones(); // Verifica qué áreas ya están desbloqueadas al iniciar la escena
    }

    public void BloqueoActivado(int bloqueadorID)
    {
        // Verificar si la zona ya está desbloqueada
        if (PlayerPrefs.GetInt("Mision_" + bloqueadorID, 0) == 1)
        {
            mensajeUI.text = "El camino está libre.";
            return;
        }

        // Verificar si ha pasado suficiente tiempo desde el último sonido
        if (Time.time >= siguienteTiempo)
        {
            // Reproducir un sonido aleatorio
            if (bloqueoSonidos.Length > 0)
            {
                int randomIndex = Random.Range(0, bloqueoSonidos.Length);
                audioSource.PlayOneShot(bloqueoSonidos[randomIndex]);
            }

            // Mostrar un mensaje aleatorio
            if (mensajesBloqueo.Length > 0)
            {
                int randomIndex = Random.Range(0, mensajesBloqueo.Length);
                mensajeUI.text = mensajesBloqueo[randomIndex];

                // Ocultar el mensaje después de 2 segundos
                CancelInvoke(nameof(OcultarMensaje));
                Invoke(nameof(OcultarMensaje), 2f);
            }

            // Establecer el tiempo para el siguiente sonido
            siguienteTiempo = Time.time + tiempoCooldown;
        }
    }

    public void DesbloquearBloqueador(int bloqueadorID)
    {
        // Guardar el estado del bloqueador como desbloqueado
        PlayerPrefs.SetInt("Mision_" + bloqueadorID, 1);
        PlayerPrefs.Save();

        // Buscar el bloqueador en la lista y desactivarlo
        if (bloqueadorID < bloqueadores.Length)
        {
            bloqueadores[bloqueadorID].SetActive(false);
        }
    }

    private void RevisarMisiones()
    {
        for (int i = 0; i < bloqueadores.Length; i++)
        {
            if (PlayerPrefs.GetInt("Mision_" + i, 0) == 1)
            {
                bloqueadores[i].SetActive(false); // Si la misión ya está completada, desactiva el bloqueador
            }
        }
    }

    private void OcultarMensaje()
    {
        mensajeUI.text = "";
    }
}
