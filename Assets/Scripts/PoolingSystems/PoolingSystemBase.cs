using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class PoolingSystemBase<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private int defaultCapacity = 40;
    [SerializeField] private int maxCapacity = 100;
    private ObjectPool<T> objectPool;
    [SerializeField] private T objectPrefab;
    [SerializeField] private Transform spawnedObjectsHolder;

    protected virtual void Start()
    {
        CreatePool();
    }

    private void CreatePool()
    {
        objectPool = new ObjectPool<T>(SpawnObject, OnGetAction, OnReleaseAction, OnDestroyAction, true, defaultCapacity, maxCapacity);
    }

    private T SpawnObject()
    {
        T obj = Instantiate(objectPrefab);
        obj.transform.SetParent(spawnedObjectsHolder);
        return obj;
    }

    private void OnGetAction(T obj)
    {
        obj.gameObject.SetActive(true);
    }

    private void OnReleaseAction(T obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void OnDestroyAction(T obj)
    {
        Destroy(obj.gameObject);
    }

    protected void DestroyObject(T obj)
    {
        objectPool.Release(obj);
    }

    public T GetObjectFromPool() => objectPool.Get();
}
