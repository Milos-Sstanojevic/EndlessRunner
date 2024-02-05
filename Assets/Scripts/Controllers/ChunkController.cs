using System;
using System.Collections.Generic;
using UnityEngine;

public class ChunkController : MonoBehaviour
{
    [SerializeField] private Transform startOfChunk;
    [SerializeField] private Transform endOfChunk;
    [SerializeField] private List<GameObject> positionsForRandomObstaclesOnChunk;
    [SerializeField] private List<GameObject> positionsForRandomCollectablesOnChunk;
    private List<GameObject> spawnedObjects;
    private Dictionary<GameObject, Vector3> initialObjectPositions;


    private void Awake()
    {
        spawnedObjects = new List<GameObject>();
        initialObjectPositions = new Dictionary<GameObject, Vector3>();
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
        Destroy();
    }

    private void Destroy()
    {
        if (transform.position.z <= MapEdgeConstants.PositionBehindPlayerAxisZ)
        {
            ResetChunk();
            EventManager.Instance.OnChunkDestroyed(this);
            RespawnMissingObjects();
        }
    }

    public List<GameObject> GetPositionsForRandomObstaclesOnChunk() => positionsForRandomObstaclesOnChunk;

    public List<GameObject> GetPositionsForRandomCollectablesOnChunk() => positionsForRandomCollectablesOnChunk;

    private void ResetChunk()
    {
        Dictionary<Type, Action<Component>> eventHandlers = new Dictionary<Type, Action<Component>>
        {
            { typeof(GunController), c => EventManager.Instance.OnGunDestroyed((GunController)c) },
            { typeof(JetController), c => EventManager.Instance.OnJetDestroyed((JetController)c) },
            { typeof(CollectableController), c => EventManager.Instance.OnSpaceshipDestroyed((CollectableController)c) },
            { typeof(EnemyController), c => EventManager.Instance.OnEnemyDestroyed((EnemyController)c) },
            { typeof(EnvironmentMovementController), c => EventManager.Instance.OnEnvironmentDestroyed((EnvironmentMovementController)c) }
        };

        foreach (GameObject obj in spawnedObjects)
        {
            if (!obj.activeSelf)
                continue;

            foreach (var keyValuePair in eventHandlers)
            {
                Component component = obj.GetComponent(keyValuePair.Key);
                if (component != null)
                {
                    keyValuePair.Value(component);
                    break;
                }
            }
        }

        spawnedObjects.Clear();
    }

    public void AddObjectToList(GameObject spawnedObject)
    {
        spawnedObjects.Add(spawnedObject);
    }

    public Vector3 GetEndOfChunk() => endOfChunk.position;

    private void RespawnMissingObjects()
    {
        foreach (var initialObjectPosition in initialObjectPositions)
        {
            GameObject initialObject = initialObjectPosition.Key;
            if (!IsObjectActiveInChunk(initialObject))
            {
                RespawnObject(initialObject);
            }
        }
    }

    private bool IsObjectActiveInChunk(GameObject obj)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject == obj && child.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    private void RespawnObject(GameObject obj)
    {
        Type objectType = GetObjectType(obj);

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
        else if (objectType == typeof(GunController))
        {
            GunController gun = PoolingSystemController.Instance.GetGunPoolingSystem().GetObjectFromPool();
            gun.transform.SetParent(transform);
            gun.transform.localPosition = originalPosition;
        }
        else if (objectType == typeof(CollectableController))
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


    private Type GetObjectType(GameObject obj)
    {
        if (obj.GetComponent<GunController>() != null)
        {
            return typeof(GunController);
        }
        else if (obj.GetComponent<JetController>() != null)
        {
            return typeof(JetController);
        }
        else if (obj.GetComponent<CollectableController>() != null)
        {
            return typeof(CollectableController);
        }
        else
        {
            Debug.LogWarning("Unknown object type");
            return null;
        }
    }
}
