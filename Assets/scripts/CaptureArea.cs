using UnityEngine;

/// <summary>
/// Controla un �rea de captura que sigue al cursor y permite atrapar ranas espec�ficas.
/// El �rea se encoge con el tiempo y requiere que la rana permanezca dentro por un per�odo determinado.
/// </summary>
public class CaptureArea : MonoBehaviour
{
    #region Variables Privadas
    private Transform targetFrog;                    // Referencia a la rana objetivo
    private int frogType;                           // Tipo de rana a capturar
    private float captureTimer;                     // Temporizador actual de captura
    private bool isCapturing;                       // Estado actual de la captura
    private Vector3 initialScale;                   // Escala inicial del �rea de captura
    private Camera mainCamera;                      // Referencia cacheada a la c�mara principal
    private bool hasFrogExited;                     // Indica si la rana ha salido del �rea
    private float captureThreshold;                 // Tiempo m�nimo que la rana debe permanecer en el �rea
    #endregion

    #region Variables Configurables
    [Header("Configuraci�n de Captura")]
    [Tooltip("Tiempo total que tarda el �rea en encogerse")]
    // Cambiado a public para acceso desde FrogSpawner
    public float shrinkTime = 5f;

    [Tooltip("Retardo antes de iniciar la captura")]
    [SerializeField] private float startDelay = 1f;

    [Tooltip("Factor m�nimo de escala (porcentaje del tama�o original)")]
    [Range(0.1f, 1f)]
    [SerializeField] private float minScaleFactor = 0.7f;

    [Tooltip("Tecla para cancelar la captura")]
    [SerializeField] private KeyCode cancelKey = KeyCode.Escape;

    [Tooltip("Porcentaje del tiempo total necesario para una captura exitosa (0-1)")]
    [Range(0f, 1f)]
    [SerializeField] private float captureThresholdPercentage = 0.3f;
    #endregion

    #region M�todos de Inicializaci�n
    /// <summary>
    /// Inicializa el �rea de captura con una rana objetivo espec�fica.
    /// </summary>
    /// <param name="frog">Transform de la rana objetivo</param>
    /// <param name="frogType">Tipo de rana</param>
    public void Initialize(Transform frog, int frogType)
    {
        // Inicializaci�n de variables
        targetFrog = frog;
        this.frogType = frogType;
        initialScale = transform.localScale;
        captureTimer = 0f;
        isCapturing = false;
        hasFrogExited = false;

        // C�lculo del umbral de captura basado en el porcentaje configurado
        captureThreshold = shrinkTime * captureThresholdPercentage;

        // Cache de la referencia a la c�mara
        mainCamera = Camera.main;

        // Inicia la captura despu�s del retardo configurado
        Invoke(nameof(StartCapturing), startDelay);
    }

    /// <summary>
    /// Inicia el proceso de captura y verifica la posici�n inicial de la rana.
    /// </summary>
    private void StartCapturing()
    {
        isCapturing = true;
        Debug.Log("[CaptureArea] �rea de captura activada.");

        CheckInitialFrogPosition();
    }

    /// <summary>
    /// Verifica si la rana est� dentro del �rea al momento de iniciar la captura.
    /// </summary>
    private void CheckInitialFrogPosition()
    {
        float checkRadius = transform.localScale.x / 2f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, checkRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.transform == targetFrog)
            {
                hasFrogExited = false;
                Debug.Log("[CaptureArea] Rana detectada en posici�n inicial.");
                break;
            }
        }
    }
    #endregion

    #region L�gica Principal
    private void Update()
    {
        // Verifica cancelaci�n
        if (Input.GetKeyDown(cancelKey))
        {
            CancelCapture();
            return;
        }

        // Actualiza posici�n seg�n el mouse
        UpdatePosition();

        // Procesa la captura si est� activa
        if (isCapturing)
        {
            ProcessCapture();
        }
    }

    /// <summary>
    /// Actualiza la posici�n del �rea seg�n la posici�n del mouse.
    /// </summary>
    private void UpdatePosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        }
    }

    /// <summary>
    /// Procesa la l�gica de captura, incluyendo el encogimiento y verificaci�n de captura.
    /// </summary>
    private void ProcessCapture()
    {
        // Actualiza el temporizador
        captureTimer += Time.deltaTime;

        // Calcula y aplica el encogimiento
        float shrinkProgress = captureTimer / shrinkTime;
        float currentScale = Mathf.Lerp(1f, minScaleFactor, shrinkProgress);
        transform.localScale = initialScale * currentScale;

        // Verifica el progreso de la captura
        if (captureTimer >= captureThreshold && !hasFrogExited)
        {
             //Debug.Log("[CaptureArea] Tiempo m�nimo de captura alcanzado.");
        }

        // Verifica si se complet� el tiempo total
        if (captureTimer >= shrinkTime)
        {
            CompleteCaptureAttempt();
        }
    }

    /// <summary>
    /// Completa el intento de captura, sea exitoso o fallido.
    /// </summary>
    private void CompleteCaptureAttempt()
    {
        if (captureTimer >= captureThreshold && !hasFrogExited)
        {
            // Captura exitosa
            FrogSpawner.Instance.CaptureFrog(frogType);
            if (targetFrog != null)
            {
                Destroy(targetFrog.gameObject);
            }
            Debug.Log("[CaptureArea] Captura exitosa.");
        }
        else
        {
            Debug.Log("[CaptureArea] Captura fallida.");
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Cancela el proceso de captura actual.
    /// </summary>
    private void CancelCapture()
    {
        if (isCapturing)
        {
            Debug.Log("[CaptureArea] Captura cancelada por el jugador.");
            Destroy(gameObject);
        }
    }
    #endregion

    #region Eventos de Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (isCapturing && other.transform == targetFrog)
        {
            hasFrogExited = false;
            Debug.Log("[CaptureArea] Rana entr� en el �rea.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isCapturing && other.transform == targetFrog)
        {
            hasFrogExited = true;
            Debug.Log("[CaptureArea] Rana sali� del �rea.");
        }
    }
    #endregion
}