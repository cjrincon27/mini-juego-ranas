using UnityEngine;
using UnityEngine.SceneManagement;

public class FrogSpawner : MonoBehaviour
{
    public static FrogSpawner Instance;

    [Header("Prefabs y Materiales")]
    public GameObject frogPrefab;
    public GameObject captureAreaPrefab;
    public Material[] frogMaterials;
    public AudioClip[] frogSounds;

    [Header("Configuración de Captura")]
    public float captureAreaShrinkTime = 5f;

    [Header("Tiempos de Generación")]
    public float initialSpawnDelay = 10f;     // Tiempo antes del primer spawn periódico (segundos)
    public float spawnInterval = 10f;         // Intervalo entre spawns periódicos (segundos)

    [Header("Vida de las Ranas (segundos)")]
    public float frogLifetimeMin = 60f;
    public float frogLifetimeMax = 120f;

    [Header("Velocidad de las Ranas (unidades/segundo)")]
    public float frogSpeedMin = 0.5f;
    public float frogSpeedMax = 1.5f;

    [Header("Boost de las Ranas (segundos)")]
    public float frogBoostDuration = 2f;

    [Header("Cambio de velocidad/dirección (segundos)")]
    public float frogMinChangeInterval = 5f;
    public float frogMaxChangeInterval = 15f;

    [Header("Probabilidades de cada tipo (suma = 100)")]
    public int[] frogProbabilities = new int[5]; // Cada elemento es la probabilidad porcentual de cada tipo

    private int frogCount = 0;
    private int[] frogCaptures = new int[5];

    private Vector3[] initialPositions = {
        new Vector3(0, 0, 0),
        new Vector3(0, 0, -2),
        new Vector3(0, 0, -4),
        new Vector3(-1, 0, -1),
        new Vector3(-1, 0, -4)
    };

    private static bool initialSpawnDone = false;
    private static bool periodicSpawnStarted = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 1) Spawn inicial (solo una vez)
        if (!initialSpawnDone)
        {
            for (int i = 0; i < initialPositions.Length; i++)
            {
                SpawnFrog(false, initialPositions[i]);
            }
            initialSpawnDone = true;
        }

        // 2) Spawn periódico (solo una vez)
        if (!periodicSpawnStarted)
        {
            InvokeRepeating(nameof(SpawnFrog), initialSpawnDelay, spawnInterval);
            periodicSpawnStarted = true;
        }
    }

    // Llamado por InvokeRepeating
    void SpawnFrog()
    {
        SpawnFrog(true, new Vector3(0, 0, 3.5f));
    }

    // Versión interna que realmente instancia la rana
    void SpawnFrog(bool enableSound, Vector3 position)
    {
        // 1) Determinar tipo de rana según probabilidad
        int frogType = DetermineFrogType();

        // 2) Obtener material y sonido según tipo
        Material frogMaterial = frogMaterials[frogType - 1];
        AudioClip frogSound = frogSounds[frogType - 1];

        // 3) Instanciar el prefab en la posición dada
        GameObject frog = Instantiate(frogPrefab, position, Quaternion.identity);
        frog.name = "rana" + frogType + "_" + (++frogCount);
        DontDestroyOnLoad(frog);

        // 4) Cambiar color/material del cuerpo si existe
        Transform ranaBody = frog.transform.Find("RanaBody");
        if (ranaBody != null)
        {
            Renderer renderer = ranaBody.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material = frogMaterial;
        }

        // 5) Asignar AudioSource si tiene
        AudioSource audio = frog.GetComponent<AudioSource>();
        if (audio != null && frogSound != null)
        {
            audio.clip = frogSound;
            audio.enabled = enableSound;
            if (enableSound) audio.Play();
        }

        // 6) Añadir componente FrogMovement y configurarlo
        FrogMovement movement = frog.AddComponent<FrogMovement>();

        // Pasamos todos los rangos y duraciones que configuraste en el Inspector:
        movement.ConfigureMovement(
            frogSpeedMin,
            frogSpeedMax,
            frogBoostDuration,
            frogMinChangeInterval,
            frogMaxChangeInterval
        );

        // 7) Asignar tipo de rana y tiempo de vida
        movement.SetFrogType(frogType);
        float life = Random.Range(frogLifetimeMin, frogLifetimeMax);
        movement.SetLifetime(life);
    }

    public void CaptureFrog(int frogType)
    {
        frogCaptures[frogType - 1]++;
        Debug.Log($"Captura de rana tipo {frogType}. Total capturas: {frogCaptures[frogType - 1]}");
        FrogCounterUI.Instance.IncrementCounter(frogType);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Frog"))
                {
                    SpawnCaptureArea(hit.collider.gameObject);
                }
            }
        }
    }

    void SpawnCaptureArea(GameObject frog)
    {
        Vector3 spawnPosition = new Vector3(
            frog.transform.position.x,
            captureAreaPrefab.transform.position.y,
            frog.transform.position.z
        );

        GameObject captureArea = Instantiate(captureAreaPrefab, spawnPosition, Quaternion.identity);
        CaptureArea script = captureArea.GetComponent<CaptureArea>();
        script.Initialize(frog.transform, frog.GetComponent<FrogMovement>().GetFrogType());
        script.shrinkTime = captureAreaShrinkTime;
    }

    int DetermineFrogType()
    {
        int total = 0;
        foreach (int prob in frogProbabilities)
        {
            total += prob;
        }

        if (total != 100)
        {
            Debug.LogError("La suma total de las probabilidades debe ser 100. Se han encontrado: " + total);
            return 5;
        }

        int randomValue = Random.Range(0, 100);
        int cumulative = 0;

        for (int i = 0; i < frogProbabilities.Length; i++)
        {
            cumulative += frogProbabilities[i];
            if (randomValue < cumulative)
            {
                return i + 1;
            }
        }

        return 5;
    }
}
