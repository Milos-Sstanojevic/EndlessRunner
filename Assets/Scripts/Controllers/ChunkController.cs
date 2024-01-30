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
        if (transform.position.z <= GlobalConstants.PositionBehindPlayerAxisZ)
        {
            ResetChunk();
            EventManager.Instance.OnChunkDestroyed(this);
        }
    }

    public List<GameObject> GetPositionsForRandomObstaclesOnChunk() => positionsForRandomObstaclesOnChunk;

    public List<GameObject> GetPositionsForRandomCollectablesOnChunk() => positionsForRandomCollectablesOnChunk;

    private void ResetChunk()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            EnvironmentMovementController obstacle = obj.GetComponent<EnvironmentMovementController>();
            if (obstacle != null)
            {
                EventManager.Instance.OnEnvironmentDestroyed(obstacle);
            }
        }

        spawnedObjects.Clear();
    }

    public void AddObjectToList(GameObject spawnedObject)
    {
        spawnedObjects.Add(spawnedObject);
    }
}
