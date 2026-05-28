using System;
using System.Collections.Generic;
using UnityEngine;

public class RunnerWorldGenerator : MonoBehaviour
{
    [Serializable]
    public class SpawnObject
    {
        public string name;

        [Header("Prefab")]
        public GameObject prefab;

        [Header("Type")]
        public bool isPassable;
        public bool isFrequent;

        [Header("Biome")]
        public string[] biomesForSpawn;

        [Header("Raycast Placement")]
        public bool dontSpawnIfHasDetection;
        public bool spawnOnPointDetected;
        public float raycastHeight = 10f;

        [Header("Spawn Settings")]
        [Range(0f, 1f)] public float spawnChance = 1f;
        [Min(0.01f)] public float weight = 1f;
        [Min(1)] public int maxPerWave = 1;

        public float spawnY = 1f;

        [HideInInspector]
        public readonly List<GameObject> spawnedObjects = new();
    }

    private EnvironmentSpawner _environmentSpawner;

    [Header("Objects")]
    public SpawnObject[] _objects;

    [Header("Seed")]
    public string _seed;

    private System.Random _random;

    [Header("Lanes")]
    public float[] _lanesX = { -2.5f, 0f, 2.5f };

    [Header("Distance")]
    public float _minGridDistance = 2.5f;
    public float _startDistanceBetween = 15f;
    public float _minDistanceBetween = 5f;

    [Header("Difficulty")]
    public float _distanceDifficultyMultiplier = 0.02f;
    public float _difficultyDistanceStep = 50f;

    [Range(0, 3)] public int _startBlockedObjectsPerWave = 1;
    [Range(1, 3)] public int _absoluteMaxBlockedObjectsPerWave = 2;

    [Header("Optimization")]
    public float _destroyBehindDistance = 20f;
    public int _generateAheadRows = 20;

    private float _lastSpawnZ;
    private float _currentDistanceBetween;
    private int _currentBlockedObjectsPerWave;

    private void Start()
    {
        _environmentSpawner = FindObjectOfType<EnvironmentSpawner>();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStartGame += RestartGenerator;
            GameManager.Instance.OnMenu += DestroyAll;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStartGame -= RestartGenerator;
            GameManager.Instance.OnMenu -= DestroyAll;
        }
    }

    private void Update()
    {
        if (GameManager.Instance?.gameState != GameState.GameStart) return;
        UpdateDifficulty();
        CleanupObjects();
        GenerateAhead();
    }

    private void InitializeSeed()
    {
        if (string.IsNullOrWhiteSpace(_seed)) _seed = Guid.NewGuid().ToString();
        _random = new System.Random(_seed.GetHashCode());
    }

    private void RestartGenerator()
    {
        if (GameManager.Instance?.GetPlayer == null) return;
        InitializeSeed();
        DestroyAll();
        _currentDistanceBetween = _startDistanceBetween;
        _currentBlockedObjectsPerWave = _startBlockedObjectsPerWave;
        _lastSpawnZ = Mathf.Floor( GameManager.Instance.GetPlayer.GetPlayerTransform.position.z);
        for (int i = 0; i < _generateAheadRows; i++) GenerateWave();
    }

    public void DestroyAll()
    {
        foreach (SpawnObject obj in _objects)
        {
            foreach (GameObject spawned in obj.spawnedObjects) if (spawned != null) Destroy(spawned);
            obj.spawnedObjects.Clear();
        }
    }

    private void UpdateDifficulty()
    {
        if (GameManager.Instance?.GetPlayer == null) return;
        float distance = GameManager.Instance.GetPlayer.GetPlayerTransform.position.z;
        int difficulty = Mathf.FloorToInt(distance / _difficultyDistanceStep);
        _currentDistanceBetween = Mathf.Max(_minDistanceBetween, _startDistanceBetween - (difficulty * _distanceDifficultyMultiplier));
        _currentBlockedObjectsPerWave = Mathf.Clamp(_startBlockedObjectsPerWave + difficulty, 1, _absoluteMaxBlockedObjectsPerWave);
    }

    private void GenerateAhead()
    {
        if (GameManager.Instance?.GetPlayer == null) return;
        while (_lastSpawnZ < GameManager.Instance.GetPlayer.GetPlayerTransform.position.z + (_generateAheadRows * _currentDistanceBetween)) GenerateWave();
    }

    private void GenerateWave()
    {
        _lastSpawnZ += _currentDistanceBetween;
        GenerateObjects(_lastSpawnZ, false);
        int subdivisions = Mathf.Max(1, Mathf.RoundToInt(_currentDistanceBetween / _minGridDistance));
        float step = _currentDistanceBetween / subdivisions;
        for (int i = 0; i < subdivisions; i++)
        {
            float subZ = _lastSpawnZ - _currentDistanceBetween + (step * (i + 1));
            GenerateObjects(subZ, true);
        }
    }

    private void GenerateObjects(float z, bool frequent)
    {
        List<int> lanes = new();
        for (int i = 0; i < _lanesX.Length; i++) lanes.Add(i);
        Shuffle(lanes);
        int maxObjects = frequent ? _lanesX.Length : Mathf.Min(_currentBlockedObjectsPerWave, _lanesX.Length - 1);
        int spawned = 0;
        foreach (int lane in lanes)
        {
            if (spawned >= maxObjects) break;
            SpawnObject selected = GetWeightedObject(z, frequent);
            if (selected == null) continue;
            Spawn(selected, lane, z);
            spawned++;
        }
    }

    private SpawnObject GetWeightedObject(float z, bool frequent)
    {
        List<SpawnObject> valid = new();
        string biome = _environmentSpawner.GetBiomeAtZ(z);
        foreach (SpawnObject obj in _objects)
        {
            if (obj.prefab == null) continue;
            if (obj.isFrequent != frequent) continue;
            if (!frequent && obj.isPassable) continue;
            if (!ShopManager.Instance.HasItem(obj.name)) continue;
            if (!CanSpawnInBiome(obj, biome)) continue;
            if (_random.NextDouble() > obj.spawnChance) continue;
            valid.Add(obj);
        }
        if (valid.Count == 0) return null;
        float totalWeight = 0f;
        foreach (SpawnObject obj in valid) totalWeight += obj.weight;
        float random = (float)_random.NextDouble() * totalWeight;
        float current = 0f;
        foreach (SpawnObject obj in valid)
        {
            current += obj.weight;
            if (random <= current) return obj;
        }
        return valid[0];
    }

    private void Spawn(SpawnObject spawnObject, int lane, float z)
    {
        Vector3 spawnPosition = new(_lanesX[lane], 0f, z);
        if (spawnObject.dontSpawnIfHasDetection)
        {
            if (TryGetGroundPoint(spawnObject, spawnPosition, out RaycastHit hit))
            {
                if (!hit.transform.CompareTag("Untagged")) return;
            }
        }
        if (spawnObject.spawnOnPointDetected)
        {
            if (TryGetGroundPoint(spawnObject, spawnPosition, out RaycastHit hit)) spawnPosition = hit.point;
            else return;
        }
        GameObject spawned = Instantiate(spawnObject.prefab, spawnPosition + Vector3.up * spawnObject.spawnY, Quaternion.identity, transform);
        MeshGenerator generator = spawned.GetComponent<MeshGenerator>();
        if (generator != null) generator.Generate(_random.Next());
        spawnObject.spawnedObjects.Add(spawned);
    }

    private bool CanSpawnInBiome(SpawnObject obj, string biome)
    {
        if (obj.biomesForSpawn == null || obj.biomesForSpawn.Length == 0) return true;
        foreach (string b in obj.biomesForSpawn)
        {
            if (string.IsNullOrEmpty(b)) continue;
            if (b == biome) return true;
        }
        return false;
    }

    private bool TryGetGroundPoint(SpawnObject spawnObject, Vector3 basePos, out RaycastHit hit)
    {
        Vector3 origin = basePos + Vector3.up * spawnObject.raycastHeight;
        if (Physics.Raycast(origin, Vector3.down, out hit, spawnObject.raycastHeight * 2f)) return true;
        return false;
    }

    private void CleanupObjects()
    {
        if (GameManager.Instance?.GetPlayer == null) return;
        float playerZ = GameManager.Instance.GetPlayer.GetPlayerTransform.position.z;
        foreach (SpawnObject spawnObject in _objects)
        {
            for (int i = spawnObject.spawnedObjects.Count - 1; i >= 0; i--)
            {
                GameObject obj = spawnObject.spawnedObjects[i];
                if (obj == null)
                {
                    spawnObject.spawnedObjects.RemoveAt(i);
                    continue;
                }
                if (playerZ - obj.transform.position.z > _destroyBehindDistance)
                {
                    Destroy(obj);
                    spawnObject.spawnedObjects.RemoveAt(i);
                }
            }
        }
    }

    private void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = _random.Next(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}