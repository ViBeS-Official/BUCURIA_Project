using System;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
    [Serializable]
    public class Spawnable
    {
        public GameObject prefab;
        [Min(0.01f)] public float weight = 1f;
        public bool randomRotateY180;
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

    [Serializable]
    public class GeneratedBiome
    {
        public string biomeName;
        public float startZ;
        public float endZ;

        public GeneratedBiome(string biomeName, float startZ, float endZ)
        {
            this.biomeName = biomeName;
            this.startZ = startZ;
            this.endZ = endZ;
        }

        public bool Contains(float z)
        {
            return z >= startZ && z < endZ;
        }
    }

    [Header("Biomes")]
    public List<Biome> _biomes = new();

    public int _startBiomeIndex = 0;

    [Header("Spawner Settings")]
    public int _startSegments = 6;
    public float _segmentLength = 20f;
    public float _zOffset = 10f;

    [Tooltip("На каком расстоянии вперёд создавать сегменты")]
    public float _spawnForwardDistance = 100f;

    [Tooltip("На каком расстоянии удалять сегменты")]
    public float _destroyBehindDistance = 40f;

    private readonly List<GameObject> _spawnedObjects = new();
    private float _lastSpawnZ;
    private int _currentBiomeIndex;
    private int _biomeSegmentsLeft;

    private readonly List<GeneratedBiome> _generatedBiomes = new();
    private string _currentPlayerBiome;

    private void Start()
    {
        InitializeBiome(_startBiomeIndex);
        if (GameManager.Instance != null) GameManager.Instance.OnStartGame += RestartSpawner;
        if (GameManager.Instance != null) GameManager.Instance.OnMenu += DestroyAll;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnStartGame -= RestartSpawner;
        if (GameManager.Instance != null) GameManager.Instance.OnMenu -= DestroyAll;
    }

    private void Update()
    {
        if (GameManager.Instance?.gameState != GameState.GameStart) return;
        SpawnObjects();
        CleanupObjects();
        CheckBiomeAudio();
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
        Spawnable spawnable = GetRandomSpawnable(biome);
        if (spawnable != null)
        {
            Vector3 spawnPosition = new(0f, 0f, _lastSpawnZ + _zOffset);
            Quaternion rotation = Quaternion.identity;
            if (spawnable.randomRotateY180)
            {
                int yRotation = UnityEngine.Random.value > 0.5f ? 0 : 180;
                rotation = Quaternion.Euler(0f, yRotation, 0f);
            }
            GameObject obj = Instantiate(spawnable.prefab, spawnPosition, rotation, transform);
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
        List<int> availableBiomes = new();
        for (int i = 0; i < _biomes.Count; i++)
        {
            if (i == previousBiome) continue;
            if (!ShopManager.Instance.HasItem(_biomes[i].name)) continue;
            availableBiomes.Add(i);
        }
        if (availableBiomes.Count == 0)
        {
            InitializeBiome(previousBiome);
            return;
        }
        int randomIndex = UnityEngine.Random.Range(0, availableBiomes.Count);
        int newBiome = availableBiomes[randomIndex];
        GameObject transitionPrefab = _biomes[previousBiome].transitionPrefab;
        if (transitionPrefab != null)
        {
            Vector3 transitionPos = new(0f, 0f, _lastSpawnZ + _zOffset);
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
        float startZ = _lastSpawnZ;
        float endZ = startZ + (_biomeSegmentsLeft * _segmentLength);
        _generatedBiomes.Add(new GeneratedBiome(biome.name, startZ, endZ));
    }

    private void CheckBiomeAudio()
    {
        if (!GameManager.Instance || GameManager.Instance.GetPlayer == null) return;
        float playerZ = GameManager.Instance.GetPlayer.GetPlayerTransform.position.z;
        string biome = GetBiomeAtZ(playerZ);
        if (_currentPlayerBiome != biome)
        {
            _currentPlayerBiome = biome;
            AudioManager.Instance?.Play(biome);
        }
    }

    public string GetBiomeAtZ(float z)
    {
        for (int i = 0; i < _generatedBiomes.Count; i++)
        {
            if (_generatedBiomes[i].Contains(z - _zOffset)) return _generatedBiomes[i].biomeName;
        }
        return _biomes[_startBiomeIndex].name;
    }

    private Spawnable GetRandomSpawnable(Biome biome)
    {
        if (biome.objects.Count == 0) return null;
        float totalWeight = 0f;
        foreach (Spawnable obj in biome.objects) totalWeight += obj.weight;
        float random = UnityEngine.Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        foreach (Spawnable obj in biome.objects)
        {
            currentWeight += obj.weight;
            if (random <= currentWeight) return obj;
        }
        return biome.objects[0];
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
        for (int i = _generatedBiomes.Count - 1; i >= 0; i--)
        {
            if (player.position.z > _generatedBiomes[i].endZ + _destroyBehindDistance)
            {
                _generatedBiomes.RemoveAt(i);
            }
        }
    }

    public void DestroyAll()
    {
        foreach (GameObject obj in _spawnedObjects) if (obj != null) Destroy(obj);
        _spawnedObjects.Clear();
    }
    public void RestartSpawner()
    {
        if (GameManager.Instance?.gameState != GameState.GameStart) return;
        DestroyAll();
        _generatedBiomes.Clear();
        _currentPlayerBiome = string.Empty;
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