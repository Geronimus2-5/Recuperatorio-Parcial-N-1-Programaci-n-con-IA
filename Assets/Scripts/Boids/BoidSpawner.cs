using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    [Header("Zona de Movimiento")]
    public BoidZoneBounds zoneBounds;

    [Header("Configuración de Boids")]
    public GameObject boidPrefab;
    public int boidCount = 6;

    [Header("Generación de Objetos de Interés")]
    public GameObject poiPrefab;
    public float poiSpawnInterval = 6f;
    public int maxActivePOIs = 5;
    private float poiTimer = 0f;

    private void Awake()
    {
        if (zoneBounds == null)
            zoneBounds = FindFirstObjectByType<BoidZoneBounds>();
    }

    private void Start()
    {
        SpawnInitialBoids();
    }

    private void Update()
    {
        if (poiPrefab == null || zoneBounds == null) return;

        poiTimer += Time.deltaTime;
        if (poiTimer >= poiSpawnInterval)
        {
            poiTimer = 0f;
            TrySpawnPOI();
        }
    }

    private void SpawnInitialBoids()
    {
        if (boidPrefab == null || zoneBounds == null) return;

        for (int i = 0; i < boidCount; i++)
        {
            Vector3 spawnPos = zoneBounds.GetRandomPositionInside(2f);
            GameObject boidObj = Instantiate(boidPrefab, spawnPos, Quaternion.identity);
            boidObj.name = $"Boid_{i + 1}";

            BoidTarget boidScript = boidObj.GetComponent<BoidTarget>();
            if (boidScript != null)
            {
                boidScript.zoneBounds = zoneBounds;
            }
        }
    }

    private void TrySpawnPOI()
    {
        PointOfInterest[] activePOIs = FindObjectsByType<PointOfInterest>(FindObjectsSortMode.None);
        if (activePOIs.Length < maxActivePOIs)
        {
            Vector3 spawnPos = zoneBounds.GetRandomPositionInside(2f);
            Instantiate(poiPrefab, spawnPos, Quaternion.identity);
        }
    }
}