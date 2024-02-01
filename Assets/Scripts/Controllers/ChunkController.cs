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
        foreach (GameObject obj in spawnedObjects)
        {
            if (!obj.activeSelf)
                continue;

            EnvironmentMovementController obstacle = obj.GetComponent<EnvironmentMovementController>();
            GunController gun = obj.GetComponent<GunController>();
            JetController jet = obj.GetComponent<JetController>();
            CollectableController spaceship = obj.GetComponent<CollectableController>();
            EnemyController enemy = obj.GetComponent<EnemyController>();

            if (gun != null)
            {
                EventManager.Instance.OnGunDestroyed(gun);
            }
            else if (jet != null)
            {
                EventManager.Instance.OnJetDestroyed(jet);
            }
            else if (spaceship != null)
            {
                EventManager.Instance.OnSpaceshipDestroyed(spaceship);
            }
            else if (enemy != null)
            {
                EventManager.Instance.OnEnemyDestroyed(enemy);
            }
            else if (obstacle != null)
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

    public Vector3 GetEndOfChunk() => endOfChunk.position;
}
