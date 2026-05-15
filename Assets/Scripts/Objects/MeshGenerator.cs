using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CandyVariant
{
    public string name;
    public GameObject prefab;
    [Range(0f, 100f)] public float weight = 1f;
}

public class MeshGenerator : MonoBehaviour
{
    [Header("Variants")]
    public List<CandyVariant> _variants = new();

    [Header("Mesh")]
    public GameObject _mesh;

    public void Generate(int seed)
    {
        if (_variants == null || _variants.Count == 0) return;
        System.Random rng = new(seed);
        CandyVariant selected = GetWeightedRandom(rng);
        CreateMesh(selected);
    }

    private CandyVariant GetWeightedRandom(System.Random rng)
    {
        float totalWeight = 0f;
        foreach (var v in _variants) totalWeight += Mathf.Max(0, v.weight);
        double randomValue = rng.NextDouble() * totalWeight;
        float current = 0f;
        foreach (var v in _variants)
        {
            current += Mathf.Max(0, v.weight);
            if (randomValue <= current) return v;
        }
        return _variants[_variants.Count - 1];
    }

    private void CreateMesh(CandyVariant variant)
    {
        if (variant == null || variant.prefab == null) return;
        _mesh = Instantiate(variant.prefab, transform);
    }
}