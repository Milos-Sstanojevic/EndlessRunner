using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private const float OffsetFromCenterOfChunk = 12f;
    private const int ChanceForRegularChunk = 30;
    private const int ChanceForTwoEnemiesChunk = 60;
    private const int ChanceForFlyingEnemyChunk = 90;
    private const int ChanceForRandomChunk = 100;
    private const int ChanceForSpaceship = 85;
    private const int ChanceForJet = 100;
    private Dictionary<EnvironmentMovementController, int> chancesForObstacles;
    private Dictionary<CollectableController, int> chancesForCollectables;
    private Dictionary<ChunkController, int> chancesForChunks;
    private const int HundredPercent = 101;
    private const int ZeroPercent = 0;
    private const float PosZ = 50f;
    private float chunkSpawnDelay = 3.5f;
    private bool canSpawn;
    private bool spawnedEnemy;
    private Vector3 endOfPreviousChunk;
    private ChunkController chunk;

    private void Awake()
    {
        chancesForObstacles = new Dictionary<EnvironmentMovementController, int>
        {
            { PoolingSystemController.Instance.GetRoadblockObstaclePrefab(), 60 },
            { PoolingSystemController.Instance.GetRightObstaclePrefab(), 40 },
            { PoolingSystemController.Instance.GetLeftObstaclePrefab(), 40 },
            { PoolingSystemController.Instance.GetLeftAndRightObstaclePrefab(), 50 },
            { PoolingSystemController.Instance.GetGroundEnemyPrefab().GetComponent<EnvironmentMovementController>(), 20 },
            { PoolingSystemController.Instance.GetFlyingEnemyPrefab().GetComponent<EnvironmentMovementController>(), 20 }
        };

        chancesForCollectables = new Dictionary<CollectableController, int>
        {
            { PoolingSystemController.Instance.GetSpaceshipPoolingSystem().GetBasePrefab(), 70 },
            { PoolingSystemController.Instance.GetJetPoolingSystem().GetBasePrefab().GetComponent<CollectableController>(), 20 }
        };

        chancesForChunks = new Dictionary<ChunkController, int>
        {
            {PoolingSystemController.Instance.GetCompleteChunk(),60},
            {PoolingSystemController.Instance.GetChunkWithTwoEnemies(),45},
            {PoolingSystemController.Instance.GetChunkWithFlyingEnemy(),50},
            {PoolingSystemController.Instance.GetChunkWithRandomObstacles(),20}
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
        {
            SpawnGun(collectablePos.transform.position);
            spawnedEnemy = false;
        }
        else
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
                    {
                        SpawnJet(collectablePos.transform.position);
                    }
                    else
                    {
                        SpawnSpaceship(collectablePos.transform.position);
                    }
                    break;
                }
            }
        }
    }

    private void SpawnGun(Vector3 positionToSpawn)
    {
        GunController gun = PoolingSystemController.Instance.GetGunPoolingSystem().GetObjectFromPool();
        SetPositionAndParentOfObjectFromPool(gun.gameObject, positionToSpawn);
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
