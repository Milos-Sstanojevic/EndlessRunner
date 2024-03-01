using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Fusion;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    private const float OffsetFromCenterOfChunk = 12f;
    private const float minimumChunkSpawningDelay = 0.5f;
    private const float spawningDelayDecreaser = 0.15f;
    private const float PosZ = 50f;

    [SerializeField] private OneScreenController screenOfSpawnManager;

    private List<EnvironmentMovementController> chancesForObstacles;
    private List<CollectableController> chancesForCollectables;
    private List<ChunkController> chancesForChunks;

    private float chunkSpawnDelay = 3.5f;
    private bool canSpawn;
    private bool spawnedEnemy;
    private Vector3 endOfPreviousChunk;
    private ChunkController chunk;
    private Vector3 offsetToRespectedStage;
    private int seedForPlayer;

    private void Awake()
    {
        InitializeChancesForObstacles();
        InitializeChancesForCollectables();
        InitializeChancesForChunks();

        Random.InitState(seedForPlayer);
    }

    private void OnEnable()
    {
        EventManager.Instance.SubscribeToOnDecreaseSpawningTimeOfChunk(DecreaseSpawningTime);
        EventManager.Instance.SubscribeToOnPlayerDeadAction(DisableSpawningForThisPlayer);

        StartCoroutine(SpawnChunks());
        endOfPreviousChunk = Vector3.zero;
    }

    private void InitializeChancesForObstacles()
    {
        chancesForObstacles = new List<EnvironmentMovementController>
        {
            PoolingSystemController.Instance.GetRoadblockObstaclePrefab(),
            PoolingSystemController.Instance.GetRightObstaclePrefab(),
            PoolingSystemController.Instance.GetLeftObstaclePrefab(),
            PoolingSystemController.Instance.GetLeftAndRightObstaclePrefab(),
            PoolingSystemController.Instance.GetGroundEnemyPrefab().GetComponent<EnvironmentMovementController>(),
            PoolingSystemController.Instance.GetFlyingEnemyPrefab().GetComponent<EnvironmentMovementController>()
        };
    }

    private void InitializeChancesForCollectables()
    {
        chancesForCollectables = new List<CollectableController>
        {
            PoolingSystemController.Instance.GetSpaceshipPoolingSystem().GetBasePrefab(),
            PoolingSystemController.Instance.GetJetPoolingSystem().GetBasePrefab().GetComponent<CollectableController>()
        };
    }

    private void InitializeChancesForChunks()
    {
        chancesForChunks = new List<ChunkController>
        {
            PoolingSystemController.Instance.GetCompleteChunk(),
            PoolingSystemController.Instance.GetChunkWithTwoEnemies(),
            PoolingSystemController.Instance.GetChunkWithFlyingEnemy(),
            PoolingSystemController.Instance.GetChunkWithRandomObstacles()
        };
    }

    public IEnumerator SpawnChunks()
    {
        while (canSpawn)
        {
            SpawnChunk();
            yield return new WaitForSeconds(chunkSpawnDelay);
        }
    }

    private void SpawnChunk()
    {
        SetEndOfPreviousChunk();
        PickTypeOfChunk();
        HandleChunkSpawning();

        EventManager.Instance.OnObjectsInSceneChanged(this);
    }

    private void SetEndOfPreviousChunk()
    {
        if (chunk != null)
            endOfPreviousChunk = chunk.GetEndOfChunk();
    }

    private void PickTypeOfChunk()
    {
        int totalWeight = 0;
        foreach (var chance in chancesForChunks)
            totalWeight += chance.GetChanceForThisChunk();

        int randomValue = Random.Range(0, totalWeight - 1);

        int coursor = 0;
        foreach (var chance in chancesForChunks)
        {
            coursor += chance.GetChanceForThisChunk();
            if (coursor >= randomValue)
            {
                PoolingSystemController.Instance.GetChunkPoolingSystem().SetBasePrefab(chance);
                break;
            }
        }
    }

    private void HandleChunkSpawning()
    {
        chunk = PoolingSystemController.Instance.GetChunkPoolingSystem().GetObjectFromPool();
        EnableMovementOnTakingFromPool(chunk.gameObject);
        chunk.SetSpawnManagerOfChunk(this);

        List<GameObject> randomObstaclesOnChunk = chunk.GetPositionsForRandomObstaclesOnChunk();
        List<GameObject> randomCollectablesOnChunk = chunk.GetPositionsForRandomCollectablesOnChunk();

        HandleSpawningOfPremadeChunk(randomObstaclesOnChunk, randomCollectablesOnChunk);
        SetPositionOfChunk();
    }

    private void HandleSpawningOfPremadeChunk(List<GameObject> randomObstaclesOnChunk, List<GameObject> randomCollectablesOnChunk)
    {
        if (IsChunkPremade(randomObstaclesOnChunk, randomCollectablesOnChunk))
            return;

        spawnedEnemy = false;
        HandleObstacleSpawning(randomObstaclesOnChunk);
        HandleCollectableSpawning(randomCollectablesOnChunk);
    }

    private void SetPositionOfChunk()
    {
        chunk.transform.position = endOfPreviousChunk == Vector3.zero
            ? new Vector3(offsetToRespectedStage.x, chunk.transform.position.y, PosZ)
            : chunk.transform.position = new Vector3(offsetToRespectedStage.x, chunk.transform.position.y, endOfPreviousChunk.z + OffsetFromCenterOfChunk);
    }

    //If lists for positions of obstacles are not empty, it is not premade chunk
    private bool IsChunkPremade(List<GameObject> randomObstaclesOnChunk, List<GameObject> randomCollectablesOnChunk) => randomObstaclesOnChunk.Count == 0 && randomCollectablesOnChunk.Count == 0;

    private void HandleObstacleSpawning(List<GameObject> randomObstaclesOnChunk)
    {
        foreach (GameObject obstaclePos in randomObstaclesOnChunk)
            PickTypeOfObstacle(obstaclePos);
    }

    private void PickTypeOfObstacle(GameObject obstaclePos)
    {
        int totalWeight = 0;

        foreach (var chancePair in chancesForObstacles)
            totalWeight += chancePair.GetChanceForThisObstacle();

        int randomValue = Random.Range(0, totalWeight + 1);

        int coursor = 0;
        foreach (var chance in chancesForObstacles)
        {
            spawnedEnemy = false;
            coursor += chance.GetChanceForThisObstacle();
            if (coursor >= randomValue)
            {
                if (chance.GetComponent<EnemyController>() != null)
                {
                    spawnedEnemy = true;
                    PoolingSystemController.Instance.GetEnemyPoolingSystem().SetBasePrefab(chance.GetComponent<EnemyController>());
                    EnemyController enemy = PoolingSystemController.Instance.GetEnemyPoolingSystem().GetObjectFromPool();
                    EnableMovementOnTakingFromPool(enemy.gameObject);
                    SetPositionAndParentOfObjectFromPool(enemy.gameObject, obstaclePos.transform.position);
                }
                else
                {
                    PoolingSystemController.Instance.GetObstaclesPoolingSystem().SetBasePrefab(chance);
                    EnvironmentMovementController obstacle = PoolingSystemController.Instance.GetObstaclesPoolingSystem().GetObjectFromPool();
                    EnableMovementOnTakingFromPool(obstacle.gameObject);
                    SetPositionAndParentOfObjectFromPool(obstacle.gameObject, obstaclePos.transform.position);
                }

                break;
            }
        }
    }

    private void EnableMovementOnTakingFromPool(GameObject gameObject)
    {
        EventManager.Instance.OnEnableMovementOfObject(gameObject);
    }

    private void SetPositionAndParentOfObjectFromPool(GameObject obj, Vector3 positionToSpawn)
    {
        obj.transform.position = positionToSpawn;
        obj.transform.SetParent(chunk.transform);
        chunk.AddObjectToList(obj);
    }

    //In inspector, always set position for gun spawning first in list
    private void HandleCollectableSpawning(List<GameObject> randomCollectablesOnChunk)
    {
        foreach (GameObject collectablePos in randomCollectablesOnChunk)
            PickTypeOfCollectable(collectablePos);
    }

    private void PickTypeOfCollectable(GameObject collectablePos)
    {
        if (spawnedEnemy)
            SpawnGun(collectablePos.transform.position);
        else
            CollectableWeightedRandomAlgorithm(collectablePos);
    }

    private void CollectableWeightedRandomAlgorithm(GameObject collectablePos)
    {
        int totalWeight = 0;
        foreach (var chance in chancesForCollectables)
            totalWeight += chance.GetChanceForThisCollectable();

        int randomValue = Random.Range(0, totalWeight);

        int coursor = 0;

        foreach (var chance in chancesForCollectables)
        {
            coursor += chance.GetChanceForThisCollectable();
            if (coursor >= randomValue)
            {
                if (chance.GetComponent<JetController>() != null)
                    SpawnJet(collectablePos.transform.position);
                else
                    SpawnSpaceship(collectablePos.transform.position);
                break;
            }
        }
    }

    private void SpawnGun(Vector3 positionToSpawn)
    {
        GunController gun = PoolingSystemController.Instance.GetGunPoolingSystem().GetObjectFromPool();
        EnableMovementOnTakingFromPool(gun.gameObject);
        SetPositionAndParentOfObjectFromPool(gun.gameObject, positionToSpawn);
        spawnedEnemy = false;
    }

    private void SpawnJet(Vector3 positionToSpawn)
    {
        JetController jet = PoolingSystemController.Instance.GetJetPoolingSystem().GetObjectFromPool();
        EnableMovementOnTakingFromPool(jet.gameObject);
        SetPositionAndParentOfObjectFromPool(jet.gameObject, positionToSpawn);
    }

    private void SpawnSpaceship(Vector3 positionToSpawn)
    {
        CollectableController spaceship = PoolingSystemController.Instance.GetSpaceshipPoolingSystem().GetObjectFromPool();
        EnableMovementOnTakingFromPool(spaceship.gameObject);
        SetPositionAndParentOfObjectFromPool(spaceship.gameObject, positionToSpawn);
    }

    public void EnableSpawning()
    {
        canSpawn = true;
    }

    public void DisableSpawning()
    {
        canSpawn = false;
    }

    public void DisableSpawningForThisPlayer(int id, PlayerController player, GameObject endScreen)
    {
        if (player.GetScreenOfPlayer() == screenOfSpawnManager)
            canSpawn = false;
    }

    public float GetChunkSpawnDelay() => chunkSpawnDelay;
    public void SetChunkSpawnDelay(float delay)
    {
        chunkSpawnDelay = delay;
    }

    public void SetOffsetToRespectedStage(Vector3 offset)
    {
        offsetToRespectedStage = offset;
    }

    private void DecreaseSpawningTime(SpawnManager spawnManager)
    {
        if (spawnManager != this)
            return;

        float chunkSpawnDelay = GetChunkSpawnDelay();
        float spawnDelay;
        if (chunkSpawnDelay > minimumChunkSpawningDelay)
        {
            spawnDelay = HandleDecreasingSpawnDelay(chunkSpawnDelay);
            SetChunkSpawnDelay(spawnDelay);
        }
    }

    private float HandleDecreasingSpawnDelay(float spawnDelay)
    {
        float delay = spawnDelay - spawningDelayDecreaser;

        if (delay < minimumChunkSpawningDelay)
            delay = minimumChunkSpawningDelay;

        return delay;
    }


    public void SetSeedForRandomness(int seed)
    {
        seedForPlayer = seed;
    }

    private void OnDisable()
    {
        EventManager.Instance.UnsubscribeToOnDecreaseSpawningTimeOfChunk(DecreaseSpawningTime);
        EventManager.Instance.SubscribeToOnPlayerDeadAction(DisableSpawningForThisPlayer);

        StopCoroutine(SpawnChunks());
    }
}
