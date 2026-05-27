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

        [Header("Unlock")]
        public bool unlocked = true;

        [Header("Biome Length")]
        public int minLength = 10;
        public int maxLength = 25;

        [Header("Objects")]
        public List<Spawnable> objects = new();

        [Header("Transition")]
        public GameObject transitionPrefab;
    }

    [Serializable]
    public class BiomeSaveData
    {
        public string biomeName;
        public bool unlocked;
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
        LoadBiomes();
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
            Vector3 spawnPosition = new(0f, 0f, _lastSpawnZ + 10f);
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
            if (!_biomes[i].unlocked) continue;
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
        AudioManager.Instance?.Play(biome.name);
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
        if (!GameManager.Instance || GameManager.Instance.GetPlayer == null) _lastSpawnZ = 0f;
        else
        {
            Transform player = GameManager.Instance.GetPlayer.GetPlayerTransform;
            _lastSpawnZ = Mathf.Floor(player.position.z / _segmentLength) * _segmentLength;
        }
        InitializeBiome(_startBiomeIndex);
        GenerateStartObjects();
    }

    public void UnlockBiome(string biomeName)
    {
        foreach (Biome biome in _biomes)
        {
            if (biome.name == biomeName)
            {
                biome.unlocked = true;
                SaveBiomes();
                return;
            }
        }
    }

    public bool IsBiomeUnlocked(string biomeName)
    {
        foreach (Biome biome in _biomes)
        {
            if (biome.name == biomeName)
            {
                return biome.unlocked;
            }
        }

        return false;
    }

    public string GetCurrentBiomeName() => _biomes[_currentBiomeIndex].name;

    public void SaveBiomes()
    {
        List<BiomeSaveData> save = new();
        foreach (Biome biome in _biomes)
        {
            save.Add(new BiomeSaveData()
            {
                biomeName = biome.name,
                unlocked = biome.unlocked
            });
        }
        string json = JsonUtility.ToJson(new Serialization<BiomeSaveData>(save), true);
        PlayerPrefs.SetString("Biomes", json);
    }

    public void LoadBiomes()
    {
        if (!PlayerPrefs.HasKey("Biomes")) return;
        string json = PlayerPrefs.GetString("Biomes");
        Serialization<BiomeSaveData> data = JsonUtility.FromJson<Serialization<BiomeSaveData>>(json);
        foreach (BiomeSaveData save in data.items)
        {
            foreach (Biome biome in _biomes) if (biome.name == save.biomeName) biome.unlocked = save.unlocked;
        }
    }

    [Serializable]
    public class Serialization<T>
    {
        public List<T> items;
        public Serialization(List<T> items) => this.items = items;
    }
}