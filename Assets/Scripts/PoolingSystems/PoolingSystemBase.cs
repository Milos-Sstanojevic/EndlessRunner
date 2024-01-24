using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolingSystemBase<T> : MonoBehaviour where T : EnviromentMovementBase
{
    private const int defaultCapacityForPool = 40;
    private const int maximumCapacityForPool = 100;
    public static PoolingSystemBase<T> Instance { get; private set; }
    [SerializeField] private Transform parentOfPool;
    private ObjectPool<T> _pool;
    private List<T> instantiatedObjects;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        instantiatedObjects = new List<T>();
        instantiatedObjects = CreateObjectPool();
    }

    private void OnEnable()
    {
        SubscribeToDestroyAction();
    }

    private void SubscribeToDestroyAction()
    {
        EventManager.Instance.OnDestroyAction += DestroyObjects;
    }

    private void DestroyObjects(EnviromentMovementBase obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(parentOfPool);
        _pool.Release((T)obj);
    }

    private List<T> CreateObjectPool()
    {
        _pool = new ObjectPool<T>(CreateObject, obj =>
        {
            obj.gameObject.SetActive(true);
        }, obj =>
        {
            obj.gameObject.SetActive(false);
        }, obj =>
        {
            UnsubscribeFromDestroyAction();
            Destroy(obj.gameObject);
        }, true, defaultCapacityForPool, maximumCapacityForPool);

        return instantiatedObjects;
    }

    protected virtual T CreateObject()
    {
        return Instantiate(GetPrefab());
    }

    protected virtual T GetPrefab()
    {
        return null; // To be implemented in the derived classes
    }

    private void OnDisable()
    {
        UnsubscribeFromDestroyAction();
    }

    private void UnsubscribeFromDestroyAction()
    {
        EventManager.Instance.OnDestroyAction -= DestroyObjects;
    }

    public T GetFromPool() => _pool.Get();

    public List<T> GetInstantiatedObjects() => instantiatedObjects;
}
