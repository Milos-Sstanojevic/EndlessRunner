using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private const float OffsetFromCenterOfChunk = 12f;
    private const int ChanceForRoadblock = 60;
    private const int ChanceForRightObstacle = 30;
    private const int ChanceForLeftObstacle = 30;
    private const int ChanceForLeftAndRightObstacle = 40;
    private const int ChanceForFlyingEnemy = 20;
    private const int ChanceForGroundEnemy = 20;
    private const int ChanceForCompleteChunk = 60;
    private const int ChanceForTwoEnemiesChunk = 45;
    private const int ChanceForFlyingEnemyChunk = 50;
    private const int ChanceForRandomChunk = 20;
    private const int ChanceForSpaceship = 70;
    private const int ChanceForJet = 20;
    private Dictionary<EnvironmentMovementController, int> chancesForObstacles;
    private Dictionary<CollectableController, int> chancesForCollectables;
    private Dictionary<ChunkController, int> chancesForChunks;
    private const float PosZ = 50f;
    private float chunkSpawnDelay = 3.5f;
    private bool canSpawn;
    private bool spawnedEnemy;
    private Vector3 endOfPreviousChunk;
    private ChunkController chunk;

    private void Awake()
    {
        InitializeChancesForObstacles();
        InitializeChancesForCollectables();
        InitializeChancesForChunks();
    }

    private void InitializeChancesForObstacles()
    {
        chancesForObstacles = new Dictionary<EnvironmentMovementController, int>
        {
            { PoolingSystemController.Instance.GetRoadblockObstaclePrefab(), ChanceForRoadblock },
            { PoolingSystemController.Instance.GetRightObstaclePrefab(), ChanceForRightObstacle},
            { PoolingSystemController.Instance.GetLeftObstaclePrefab(), ChanceForLeftObstacle },
            { PoolingSystemController.Instance.GetLeftAndRightObstaclePrefab(), ChanceForLeftAndRightObstacle},
            { PoolingSystemController.Instance.GetGroundEnemyPrefab().GetComponent<EnvironmentMovementController>(), ChanceForFlyingEnemy },
            { PoolingSystemController.Instance.GetFlyingEnemyPrefab().GetComponent<EnvironmentMovementController>(), ChanceForGroundEnemy}
        };
    }

    private void InitializeChancesForCollectables()
    {
        chancesForCollectables = new Dictionary<CollectableController, int>
        {
            { PoolingSystemController.Instance.GetSpaceshipPoolingSystem().GetBasePrefab(), ChanceForSpaceship },
            { PoolingSystemController.Instance.GetJetPoolingSystem().GetBasePrefab().GetComponent<CollectableController>(), ChanceForJet }
        };
    }

    private void InitializeChancesForChunks()
    {
        chancesForChunks = new Dictionary<ChunkController, int>
        {
            {PoolingSystemController.Instance.GetCompleteChunk(),ChanceForCompleteChunk},
            {PoolingSystemController.Instance.GetChunkWithTwoEnemies(),ChanceForTwoEnemiesChunk},
            {PoolingSystemController.Instance.GetChunkWithFlyingEnemy(),ChanceForFlyingEnemyChunk},
            {PoolingSystemController.Instance.GetChunkWithRandomObstacles(),ChanceForRandomChunk}
        };
    }

    private void OnEnable()
    {
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
            totalWeight += chance.Value;

        int randomValue = Random.Range(0, totalWeight + 1);
        int coursor = 0;
        foreach (var chance in chancesForChunks)
        {
            coursor += chance.Value;
            if (coursor >= randomValue)
            {
                PoolingSystemController.Instance.GetChunkPoolingSystem().SetBasePrefab(chance.Key);
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
        if (IsChunkNotPremade(randomObstaclesOnChunk, randomCollectablesOnChunk))
        {
            spawnedEnemy = false;
            HandleObstacleSpawning(randomObstaclesOnChunk);
            HandleCollectableSpawning(randomCollectablesOnChunk);
        }
    }

    private void SetPositionOfChunk()
    {
        if (endOfPreviousChunk == Vector3.zero)
            chunk.transform.position = new Vector3(chunk.transform.position.x, chunk.transform.position.y, PosZ);
        else
            chunk.transform.position = new Vector3(chunk.transform.position.x, chunk.transform.position.y, endOfPreviousChunk.z + OffsetFromCenterOfChunk);
    }

    //If lists for positions of obstacles are not empty, it is not premade chunk
    private bool IsChunkNotPremade(List<GameObject> randomObstaclesOnChunk, List<GameObject> randomCollectablesOnChunk)
    {
        return randomObstaclesOnChunk.Count != 0 && randomCollectablesOnChunk.Count != 0;
    }

    private void HandleObstacleSpawning(List<GameObject> randomObstaclesOnChunk)
    {
        foreach (GameObject obstaclePos in randomObstaclesOnChunk)
            PickTypeOfObstacle(obstaclePos);
    }

    private void PickTypeOfObstacle(GameObject obstaclePos)
    {
        int totalWeight = 0;

        foreach (var chancePair in chancesForObstacles)
            totalWeight += chancePair.Value;

        int randomValue = Random.Range(0, totalWeight + 1);

        int coursor = 0;
        foreach (var chance in chancesForObstacles)
        {
            spawnedEnemy = false;
            coursor += chance.Value;
            if (coursor >= randomValue)
            {
                if (chance.Key.GetComponent<EnemyController>() != null)
                {
                    spawnedEnemy = true;
                    PoolingSystemController.Instance.GetEnemyPoolingSystem().SetBasePrefab(chance.Key.GetComponent<EnemyController>());
                    EnemyController enemy = PoolingSystemController.Instance.GetEnemyPoolingSystem().GetObjectFromPool();
                    SetPositionAndParentOfObjectFromPool(enemy.gameObject, obstaclePos.transform.position);
                }
                else
                {
                    PoolingSystemController.Instance.GetObstaclesPoolingSystem().SetBasePrefab(chance.Key);
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
        chunk.AddObjectToList(obj.gameObject);
    }

    //In inspector, always set position for gun spawning first in list
    private void HandleCollectableSpawning(List<GameObject> randomCollectablesOnChunk)
    {
        foreach (GameObject collectablePos in randomCollectablesOnChunk)
            PickTypeOfCollectable(collectablePos);
    }

    private void PickTypeOfCollectable(GameObject collectablePos)
    {

        if (spawnedEnemy == true)
            SpawnGun(collectablePos.transform.position);
        else
            CollectableWeightedRandomAlgorithm(collectablePos);
    }

    private void CollectableWeightedRandomAlgorithm(GameObject collectablePos)
    {
        int totalWeight = 0;

        foreach (var chance in chancesForCollectables)
            totalWeight += chance.Value;

        int randomValue = Random.Range(0, totalWeight);

        int coursor = 0;

        foreach (var chance in chancesForCollectables)
        {
            coursor += chance.Value;
            if (coursor >= randomValue)
            {
                if (chance.Key.GetComponent<JetController>() != null)
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
        StopCoroutine(SpawnChunks());
    }
}
