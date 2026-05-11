using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject obstaclePrefab;
    public Transform player;

    [Header("Spawn Settings")]
    public int obstaclesCount = 10;
    public float distanceBetween = 5f;

    [Range(0, 3)]
    public int minObstaclesPerWave = 1;

    [Range(0, 3)]
    public int maxObstaclesPerWave = 2;

    [Header("Lanes")]
    public float[] lanesX = { -2.5f, 0f, 2.5f };

    private readonly List<GameObject> spawnedObstacles = new();

    private float lastSpawnZ;

    private void Start()
    {
        GameManager.Instance.OnRestart += RestartSpawner;
    }

    private void Update()
    {
        HandleObstacles();
    }

    private void GenerateStartObstacles()
    {
        for (int i = 0; i < obstaclesCount; i++) TrySpawnObstacleWave();
    }

    private void HandleObstacles()
    {
        spawnedObstacles.RemoveAll(o => o == null);
        while (spawnedObstacles.Count < obstaclesCount) TrySpawnObstacleWave();
        if (spawnedObstacles.Count == 0) return;
        GameObject firstObstacle = spawnedObstacles[0];
        if (player.position.z - firstObstacle.transform.position.z > 15f)
        {
            spawnedObstacles.RemoveAt(0);
            Destroy(firstObstacle);
        }
    }

    private void TrySpawnObstacleWave()
    {
        lastSpawnZ += distanceBetween;
        int obstaclesToSpawn = Random.Range(minObstaclesPerWave, maxObstaclesPerWave + 1);
        List<int> freeLanes = new() { 0, 1, 2 };
        int spawnedCount = 0;
        while (spawnedCount < obstaclesToSpawn && freeLanes.Count > 0)
        {
            int randomLaneIndex = Random.Range(0, freeLanes.Count);
            int lane = freeLanes[randomLaneIndex];
            freeLanes.RemoveAt(randomLaneIndex);
            float x = lanesX[lane];
            Vector3 spawnPosition = new(x, 1f, lastSpawnZ);
            Collider[] overlaps = Physics.OverlapSphere(spawnPosition, 1f);
            bool blocked = false;
            foreach (Collider col in overlaps)
            {
                if (col.CompareTag("Obstacle") || col.CompareTag("Candy"))
                {
                    blocked = true;
                    break;
                }
            }
            if (blocked) continue;
            GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity, transform);
            spawnedObstacles.Add(obstacle);
            spawnedCount++;
        }
    }

    private void RestartSpawner()
    {
        foreach (GameObject obj in spawnedObstacles) Destroy(obj);
        spawnedObstacles.Clear();
        lastSpawnZ = Mathf.Floor(player.position.z / distanceBetween) * distanceBetween;
        GenerateStartObstacles();
    }
}