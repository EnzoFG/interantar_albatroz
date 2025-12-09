using UnityEngine;

public class WindSpawner : MonoBehaviour
{
    [Header("Prefabs dos Ventos")]
    public GameObject windBoostPrefab;
    public GameObject windSlowPrefab;

    [Header("Configuração de Tempo (em segundos)")]
    public float spawnIntervalMin = 2f;
    public float spawnIntervalMax = 5f;

    [Header("Configuração de Posição")]
    public float spawnXPosition = 12f;
    public float spawnYMin = -4f;
    public float spawnYMax = 4f;
    public float minVerticalDistance = 1.5f;

    private float timer;
    private float lastSpawnY = 0f;

    void Start()
    {
        SetNextSpawnTime();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SpawnWind();
            SetNextSpawnTime();
        }
    }

    void SetNextSpawnTime()
    {
        timer = Random.Range(spawnIntervalMin, spawnIntervalMax);
    }

    void SpawnWind()
    {
        GameObject prefabToSpawn = (Random.value > 0.5f) ? windBoostPrefab : windSlowPrefab;

        float spawnY;
        int attempts = 0;

        do
        {
            spawnY = Random.Range(spawnYMin, spawnYMax);
            attempts++;
        } 
        while (Mathf.Abs(spawnY - lastSpawnY) < minVerticalDistance && attempts < 10);

        Vector3 spawnPosition = new Vector3(spawnXPosition, spawnY, 0);

        lastSpawnY = spawnY;

        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
}