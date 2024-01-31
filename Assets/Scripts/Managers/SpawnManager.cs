using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private const int numberOfCollectablesPerChunk = 3;
    private const int chanceForRegularChunk = 30;
    private const int chanceForTwoEnemiesChunk = 60;
    private const int chanceForFlyingEnemyChunk = 90;
    private const int chanceForSpaceship = 85;
    private const int chanceForJet = 100;
    private const int chanceForRoadblock = 20;
    private const int chanceForDoubleRight = 36;
    private const int chanceForDoubleLeft = 52;
    private const int chanceForDoubleMid = 68;
    private const int chanceForGroundEnemy = 84;
    private const int chanceForFlyingEnemy = 100;
    private const int hundredPercent = 101;
    private const int zeroPercent = 0;
    private const float posZ = 50f;
    private float chunkSpawnDelay = 5.7f;
    private bool canSpawn;
    private float spacingBetweenObstacles = 10f;
    private bool spawnedEnemy;
    private Vector3 endOfPreviousChunk;
    private ChunkController chunk;

    private void OnEnable()
    {
        StartCoroutine(SpawnObstacle());
        endOfPreviousChunk = Vector3.zero;
    }

    public IEnumerator SpawnObstacle()
    {
        while (canSpawn)
        {
            SpawnChunk();
            yield return new WaitForSeconds(chunkSpawnDelay);
        }
    }


    private void SpawnChunk()
    {
        if (chunk != null)
        {
            endOfPreviousChunk = chunk.GetEndOfChunk();
        }

        int indexForChunk = Random.Range(zeroPercent, hundredPercent);

        // if (indexForChunk <= chanceForRegularChunk)
        // {
        //     ChunkPoolingSystem.Instance.SetCompleteChunkAsBase();
        // }
        // else if (chanceForRegularChunk < indexForChunk && indexForChunk <= chanceForTwoEnemiesChunk)
        // {
        //     ChunkPoolingSystem.Instance.SetChunkWithTwoEnemiesAsBase();
        // }
        // else if (chanceForTwoEnemiesChunk < indexForChunk && indexForChunk <= chanceForFlyingEnemyChunk)
        // {
        //     ChunkPoolingSystem.Instance.SetChunkWithFlyingEnemyAsBase();
        // }
        // else
        // {
        ChunkPoolingSystem.Instance.SetChunkWithRandomObstaclesAsBase();
        //}

        chunk = ChunkPoolingSystem.Instance.GetObjectFromPool();

        List<GameObject> randomObstaclesOnChunk = chunk.GetPositionsForRandomObstaclesOnChunk();
        List<GameObject> randomCollectablesOnChunk = chunk.GetPositionsForRandomCollectablesOnChunk();

        if (randomObstaclesOnChunk.Count != 0 && randomCollectablesOnChunk.Count != 0)
        {
            spawnedEnemy = false;
            HandleObstacleSpawning(randomObstaclesOnChunk, chunk);
            HandleCollectableSpawning(randomCollectablesOnChunk, chunk);

        }
        if (endOfPreviousChunk == Vector3.zero)
            chunk.transform.position = new Vector3(chunk.transform.position.x, chunk.transform.position.y, posZ);
        else
            chunk.transform.position = new Vector3(chunk.transform.position.x, chunk.transform.position.y, endOfPreviousChunk.z + 15f);
    }

    //In inspector, always set position for gun spawning first in list
    private void HandleCollectableSpawning(List<GameObject> randomCollectablesOnChunk, ChunkController chunk)
    {
        foreach (GameObject collectablePos in randomCollectablesOnChunk)
        {
            int index = Random.Range(zeroPercent, hundredPercent);
            int[] collectableChances ={
                chanceForSpaceship,
                chanceForJet
            };

            System.Action<Vector3, ChunkController>[] collectableSpawnFunctions ={
                SpawnSpaceship,
                SpawnJet
            };

            if (spawnedEnemy == true)
            {
                SpawnGun(collectablePos.transform.position, chunk);
                spawnedEnemy = false;
            }
            else
            {
                for (int i = 0; i < collectableChances.Length; i++)
                {
                    if (index <= collectableChances[i])
                    {
                        collectableSpawnFunctions[i].Invoke(collectablePos.transform.position, chunk);
                    }
                }
            }
        }
    }

    private void HandleObstacleSpawning(List<GameObject> randomObstaclesOnChunk, ChunkController chunk)
    {
        foreach (GameObject obstaclePos in randomObstaclesOnChunk)
        {
            int index = Random.Range(zeroPercent, hundredPercent);
            int[] obstaclesChances ={
                    chanceForRoadblock,
                    chanceForDoubleRight,
                    chanceForDoubleLeft,
                    chanceForDoubleMid,
                    chanceForGroundEnemy,
                    chanceForFlyingEnemy
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
                enemy.transform.position = obstaclePos.transform.position;
                spawnedEnemy = true;
                enemy.transform.SetParent(chunk.transform);
                chunk.AddObjectToList(enemy.gameObject);
            }
            else
            {
                EnvironmentMovementController obstacle = ObstaclesPoolingSystem.Instance.GetObjectFromPool();
                obstacle.transform.position = obstaclePos.transform.position;
                obstacle.transform.SetParent(chunk.transform);
                chunk.AddObjectToList(obstacle.gameObject);
            }
        }
    }

    private void SpawnGun(Vector3 positionToSpawn, ChunkController chunk)
    {
        GunController gun = GunPoolingSystem.Instance.GetObjectFromPool();
        gun.transform.position = positionToSpawn;
        gun.transform.SetParent(chunk.transform);
        chunk.AddObjectToList(gun.gameObject);
    }

    private void SpawnJet(Vector3 positionToSpawn, ChunkController chunk)
    {
        JetController jet = JetPoolingSystem.Instance.GetObjectFromPool();
        jet.transform.position = positionToSpawn;
        jet.transform.SetParent(chunk.transform);
        chunk.AddObjectToList(jet.gameObject);
    }

    private void SpawnSpaceship(Vector3 positionToSpawn, ChunkController chunk)
    {
        CollectableController spaceship = SpaceshipPoolingSystem.Instance.GetObjectFromPool();
        spaceship.transform.position = positionToSpawn;
        spaceship.transform.SetParent(chunk.transform);
        chunk.AddObjectToList(spaceship.gameObject);
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
        StopCoroutine(SpawnObstacle());
    }

    public float GetChunkSpawnDelay() => chunkSpawnDelay;

    public void SetChunkSpawnDelay(float spawnDelayInSeconds)
    {
        chunkSpawnDelay = spawnDelayInSeconds;
    }

    public void SetSpacingBetweenObstacles(float space)
    {
        spacingBetweenObstacles = space;
    }

    public float GetSpacingBetweenObstacles() => spacingBetweenObstacles;
}
