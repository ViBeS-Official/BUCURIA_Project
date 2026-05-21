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
        public bool isPassable;
        [Tooltip("Частая генерация внутри ряда")] public bool isFrequent;

        [Header("Settings")]
        [Range(0f, 1f)] public float spawnChance = 1f;
        [Min(0)] public int maxPerWave = 1;
        public float spawnY = 1f;
        [HideInInspector] public readonly List<GameObject> spawnedObjects = new();
    }

    [Header("Objects")]
    public SpawnObject[] _objects;

    [Header("Seed")]
    [Tooltip("Если пусто — создаётся автоматически")] public string _seed;
    private System.Random _random;
    private int _seedHash;

    [Header("Lanes")]
    public float[] _lanesX = { -2.5f, 0f, 2.5f };

    [Header("Distance")]
    [Tooltip("Минимальный GRID шаг")] public float _minGridDistance = 2.5f;
    [Tooltip("Начальная дистанция между рядами")] public float _startDistanceBetween = 15f;
    [Tooltip("Минимальная дистанция между рядами")] public float _minDistanceBetween = 5f;

    private float _lastSpawnZ;

    [Header("Difficulty")]
    [Tooltip("Насколько быстро уменьшается дистанция")] public float _distanceDifficultyMultiplier = 0.02f;
    [Tooltip("Через сколько метров увеличивается сложность")] public float _difficultyDistanceStep = 50f;

    [Tooltip("Начальное количество НЕпроходимых объектов")][Range(0, 3)] public int _startBlockedObjectsPerWave = 1;
    [Tooltip("Максимальное количество НЕпроходимых объектов")][Range(1, 3)] public int _absoluteMaxBlockedObjectsPerWave = 2;

    private float _currentDistanceBetween;
    private int _currentBlockedObjectsPerWave;

    [Header("Optimization")]
    public float _destroyBehindDistance = 20f;
    [Tooltip("Сколько рядов заранее генерировать")] public int _generateAheadRows = 20;

    private void Start()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnStartGame += RestartGenerator;
        if (GameManager.Instance != null) GameManager.Instance.OnMenu += DestroyAll;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnStartGame -= RestartGenerator;
        if (GameManager.Instance != null) GameManager.Instance.OnMenu -= DestroyAll;
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
        _seedHash = _seed.GetHashCode();
        _random = new System.Random(_seedHash);
    }

    public void DestroyAll()
    {
        foreach (SpawnObject obj in _objects)
        {
            for (int i = 0; i < obj.spawnedObjects.Count; i++)
                if (obj.spawnedObjects[i] != null) Destroy(obj.spawnedObjects[i]);
            obj.spawnedObjects.Clear();
        }
    }
    private void RestartGenerator()
    {
        if (GameManager.Instance?.gameState != GameState.GameStart || !GameManager.Instance?.GetPlayer) return;
        InitializeSeed();
        DestroyAll();

        _currentDistanceBetween = _startDistanceBetween;
        _currentBlockedObjectsPerWave = _startBlockedObjectsPerWave;
        _lastSpawnZ = Mathf.Floor(GameManager.Instance.GetPlayer.GetPlayerTransform.position.z);
        for (int i = 0; i < _generateAheadRows; i++) GenerateWave();
    }

    private void UpdateDifficulty()
    {
        if (!GameManager.Instance || GameManager.Instance.GetPlayer == null) return;
        float distanceTravelled = GameManager.Instance.GetPlayer.GetPlayerTransform.position.z;
        int difficultyLevel = Mathf.FloorToInt(distanceTravelled / _difficultyDistanceStep);
        _currentDistanceBetween = Mathf.Max(_minDistanceBetween, _startDistanceBetween - (difficultyLevel * _distanceDifficultyMultiplier));
        _currentBlockedObjectsPerWave = Mathf.Clamp(_startBlockedObjectsPerWave + difficultyLevel, 1, _absoluteMaxBlockedObjectsPerWave);
    }

    private void GenerateAhead()
    {
        if (!GameManager.Instance || GameManager.Instance.GetPlayer == null) return;
        while (_lastSpawnZ < GameManager.Instance.GetPlayer.GetPlayerTransform.position.z + (_generateAheadRows * _currentDistanceBetween)) GenerateWave();
    }

    private void GenerateWave()
    {
        _lastSpawnZ += _currentDistanceBetween;
        GenerateBlockedObjects(_lastSpawnZ);
        GenerateFrequentObjects(_lastSpawnZ);
    }
    private void GenerateBlockedObjects(float waveZ)
    {
        int laneCount = _lanesX.Length;
        List<int> allLanes = new();
        for (int i = 0; i < laneCount; i++) allLanes.Add(i);
        HashSet<int> blockedLanes = new();
        int maxBlockedLanes = Mathf.Min(_currentBlockedObjectsPerWave, laneCount - 1);
        foreach (SpawnObject spawnObject in _objects)
        {
            if (spawnObject.prefab == null) continue;
            if (spawnObject.isPassable) continue;
            if (spawnObject.isFrequent) continue;
            Shuffle(allLanes);
            int spawnedCount = 0;
            foreach (int lane in allLanes)
            {
                if (blockedLanes.Count >= maxBlockedLanes) break;
                if (spawnedCount >= spawnObject.maxPerWave) break;
                if (_random.NextDouble() > spawnObject.spawnChance) continue;
                blockedLanes.Add(lane);
                Spawn(spawnObject, lane, waveZ);
                spawnedCount++;
            }
        }
    }
    private void GenerateFrequentObjects(float waveZ)
    {
        int subdivisions = Mathf.Max(1, Mathf.RoundToInt(_currentDistanceBetween / _minGridDistance));
        float step = _currentDistanceBetween / subdivisions;
        for (int s = 0; s < subdivisions; s++)
        {
            float subZ = waveZ - _currentDistanceBetween + (step * (s + 1));
            foreach (SpawnObject spawnObject in _objects)
            {
                if (spawnObject.prefab == null) continue;
                if (!spawnObject.isFrequent) continue;
                List<int> lanes = new();
                for (int i = 0; i < _lanesX.Length; i++) lanes.Add(i);
                Shuffle(lanes);
                int spawnedCount = 0;
                foreach (int lane in lanes)
                {
                    if (spawnedCount >= spawnObject.maxPerWave) break;
                    if (_random.NextDouble() > spawnObject.spawnChance) continue;
                    Spawn(spawnObject, lane, subZ);
                    spawnedCount++;
                }
            }
        }
    }
    private void Spawn(SpawnObject spawnObject, int lane, float z)
    {
        Vector3 spawnPosition = new(_lanesX[lane], spawnObject.spawnY, z);
        GameObject spawned = Instantiate(spawnObject.prefab, spawnPosition, Quaternion.identity, transform);
        spawned.GetComponent<MeshGenerator>().Generate(_random.Next());
        spawnObject.spawnedObjects.Add(spawned);
    }
    private void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = _random.Next(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private void CleanupObjects()
    {
        if (!GameManager.Instance || GameManager.Instance.GetPlayer == null) return;
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
                if (GameManager.Instance.GetPlayer.GetPlayerTransform.position.z - obj.transform.position.z > _destroyBehindDistance)
                {
                    Destroy(obj);
                    spawnObject.spawnedObjects.RemoveAt(i);
                }
            }
        }
    }
}