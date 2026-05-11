using System.Collections.Generic;
using UnityEngine;

public class CandySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject candyPrefab;
    public Transform player;

    [Header("Spawn Settings")]
    public int candiesCount = 10;
    public float distanceBetween = 10f;

    [Header("Lanes")]
    public float[] lanesX = { -2f, 0f, 2f };

    private List<GameObject> spawnedCandies = new();

    private float lastSpawnZ;

    private void Start()
    {
        GenerateStartCandies();
        GameManager.Instance.OnRestart += RestartSpawner;
    }

    private void Update()
    {
        HandleCandies();
    }

    private void GenerateStartCandies()
    {
        for (int i = 0; i < candiesCount; i++) SpawnCandy();
    }

    private void HandleCandies()
    {
        spawnedCandies.RemoveAll(c => c == null);
        while (spawnedCandies.Count < candiesCount) SpawnCandy();
        if (spawnedCandies.Count == 0) return;
        GameObject firstCandy = spawnedCandies[0];
        if (player.position.z - firstCandy.transform.position.z > 10f)
        {
            spawnedCandies.RemoveAt(0);
            Destroy(firstCandy);
            SpawnCandy();
        }
    }

    private void SpawnCandy()
    {
        float randomX = lanesX[Random.Range(0, lanesX.Length)];
        Vector3 spawnPosition = new(randomX, 0.5f, lastSpawnZ + distanceBetween);
        GameObject candy = Instantiate(candyPrefab, spawnPosition, Quaternion.identity);
        spawnedCandies.Add(candy);
        lastSpawnZ += distanceBetween;
    }

    private void RestartSpawner()
    {
        foreach (GameObject candy in spawnedCandies) Destroy(candy);
        spawnedCandies.Clear();
        lastSpawnZ = player.position.z;
        GenerateStartCandies();
    }
}