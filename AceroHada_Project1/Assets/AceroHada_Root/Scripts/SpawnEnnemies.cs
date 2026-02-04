using UnityEngine;
using System.Collections;
public class SpawnEnnemies : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject groundEnemyPrefab;
    [SerializeField] private GameObject bossPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int totalEnemiesBeforeBoss = 10;
    [SerializeField] private float timeBetweenSpawns = 5f;

    [Header("Boss Settings")]
    [SerializeField] private float delayBeforeBoss = 3f;
    [SerializeField] private Transform bossSpawnPoint;

    [Header("Auto Start")]
    [SerializeField] private bool startOnAwake = true;
    [SerializeField] private float initialDelay = 2f;

    [Header("Status")]
    [SerializeField] private int enemiesSpawned = 0;
    [SerializeField] private bool bossSpawned = false;
    [SerializeField] private bool isSpawning = false;

    private int enemiesAlive = 0;

    void Start()
    {
        if (startOnAwake)
        {
            Invoke("StartSpawning", initialDelay);
        }
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            StartCoroutine(SpawnWave());
        }
    }

    private IEnumerator SpawnWave()
    {
        isSpawning = true;

        while (enemiesSpawned < totalEnemiesBeforeBoss)
        {
            SpawnGroundEnemy();
            enemiesSpawned++;

            Debug.Log("Enemigo " + enemiesSpawned + "/" + totalEnemiesBeforeBoss + " spawneado");

            if (enemiesSpawned < totalEnemiesBeforeBoss)
            {
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }

        Debug.Log("Todos los enemigos pequenos spawneados. Esperando para invocar al Boss...");

        yield return new WaitForSeconds(delayBeforeBoss);

        SpawnBoss();

        isSpawning = false;
    }

    void SpawnGroundEnemy()
    {
        if (groundEnemyPrefab == null)
        {
            Debug.LogError("Ground Enemy Prefab no asignado!");
            return;
        }

        Transform spawn = spawnPoint != null ? spawnPoint : transform;
        GameObject enemy = Instantiate(groundEnemyPrefab, spawn.position, Quaternion.identity);

        enemiesAlive++;
    }

    void SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogError("Boss Prefab no asignado!");
            return;
        }

        if (bossSpawned)
        {
            Debug.LogWarning("El boss ya fue spawneado!");
            return;
        }

        Transform spawn = bossSpawnPoint != null ? bossSpawnPoint : (spawnPoint != null ? spawnPoint : transform);
        GameObject boss = Instantiate(bossPrefab, spawn.position, Quaternion.identity);

        bossSpawned = true;
        Debug.Log("BOSS SPAWNEADO!");
    }

    public void ResetSpawner()
    {
        StopAllCoroutines();
        enemiesSpawned = 0;
        bossSpawned = false;
        isSpawning = false;
        enemiesAlive = 0;
    }

    public void OnEnemyDied()
    {
        enemiesAlive--;
        if (enemiesAlive < 0) enemiesAlive = 0;
    }

    public int GetEnemiesSpawned()
    {
        return enemiesSpawned;
    }

    public int GetEnemiesAlive()
    {
        return enemiesAlive;
    }

    public bool IsBossSpawned()
    {
        return bossSpawned;
    }

    public bool IsSpawning()
    {
        return isSpawning;
    }

    void OnDrawGizmos()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
            Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + Vector3.up * 2f);
        }

        if (bossSpawnPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(bossSpawnPoint.position, 1f);
            Gizmos.DrawLine(bossSpawnPoint.position, bossSpawnPoint.position + Vector3.up * 3f);
        }
    }
}
