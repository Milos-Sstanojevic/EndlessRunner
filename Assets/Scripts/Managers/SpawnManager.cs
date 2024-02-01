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
    private const int ChanceForRoadblock = 20;
    private const int ChanceForDoubleRight = 36;
    private const int ChanceForDoubleLeft = 52;
    private const int ChanceForDoubleMid = 68;
    private const int ChanceForGroundEnemy = 84;
    private const int ChanceForFlyingEnemy = 100;
    private const int HundredPercent = 101;
    private const int ZeroPercent = 0;
    private const float PosZ = 50f;
    private float chunkSpawnDelay = 3.5f;
    private bool canSpawn;
    private bool spawnedEnemy;
    private Vector3 endOfPreviousChunk;
    private ChunkController chunk;

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
        {
            endOfPreviousChunk = chunk.GetEndOfChunk();
        }
    }

    private void PickTypeOfChunk()
    {
        int indexForChunk = Random.Range(ZeroPercent, HundredPercent);

        int[] chancesForChunks ={
            ChanceForRegularChunk,
            ChanceForTwoEnemiesChunk,
            ChanceForFlyingEnemyChunk,
            ChanceForRandomChunk
        };

        System.Action[] chunkPrefabSetters ={
            ChunkPoolingSystem.Instance.SetCompleteChunkAsBase,
            ChunkPoolingSystem.Instance.SetChunkWithTwoEnemiesAsBase,
            ChunkPoolingSystem.Instance.SetChunkWithFlyingEnemyAsBase,
            ChunkPoolingSystem.Instance.SetChunkWithRandomObstaclesAsBase
        };

        for (int i = 0; i < chancesForChunks.Length; i++)
        {
            if (indexForChunk <= chancesForChunks[i])
            {
                chunkPrefabSetters[i].Invoke();
                break;
            }
        }
    }

    private void HandleChunkSpawning()
    {
        chunk = ChunkPoolingSystem.Instance.GetObjectFromPool();

        List<GameObject> randomObstaclesOnChunk = chunk.GetPositionsForRandomObstaclesOnChunk();
        List<GameObject> randomCollectablesOnChunk = chunk.GetPositionsForRandomCollectablesOnChunk();

        HandleSpawningOfPremadeChunk(randomObstaclesOnChunk, randomCollectablesOnChunk);
        SetPositionOfChunk();
    }

    private void HandleSpawningOfPremadeChunk(List<GameObject> randomObstaclesOnChunk, List<GameObject> randomCollectablesOnChunk)
    {
        if (IsChunkPremade(randomObstaclesOnChunk, randomCollectablesOnChunk))
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

    //If lists for positions of obstcles are empty, it is premade chunk
    private bool IsChunkPremade(List<GameObject> randomObstaclesOnChunk, List<GameObject> randomCollectablesOnChunk)
    {
        return randomObstaclesOnChunk.Count != 0 && randomCollectablesOnChunk.Count != 0;
    }

    private void HandleObstacleSpawning(List<GameObject> randomObstaclesOnChunk)
    {
        foreach (GameObject obstaclePos in randomObstaclesOnChunk)
        {
            PickTypeOfObstacle(obstaclePos);
        }
    }

    private void PickTypeOfObstacle(GameObject obstaclePos)
    {
        int index = Random.Range(ZeroPercent, HundredPercent);
        int[] obstaclesChances ={
                    ChanceForRoadblock,
                    ChanceForDoubleRight,
                    ChanceForDoubleLeft,
                    ChanceForDoubleMid,
                    ChanceForGroundEnemy,
                    ChanceForFlyingEnemy
                };

        System.Action[] obstacleSetPrefabFunctions ={
                    ObstaclesPoolingSystem.Instance.SetRoadblockAsBasePrefab,
                    ObstaclesPoolingSystem.Instance.SetDoubleRightAsBasePrefab,
                    ObstaclesPoolingSystem.Instance.SetDoubleLeftAsBasePrefab,
                    ObstaclesPoolingSystem.Instance.SetLeftAndRightAsBasePrefab,
                    EnemyPoolingSystem.Instance.SetGroundEnemyAsBasePrefab,
                    EnemyPoolingSystem.Instance.SetFlyingEnemyAsBasePrefab
                };

        bool isEnemy = false;

        for (int i = 0; i < obstaclesChances.Length; i++)
        {
            if (i > 3)
            {
                isEnemy = true;
            }

            if (index <= obstaclesChances[i])
            {
                obstacleSetPrefabFunctions[i].Invoke();
                break;
            }
        }

        if (isEnemy)
        {
            EnemyController enemy = EnemyPoolingSystem.Instance.GetObjectFromPool();
            spawnedEnemy = true;
            SetPositionAndParentOfObjectFromPool(enemy.gameObject, obstaclePos.transform.position);
        }
        else
        {
            EnvironmentMovementController obstacle = ObstaclesPoolingSystem.Instance.GetObjectFromPool();
            SetPositionAndParentOfObjectFromPool(obstacle.gameObject, obstaclePos.transform.position);
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
        {
            PickTypeOfCollectable(collectablePos);
        }
    }

    private void PickTypeOfCollectable(GameObject collectablePos)
    {
        int index = Random.Range(ZeroPercent, HundredPercent);
        int[] collectableChances ={
                ChanceForSpaceship,
                ChanceForJet
            };

        System.Action<Vector3>[] collectableSpawnFunctions ={
                SpawnSpaceship,
                SpawnJet
            };

        if (spawnedEnemy == true)
        {
            SpawnGun(collectablePos.transform.position);
            spawnedEnemy = false;
        }
        else
        {
            for (int i = 0; i < collectableChances.Length; i++)
            {
                if (index <= collectableChances[i])
                {
                    collectableSpawnFunctions[i].Invoke(collectablePos.transform.position);
                    break;
                }
            }
        }
    }

    private void SpawnGun(Vector3 positionToSpawn)
    {
        GunController gun = GunPoolingSystem.Instance.GetObjectFromPool();
        SetPositionAndParentOfObjectFromPool(gun.gameObject, positionToSpawn);
    }

    private void SpawnJet(Vector3 positionToSpawn)
    {
        JetController jet = JetPoolingSystem.Instance.GetObjectFromPool();
        SetPositionAndParentOfObjectFromPool(jet.gameObject, positionToSpawn);
    }

    private void SpawnSpaceship(Vector3 positionToSpawn)
    {
        CollectableController spaceship = SpaceshipPoolingSystem.Instance.GetObjectFromPool();
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
