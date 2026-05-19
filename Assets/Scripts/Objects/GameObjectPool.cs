using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool
{
    private GameObject _prefab;
    private Transform _parent;

    private readonly Queue<GameObject> _pool = new();

    public GameObjectPool(GameObject prefab, Transform parent, int initialSize = 10)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            var obj = Create();
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    private GameObject Create()
    {
        return Object.Instantiate(_prefab, _parent);
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject obj = _pool.Count > 0 ? _pool.Dequeue() : Create();

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);

        return obj;
    }

    public void Release(GameObject obj)
    {
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }
}