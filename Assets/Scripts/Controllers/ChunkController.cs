using System;
using System.Collections.Generic;
using UnityEngine;

public class ChunkController : MonoBehaviour
{
    [SerializeField] private int chanceOfThisChunk;
    [SerializeField] private Transform startOfChunk;
    [SerializeField] private Transform endOfChunk;
    [SerializeField] private List<GameObject> positionsForRandomObstaclesOnChunk;
    [SerializeField] private List<GameObject> positionsForRandomCollectablesOnChunk;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private Dictionary<GameObject, Vector3> initialObjectPositions = new Dictionary<GameObject, Vector3>();


    private void Awake()
    {
        SaveInitialState();
    }

    private void SaveInitialState()
    {
        initialObjectPositions.Clear();

        foreach (Transform child in transform)
        {
            GameObject childObject = child.gameObject;
            initialObjectPositions[childObject] = childObject.transform.localPosition;
        }
    }

    private void Update()
    {
        if (transform.position.z <= MapEdgeConstants.PositionBehindPlayerAxisZ)
        {
            Destroy();
        }
    }

    private void Destroy()
    {
        ResetChunk();
        EventManager.Instance.OnChunkDestroyed(this);
        RespawnMissingObjects();
    }

    // private void ResetChunk()
    // {
    //     Dictionary<Type, Action<Component>> eventHandlers = new Dictionary<Type, Action<Component>>
    //     {
    //         { typeof(GunController), c => EventManager.Instance.OnGunDestroyed((GunController)c) },
    //         { typeof(JetController), c => EventManager.Instance.OnJetDestroyed((JetController)c) },
    //         { typeof(CollectableController), c => EventManager.Instance.OnSpaceshipDestroyed((CollectableController)c) },
    //         { typeof(EnemyController), c => EventManager.Instance.OnEnemyDestroyed((EnemyController)c) },
    //         { typeof(EnvironmentMovementController), c => EventManager.Instance.OnEnvironmentDestroyed((EnvironmentMovementController)c) }
    //     };

    //     foreach (GameObject obj in spawnedObjects)
    //     {
    //         if (!obj.activeSelf)
    //             continue;


    //         foreach (var keyValuePair in eventHandlers)
    //         {
    //             Component component = obj.GetComponent(keyValuePair.Key);
    //             if (component != null)
    //             {
    //                 keyValuePair.Value(component);
    //                 break;
    //             }
    //         }
    //     }

    //     spawnedObjects.Clear();
    // }

    private void ResetChunk()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (!obj.activeSelf)
                continue;

            IDestroyable destroyableComponent = obj.GetComponent<IDestroyable>();
            if (destroyableComponent != null)
            {
                destroyableComponent.Destroy();
            }
        }

        spawnedObjects.Clear();
    }

    private void RespawnMissingObjects()
    {
        foreach (var initialObjectPosition in initialObjectPositions)
        {
            GameObject initialObject = initialObjectPosition.Key;
            if (!IsObjectActiveInChunk(initialObject))
                RespawnObject(initialObject);
        }
    }

    private bool IsObjectActiveInChunk(GameObject obj)
    {
        foreach (Transform objectsInChunk in transform)
            if (objectsInChunk.gameObject == obj && objectsInChunk.gameObject.activeSelf)
                return true;

        return false;
    }

    private void RespawnObject(GameObject obj)
    {
        Vector3 originalPosition = GetOriginalObjectPosition(obj);

        if (obj.GetComponent<JetController>() != null)
        {
            JetController jet = PoolingSystemController.Instance.GetJetPoolingSystem().GetObjectFromPool();
            jet.transform.SetParent(transform);
            jet.transform.localPosition = originalPosition;
        }
        else if (obj.GetComponent<EnemyController>() != null)
        {
            EnemyController enemy = PoolingSystemController.Instance.GetEnemyPoolingSystem().GetObjectFromPool();
            enemy.transform.SetParent(transform);
            enemy.transform.localPosition = originalPosition;
        }
        else if (obj.GetComponent<GunController>() != null)
        {
            GunController gun = PoolingSystemController.Instance.GetGunPoolingSystem().GetObjectFromPool();
            gun.transform.SetParent(transform);
            gun.transform.localPosition = originalPosition;
        }
        else if (obj.GetComponent<CollectableController>() != null)
        {
            CollectableController spaceship = PoolingSystemController.Instance.GetSpaceshipPoolingSystem().GetObjectFromPool();
            spaceship.transform.SetParent(transform);
            spaceship.transform.localPosition = originalPosition;
        }

    }

    private Vector3 GetOriginalObjectPosition(GameObject obj)
    {
        if (initialObjectPositions.TryGetValue(obj, out Vector3 originalPosition))
        {
            return originalPosition;
        }

        return Vector3.zero;
    }

    public void AddObjectToList(GameObject spawnedObject)
    {
        spawnedObjects.Add(spawnedObject);
    }

    public Vector3 GetEndOfChunk() => endOfChunk.position;
    public List<GameObject> GetPositionsForRandomObstaclesOnChunk() => positionsForRandomObstaclesOnChunk;
    public List<GameObject> GetPositionsForRandomCollectablesOnChunk() => positionsForRandomCollectablesOnChunk;
    public int GetChanceForThisChunk() => chanceOfThisChunk;
}
