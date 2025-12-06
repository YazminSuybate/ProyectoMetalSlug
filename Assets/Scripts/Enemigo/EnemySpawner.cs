using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración de Spawn")]
    public GameObject enemyPrefab;

    public float minSpawnTime = 0.5f;
    public float maxSpawnTime = 1.5f;

    [Header("Posiciones de Spawn")]
    public Transform[] spawnPoints;

    private float spawnTimer;

    void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("El Prefab del enemigo no está asignado en el Spawner.");
            return;
        }
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No se han asignado puntos de spawn (Spawn Points).");
            return;
        }

        SetNewSpawnTimer();
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            SpawnEnemy();
            SetNewSpawnTimer();
        }
    }

    void SetNewSpawnTimer()
    {
        spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
    }

    void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        if (spawnPoint == null) return;

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
    }
}