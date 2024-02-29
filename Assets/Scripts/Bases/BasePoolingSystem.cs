using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Pool;

public abstract class BasePoolingSystem<T> : NetworkBehaviour where T : NetworkBehaviour
{
    [SerializeField] private int defaultCapacity = 40;
    [SerializeField] private int maxCapacity = 100;
    private ObjectPool<T> objectPool;
    [SerializeField] protected T objectPrefab;
    [SerializeField] private Transform spawnedObjectsHolder;
    private List<T> instantiatedObjects;

    protected virtual void Awake()
    {
        CreatePool();
        instantiatedObjects = new List<T>();
    }

    private void CreatePool()
    {
        objectPool = new ObjectPool<T>(SpawnObject, OnGetAction, OnReleaseAction, OnDestroyAction, true, defaultCapacity, maxCapacity);
    }

    protected virtual T SpawnObject()
    {
        T obj = Instantiate(objectPrefab);
        obj.transform.SetParent(spawnedObjectsHolder);
        instantiatedObjects.Add(obj);
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
        instantiatedObjects.Remove(obj);
        Destroy(obj.gameObject);
    }

    protected void DestroyObjects(T obj)
    {
        obj.transform.SetParent(spawnedObjectsHolder);
        objectPool.Release(obj);
    }

    public void SetBasePrefab(T prefab)
    {
        objectPrefab = prefab;
    }

    public T GetBasePrefab() => objectPrefab;

    public List<T> GetInstantiatedObjects() => instantiatedObjects;
    public T GetObjectFromPool() => objectPool.Get();
}
