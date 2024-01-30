using System.Collections.Generic;
using UnityEngine;

public class ChunkPoolingSystem : PoolingSystemBase<ChunkController>
{
    public static ChunkPoolingSystem Instance { get; private set; }

    [SerializeField] private List<ChunkController> typesOfChunk;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        EventManager.Instance.OnChunkDestroyAction += DestroyObjects;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnChunkDestroyAction -= DestroyObjects;
    }

    public void SetChunkWithTwoEnemiesAsBase()
    {
        base.objectPrefab = typesOfChunk[0];
    }

    public void SetChunkWithFlyingEnemyAsBase()
    {
        base.objectPrefab = typesOfChunk[1];
    }

    public void SetCompleteChunkAsBase()
    {
        base.objectPrefab = typesOfChunk[2];
    }

    public void SetChunkWithRandomObstaclesAsBase()
    {
        base.objectPrefab = typesOfChunk[3];
    }
}
