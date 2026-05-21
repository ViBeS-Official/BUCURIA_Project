using System;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
    [Serializable]
    public class Spawnable
    {
        public GameObject prefab;

        [Min(0.01f)]
        public float weight = 1f;
    }

    [Serializable]
    public class Biome
    {
        public string name;

        [Header("Biome Length")]
        public int minLength = 10;
        public int maxLength = 25;

        [Header("Objects")]
        public List<Spawnable> objects = new();

        [Header("Transition")]
        public GameObject transitionPrefab;
    }

    [Header("Biomes")]
    public List<Biome> _biomes = new();

    public int _startBiomeIndex = 0;

    [Header("Spawner Settings")]
    public int _startSegments = 6;

    public float _segmentLength = 20f;

    [Tooltip("На каком расстоянии вперёд создавать сегменты")]
    public float _spawnForwardDistance = 100f;

    [Tooltip("На каком расстоянии удалять сегменты")]
    public float _destroyBehindDistance = 40f;

    private readonly List<GameObject> _spawnedObjects = new();
    private float _lastSpawnZ;
    private int _currentBiomeIndex;
    private int _biomeSegmentsLeft;

    private void Start()
    {
        InitializeBiome(_startBiomeIndex);
        GenerateStartObjects();
        if (GameManager.Instance != null) GameManager.Instance.OnRestart += RestartSpawner;
    }

    private void Update()
    {
        SpawnObjects();
        CleanupObjects();
    }

    private void GenerateStartObjects()
    {
        for (int i = 0; i < _startSegments; i++) CreateObject();
    }

    private void SpawnObjects()
    {
        if (!GameManager.Instance || GameManager.Instance.GetPlayer == null) return;
        Transform player = GameManager.Instance.GetPlayer.GetPlayerTransform;
        if (player.position.z + _spawnForwardDistance > _lastSpawnZ) CreateObject();
    }

    private void CreateObject()
    {
        if (_biomes.Count == 0) return;
        if (_biomeSegmentsLeft <= 0) SwitchToRandomBiome();
        Biome biome = _biomes[_currentBiomeIndex];
        GameObject prefab = GetRandomPrefab(biome);
        if (prefab != null)
        {
            Vector3 spawnPosition = new(0f, 0f, _lastSpawnZ);
            GameObject obj = Instantiate( prefab, spawnPosition, Quaternion.identity, transform);
            _spawnedObjects.Add(obj);
        }
        _biomeSegmentsLeft--;
        _lastSpawnZ += _segmentLength;
    }

    private void SwitchToRandomBiome()
    {
        if (_biomes.Count <= 1)
        {
            InitializeBiome(_currentBiomeIndex);
            return;
        }
        int previousBiome = _currentBiomeIndex;
        int newBiome;
        do newBiome = UnityEngine.Random.Range(0, _biomes.Count);
        while (newBiome == previousBiome);
        GameObject transitionPrefab = _biomes[previousBiome].transitionPrefab;
        if (transitionPrefab != null)
        {
            Vector3 transitionPos = new(0f, 0f, _lastSpawnZ);
            GameObject transition = Instantiate(transitionPrefab, transitionPos, Quaternion.identity, transform);
            _spawnedObjects.Add(transition);
            _lastSpawnZ += _segmentLength;
        }
        InitializeBiome(newBiome);
    }

    private void InitializeBiome(int biomeIndex)
    {
        _currentBiomeIndex = biomeIndex;
        Biome biome = _biomes[_currentBiomeIndex];
        _biomeSegmentsLeft = UnityEngine.Random.Range(biome.minLength, biome.maxLength + 1);
    }

    private GameObject GetRandomPrefab(Biome biome)
    {
        if (biome.objects.Count == 0) return null;
        float totalWeight = 0f;
        foreach (Spawnable obj in biome.objects) totalWeight += obj.weight;
        float random = UnityEngine.Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        foreach (Spawnable obj in biome.objects)
        {
            currentWeight += obj.weight;
            if (random <= currentWeight) return obj.prefab;
        }
        return biome.objects[0].prefab;
    }

    private void CleanupObjects()
    {
        if (!GameManager.Instance || GameManager.Instance.GetPlayer == null) return;
        Transform player = GameManager.Instance.GetPlayer.GetPlayerTransform;
        for (int i = _spawnedObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = _spawnedObjects[i];
            if (obj == null)
            {
                _spawnedObjects.RemoveAt(i);
                continue;
            }
            if (player.position.z - obj.transform.position.z > _destroyBehindDistance)
            {
                Destroy(obj);
                _spawnedObjects.RemoveAt(i);
            }
        }
    }

    public void RestartSpawner()
    {
        foreach (GameObject obj in _spawnedObjects) if (obj != null) Destroy(obj);
        _spawnedObjects.Clear();
        if (!GameManager.Instance || GameManager.Instance.GetPlayer == null) _lastSpawnZ = 0f;
        else
        {
            Transform player = GameManager.Instance.GetPlayer.GetPlayerTransform;
            _lastSpawnZ = Mathf.Floor(player.position.z / _segmentLength) * _segmentLength;
        }
        InitializeBiome(_startBiomeIndex);
        GenerateStartObjects();
    }
}