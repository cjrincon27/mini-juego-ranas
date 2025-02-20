using UnityEngine;
using System.Collections;

public class FrogSpawner : MonoBehaviour
{
    public static FrogSpawner Instance;
    public GameObject frogPrefab;
    public GameObject captureAreaPrefab;
    public Material[] frogMaterials;
    public AudioClip[] frogSounds;
    public float captureAreaShrinkTime = 5f;
    private int frogCount = 0;
    private int[] frogCaptures = new int[5];

    public int[] frogProbabilities = new int[5];
    private Vector3[] initialPositions = {
        new Vector3(0, 0, 0),
        new Vector3(0, 0, -2),
        new Vector3(0, 0, -4),
        new Vector3(-1, 0,-1),
        new Vector3(-1, 0, -4)
    };//opdemos editar pa posicion e las inciales 

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Generar las primeras 5 ranas en las posiciones especificadas en esta desactivamos el audio source pra q no reproduscan los sonidos  al tiempo 
        for (int i = 0; i < 5; i++)
        {
            SpawnFrog(false, initialPositions[i]);
        }

        // Luego, seguir con la generación periódica cada 10 segundos en (0,0,3)
        InvokeRepeating("SpawnFrog", 10f, 10f);
    }

    void SpawnFrog()
    {
        SpawnFrog(true, new Vector3(0, 0,3.5f));
    }

    void SpawnFrog(bool enableSound, Vector3 position)
    {
        int frogType = DetermineFrogType();
        Material frogMaterial = frogMaterials[frogType - 1];
        AudioClip frogSound = frogSounds[frogType - 1];

        GameObject frog = Instantiate(frogPrefab, position, Quaternion.identity);
        frog.name = "rana" + frogType + "_" + (++frogCount);

        Transform ranaBody = frog.transform.Find("RanaBody");
        if (ranaBody != null)
        {
            Renderer ranaBodyRenderer = ranaBody.GetComponent<Renderer>();
            if (ranaBodyRenderer != null)
            {
                ranaBodyRenderer.material = frogMaterial;
            }
        }

        // Asignar el audio solo si enableSound es true
        AudioSource frogAudioSource = frog.GetComponent<AudioSource>();
        if (frogAudioSource != null && frogSound != null)
        {
            frogAudioSource.clip = frogSound;
            frogAudioSource.enabled = enableSound; // Solo se activa si enableSound es true
            if (enableSound)
            {
                frogAudioSource.Play();
            }
        }

        FrogMovement movement = frog.AddComponent<FrogMovement>();
        movement.SetLifetime(Random.Range(60, 120));
        movement.SetRandomSpeed();
        movement.SetFrogType(frogType);
    }

    public void CaptureFrog(int frogType)
    {
        frogCaptures[frogType - 1]++;
        Debug.Log($"Captura de rana tipo {frogType}. Total capturas: {frogCaptures[frogType - 1]}");
        FrogCounterUI.Instance.IncrementCounter(frogType);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detecta clic izquierdo
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Frog")) // Asegúrate de que las ranas tengan la etiqueta "Frog"
                {
                    SpawnCaptureArea(hit.collider.gameObject);
                }
            }
        }
    }

    void SpawnCaptureArea(GameObject frog)
    {
        Vector3 spawnPosition = new Vector3(frog.transform.position.x, captureAreaPrefab.transform.position.y, frog.transform.position.z);
        GameObject captureArea = Instantiate(captureAreaPrefab, spawnPosition, Quaternion.identity);

        CaptureArea captureScript = captureArea.GetComponent<CaptureArea>();
        captureScript.Initialize(frog.transform, frog.GetComponent<FrogMovement>().GetFrogType());
        captureScript.shrinkTime = captureAreaShrinkTime;
    }

    int DetermineFrogType()
    {
        int totalProbability = 0;
        for (int i = 0; i < frogProbabilities.Length; i++)
        {
            totalProbability += frogProbabilities[i];
        }

        if (totalProbability != 100)
        {
            Debug.LogError("La suma total de las probabilidades debe ser 100. Se han encontrado: " + totalProbability);
            return 5;
        }

        int randomValue = Random.Range(0, 100);
        int cumulativeProbability = 0;

        for (int i = 0; i < frogProbabilities.Length; i++)
        {
            cumulativeProbability += frogProbabilities[i];
            if (randomValue < cumulativeProbability)
            {
                return i + 1;
            }
        }

        return 5;
    }
}
