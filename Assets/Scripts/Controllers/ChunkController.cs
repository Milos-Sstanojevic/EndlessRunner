using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ChunkController : MonoBehaviour
{
    [SerializeField] private Transform startOfChunk;
    [SerializeField] private Transform endOfChunk;
    [SerializeField] private List<GameObject> positionsForRandomObstaclesOnChunk;
    [SerializeField] private List<GameObject> positionsForRandomCollectablesOnChunk;
    private List<GameObject> spawnedObjects;

    private void Awake()
    {
        spawnedObjects = new List<GameObject>();
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
}
