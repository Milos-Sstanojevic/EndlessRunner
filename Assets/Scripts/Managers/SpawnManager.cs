using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private const float OffsetFromCenterOfChunk = 12f;
    private const float minimumChunkSpawningDelay = 0.5f;
    private const float spawningDelayDecreaser = 0.15f;
    private List<EnvironmentMovementController> chancesForObstacles;
    private List<CollectableController> chancesForCollectables;
    private List<ChunkController> chancesForChunks;
    private const float PosZ = 50f;
    private float chunkSpawnDelay = 3.5f;
    private bool canSpawn;
    private bool spawnedEnemy;
    private Vector3 endOfPreviousChunk;
    private ChunkController chunk;
    private Vector3 offsetToRespectedStage;

    private void Awake()
    {
        InitializeChancesForObstacles();
        InitializeChancesForCollectables();
        InitializeChancesForChunks();
    }

    private void InitializeChancesForObstacles()
    {
        chancesForObstacles = new List<EnvironmentMovementController>
        {
            { PoolingSystemController.Instance.GetRoadblockObstaclePrefab()},
            { PoolingSystemController.Instance.GetRightObstaclePrefab()},
            { PoolingSystemController.Instance.GetLeftObstaclePrefab()},
            { PoolingSystemController.Instance.GetLeftAndRightObstaclePrefab()},
            { PoolingSystemController.Instance.GetGroundEnemyPrefab().GetComponent<EnvironmentMovementController>()},
            { PoolingSystemController.Instance.GetFlyingEnemyPrefab().GetComponent<EnvironmentMovementController>()}
        };
    }

    private void InitializeChancesForCollectables()
    {
        chancesForCollectables = new List<CollectableController>
        {
            { PoolingSystemController.Instance.GetSpaceshipPoolingSystem().GetBasePrefab() },
            { PoolingSystemController.Instance.GetJetPoolingSystem().GetBasePrefab().GetComponent<CollectableController>()}
        };
    }

    private void InitializeChancesForChunks()
    {
        chancesForChunks = new List<ChunkController>
        {
            {PoolingSystemController.Instance.GetCompleteChunk()},
            {PoolingSystemController.Instance.GetChunkWithTwoEnemies()},
            {PoolingSystemController.Instance.GetChunkWithFlyingEnemy()},
            {PoolingSystemController.Instance.GetChunkWithRandomObstacles()}
        };
    }

    private void OnEnable()
    {
        EventManager.Instance.SubscribeToOnDecreaseSpawningTimeOfChunk(DecreaseSpawningTime);

        StartCoroutine(SpawnChunks());
        endOfPreviousChunk = Vector3.zero;
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

        EventManager.Instance.OnObjectsInSceneChanged();
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

        int randomValue = Random.Range(0, totalWeight + 1);
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
                    SetPositionAndParentOfObjectFromPool(enemy.gameObject, obstaclePos.transform.position);
                }
                else
                {
                    PoolingSystemController.Instance.GetObstaclesPoolingSystem().SetBasePrefab(chance);
                    EnvironmentMovementController obstacle = PoolingSystemController.Instance.GetObstaclesPoolingSystem().GetObjectFromPool();
                    SetPositionAndParentOfObjectFromPool(obstacle.gameObject, obstaclePos.transform.position);
                }

                break;
            }
        }
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
        SetPositionAndParentOfObjectFromPool(gun.gameObject, positionToSpawn);
        spawnedEnemy = false;
    }

    private void SpawnJet(Vector3 positionToSpawn)
    {
        JetController jet = PoolingSystemController.Instance.GetJetPoolingSystem().GetObjectFromPool();
        SetPositionAndParentOfObjectFromPool(jet.gameObject, positionToSpawn);
    }

    private void SpawnSpaceship(Vector3 positionToSpawn)
    {
        CollectableController spaceship = PoolingSystemController.Instance.GetSpaceshipPoolingSystem().GetObjectFromPool();
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

    private void OnDisable()
    {
        EventManager.Instance.UnsubscribeToOnDecreaseSpawningTimeOfChunk(DecreaseSpawningTime);
        StopCoroutine(SpawnChunks());
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
        if (spawnManager == this)
        {
            float chunkSpawnDelay = GetChunkSpawnDelay();
            float spawnDelay;
            if (chunkSpawnDelay > minimumChunkSpawningDelay)
            {
                spawnDelay = HandleDecreasingSpawnDelay(chunkSpawnDelay);
                SetChunkSpawnDelay(spawnDelay);
            }
        }
    }

    private float HandleDecreasingSpawnDelay(float spawnDelay)
    {
        float delay = spawnDelay - spawningDelayDecreaser;
        if (delay < minimumChunkSpawningDelay)
            delay = minimumChunkSpawningDelay;
        return delay;
    }
}
