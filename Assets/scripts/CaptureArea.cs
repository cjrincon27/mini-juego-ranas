using UnityEngine;

/// <summary>
/// Controla un área de captura que sigue al cursor y permite atrapar ranas específicas.
/// El área se encoge con el tiempo y requiere que la rana permanezca dentro por un período determinado.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class CaptureArea : MonoBehaviour
{
    #region Variables Privadas
    private Transform targetFrog;
    private int frogType;
    private float captureTimer;
    private bool isCapturing;
    private Vector3 initialScale;
    private Camera mainCamera;
    private bool hasFrogExited;
    private float captureThreshold;
    private bool captureComplete = false;
    private AudioSource audioSource;
    #endregion

    #region Variables Configurables
    [Header("Configuración de Captura")]
    public float shrinkTime = 5f;

    [SerializeField] private float startDelay = 1f;
    [Range(0.1f, 1f)] [SerializeField] private float minScaleFactor = 0.7f;
    [SerializeField] private KeyCode cancelKey = KeyCode.Escape;
    [Range(0f, 1f)] [SerializeField] private float captureThresholdPercentage = 0.3f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip successClip;
    [SerializeField] private AudioClip failClip;
    #endregion

    #region Métodos de Inicialización
    public void Initialize(Transform frog, int frogType)
    {
        targetFrog = frog;
        this.frogType = frogType;
        initialScale = transform.localScale;
        captureTimer = 0f;
        isCapturing = false;
        hasFrogExited = false;
        captureThreshold = shrinkTime * captureThresholdPercentage;
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();

        Invoke(nameof(StartCapturing), startDelay);
    }

    private void StartCapturing()
    {
        isCapturing = true;
        Debug.Log("[CaptureArea] Área de captura activada.");
        CheckInitialFrogPosition();
    }

    private void CheckInitialFrogPosition()
    {
        float checkRadius = transform.localScale.x / 2f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, checkRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.transform == targetFrog)
            {
                hasFrogExited = false;
                Debug.Log("[CaptureArea] Rana detectada en posición inicial.");
                break;
            }
        }
    }
    #endregion

    #region Lógica Principal
    private void Update()
    {
        if (Input.GetKeyDown(cancelKey))
        {
            CancelCapture();
            return;
        }

        UpdatePosition();

        if (isCapturing)
        {
            ProcessCapture();
        }
    }

    private void UpdatePosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        }
    }

    private void ProcessCapture()
    {
        captureTimer += Time.deltaTime;

        float shrinkProgress = captureTimer / shrinkTime;
        float currentScale = Mathf.Lerp(1f, minScaleFactor, shrinkProgress);
        transform.localScale = initialScale * currentScale;

        if (captureTimer >= shrinkTime)
        {
            CompleteCaptureAttempt();
        }
    }

    private void CompleteCaptureAttempt()
    {
        if (captureComplete) return;
        captureComplete = true;

        bool wasSuccessful = captureTimer >= captureThreshold && !hasFrogExited;

        if (wasSuccessful)
        {
            FrogSpawner.Instance.CaptureFrog(frogType);
            if (targetFrog != null)
            {
                Destroy(targetFrog.gameObject);
            }
            Debug.Log("[CaptureArea] Captura exitosa.");

            if (audioSource != null && successClip != null)
            {
                audioSource.PlayOneShot(successClip);
            }
        }
        else
        {
            Debug.Log("[CaptureArea] Captura fallida.");

            if (audioSource != null && failClip != null)
            {
                audioSource.PlayOneShot(failClip);
            }
        }

        float delay = Mathf.Max(
            successClip != null ? successClip.length : 0f,
            failClip != null ? failClip.length : 0f,
            0.1f
        );

        Destroy(gameObject, delay);
    }

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
            Debug.Log("[CaptureArea] Rana entró en el área.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isCapturing && other.transform == targetFrog)
        {
            hasFrogExited = true;
            Debug.Log("[CaptureArea] Rana salió del área.");
        }
    }
    #endregion
}
