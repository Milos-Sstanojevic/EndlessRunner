using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private const int numberOfEnemiesPerChunk = 5;
    private const int numberOfCollectablesPerChunk = 3;
    private const int chanceForSpaceship = 85;
    private const int chanceForGroundEnemy = 50;
    private const int chanceForRoadblock = 30;
    private const int chanceForDoubleRight = 60;
    private const int chanceForDoubleLeft = 90;
    private const int chanceForObstacle = 75;
    private const int hundredPercent = 101;
    private const int zeroPercent = 0;
    private const float offsetZ = 5f;
    private const float posY = 4f;
    private const float posZ = 35f;
    private const float gunSpawnOffset = 4f;
    private float chunkSpawnDelay = 6f;
    private bool canSpawn;
    private float spacingBetweenObstacles = 10f;
    private bool spawnedEnemy;

    private void OnEnable()
    {
        StartCoroutine(SpawnObstacle());
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
        HandleObstacleSpawning();
        HandleCollectableSpawning();
    }

    private void HandleObstacleSpawning()
    {
        float zOffsetFromEachOther = 0f;
        for (int i = 0; i < numberOfEnemiesPerChunk; i++)
        {
            int indexForEnemyOrObstacle = Random.Range(zeroPercent, hundredPercent);

            if (indexForEnemyOrObstacle <= chanceForObstacle)
            {
                int indexForObstacle = GenerateObstacleIndexWithProbability();
                SpawnPickedObstacle(indexForObstacle, zOffsetFromEachOther);
            }
            else
            {
                int indexForEnemy = GenerateEnemyIndexWithProbability();
                SpawnPickedEnemy(indexForEnemy, zOffsetFromEachOther);
                spawnedEnemy = true;
            }
            zOffsetFromEachOther += spacingBetweenObstacles;
        }
    }

    private void HandleCollectableSpawning()
    {
        float zOffsetFromEachOther = 0f;
        float lastCollectableSpawnOffset = 0f;

        for (int i = 0; i < numberOfCollectablesPerChunk; i++)
        {
            float randomX = Random.Range(-GlobalConstants.EdgePosX, GlobalConstants.EdgePosX);
            float randomY = Random.Range(1f, posY);

            if (spawnedEnemy && i == 0)
            {
                SpawnGun(randomX, randomY, zOffsetFromEachOther + lastCollectableSpawnOffset - gunSpawnOffset);
                spawnedEnemy = false;
            }
            else
            {
                int indexForSpaceship = Random.Range(zeroPercent, hundredPercent);
                if (indexForSpaceship < chanceForSpaceship)
                {
                    SpawnSpaceship(randomX, randomY, zOffsetFromEachOther);
                }
                else
                {
                    SpawnJet(randomX, randomY, zOffsetFromEachOther);
                }
            }

            zOffsetFromEachOther += spacingBetweenObstacles;
            lastCollectableSpawnOffset = zOffsetFromEachOther;
        }
    }

    //nisam siguran da li da napravim kao Template funkciju dole
    private void SpawnGun(float randomX, float randomY, float zOffsetFromEachOther)
    {
        GunController gun = GunPoolingSystem.Instance.GetObjectFromPool();
        gun.transform.position = new Vector3(randomX, randomY, posZ + zOffsetFromEachOther);
    }

    private void SpawnJet(float randomX, float randomY, float zOffsetFromEachOther)
    {
        JetController jet = JetPoolingSystem.Instance.GetObjectFromPool();
        jet.transform.position = new Vector3(randomX, randomY, posZ + offsetZ + zOffsetFromEachOther);
    }

    private void SpawnSpaceship(float randomX, float randomY, float zOffsetFromEachOther)
    {
        CollectableController spaceship = SpaceshipPoolingSystem.Instance.GetObjectFromPool();
        spaceship.transform.position = new Vector3(randomX, randomY, posZ + offsetZ + zOffsetFromEachOther);
    }

    private int GenerateEnemyIndexWithProbability()
    {
        int indexForEnemy = Random.Range(zeroPercent, hundredPercent);
        int[] enemyChances ={
            chanceForGroundEnemy
        };

        for (int i = 0; i < enemyChances.Length; i++)
        {
            if (indexForEnemy < enemyChances[i])
                return i;
        }

        return enemyChances.Length;
    }

    private int GenerateObstacleIndexWithProbability()
    {
        int indexForObstacle = Random.Range(zeroPercent, hundredPercent);
        int[] obstaclesChances ={
            chanceForRoadblock,
            chanceForDoubleRight,
            chanceForDoubleLeft
        };

        for (int i = 0; i < obstaclesChances.Length; i++)
        {
            if (indexForObstacle < obstaclesChances[i])
                return i;
        }

        return obstaclesChances.Length;
    }

    private void SpawnPickedEnemy(int index, float zOffset)
    {
        System.Action<float>[] enemySpawnFunctions ={
            SpawnGroundEnemy
        };

        if (index == enemySpawnFunctions.Length)
        {
            float randomX = Random.Range(-GlobalConstants.EdgePosX, GlobalConstants.EdgePosX);
            SpawnFlyingEnemy(randomX, zOffset);
        }
        else
        {
            enemySpawnFunctions[index].Invoke(zOffset);
        }
    }

    private void SpawnPickedObstacle(int index, float zOffset)
    {
        System.Action<float>[] obstacleSpawnFunctions ={
            SpawnRoadblock,
            SpawnDoubleLeft,
            SpawnDoubleRight
        };

        if (index == obstacleSpawnFunctions.Length)
        {
            SpawnLeftAndRight(zOffset);
        }
        else
        {
            obstacleSpawnFunctions[index].Invoke(zOffset);
        }
    }

    private void SpawnGroundEnemy(float zOffset)
    {
        EnemyPoolingSystem.Instance.SetGroundEnemyAsBasePrefab();
        EnemyController groundEnemy = EnemyPoolingSystem.Instance.GetObjectFromPool();

        groundEnemy.transform.position = new Vector3(groundEnemy.transform.position.x, groundEnemy.transform.position.y, posZ + zOffset);
    }

    private void SpawnFlyingEnemy(float randomX, float zOffset)
    {
        EnemyPoolingSystem.Instance.SetFlyingEnemyAsBasePrefab();
        EnemyController flyingEnemy = EnemyPoolingSystem.Instance.GetObjectFromPool();

        flyingEnemy.transform.position = new Vector3(randomX, flyingEnemy.transform.position.y, posZ + zOffset);
    }

    private void SpawnRoadblock(float zOffset)
    {
        SpawnObstacle<EnvironmentMovementController>(
            () => ObstaclesPoolingSystem.Instance.SetRoadblockAsBasePrefab(),
            () => ObstaclesPoolingSystem.Instance.GetObjectFromPool(),
            zOffset
        );
    }

    private void SpawnDoubleLeft(float zOffset)
    {
        SpawnObstacle<EnvironmentMovementController>(
            () => ObstaclesPoolingSystem.Instance.SetDoubleLeftAsBasePrefab(),
            () => ObstaclesPoolingSystem.Instance.GetObjectFromPool(),
            zOffset
        );
    }

    private void SpawnDoubleRight(float zOffset)
    {
        SpawnObstacle<EnvironmentMovementController>(
            () => ObstaclesPoolingSystem.Instance.SetDoubleRightAsBasePrefab(),
            () => ObstaclesPoolingSystem.Instance.GetObjectFromPool(),
            zOffset
        );
    }

    private void SpawnLeftAndRight(float zOffset)
    {
        SpawnObstacle<EnvironmentMovementController>(
            () => ObstaclesPoolingSystem.Instance.SetLeftAndRightAsBasePrefab(),
            () => ObstaclesPoolingSystem.Instance.GetObjectFromPool(),
            zOffset
        );
    }

    private void SpawnObstacle<T>(System.Action setPrefabAction, System.Func<EnvironmentMovementController> getObjectAction, float zOffset) where T : EnvironmentMovementController
    {
        setPrefabAction.Invoke();
        EnvironmentMovementController obstacle = getObjectAction.Invoke();

        obstacle.transform.position = new Vector3(obstacle.transform.position.x, obstacle.transform.position.y, posZ + zOffset);
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
