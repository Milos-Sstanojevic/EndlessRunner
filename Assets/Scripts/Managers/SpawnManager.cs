using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private const string flagNameForDoubleObstacle = "DoubleObstacle";
    private const int offsetZ = 3;
    private const float posY = 4f;
    private const float posZ = 35f;
    private float obstacleSpawnDelay = 1f;
    private float spaceshipSpawnDelay = 2f;
    private bool canSpawn;

    private void OnEnable()
    {
        StartCoroutine(SpawnObstacle());
        StartCoroutine(SpawnSpaceship());
    }

    private void OnDisable()
    {
        StopCoroutine(SpawnObstacle());
        StopCoroutine(SpawnSpaceship());
    }

    public IEnumerator SpawnObstacle()
    {
        while (canSpawn)
        {
            yield return new WaitForSeconds(obstacleSpawnDelay);
            HandleObstacleSpawning();
        }
    }

    public IEnumerator SpawnSpaceship()
    {
        while (canSpawn)
        {
            yield return new WaitForSeconds(spaceshipSpawnDelay);
            HandleCollectableSpawning();
        }
    }


    private void HandleObstacleSpawning()
    {
        EnemyPoolingSystem.Instance.SetFlyingEnemyAsBasePrefab();
        EnemyController flyingEnemy = EnemyPoolingSystem.Instance.GetObjectFromPool();

        EnemyPoolingSystem.Instance.SetGroundEnemyAsBasePrefab();
        EnemyController groundEnemy = EnemyPoolingSystem.Instance.GetObjectFromPool();

        flyingEnemy.transform.position = new Vector3(flyingEnemy.transform.position.x, flyingEnemy.transform.position.y, posZ);

        groundEnemy.transform.position = new Vector3(groundEnemy.transform.position.x, groundEnemy.transform.position.y, posZ);

    }

    private void HandleCollectableSpawning()
    {
        // CollectableController spaceship = SpaceshipPoolingSystem.Instance.GetObjectFromPool();

        GunController gun = GunPoolingSystem.Instance.GetObjectFromPool();

        float randomXShip = Random.Range(-GlobalConstants.EdgePosX, GlobalConstants.EdgePosX);
        float randomY = Random.Range(1f, posY);

        gun.transform.position = new Vector3(randomXShip, randomY, posZ + offsetZ);
    }

    public void EnableSpawning()
    {
        canSpawn = true;
    }

    public void DisableSpawning()
    {
        canSpawn = false;
    }

    public float GetObstacleSpawnDelay() => obstacleSpawnDelay;

    public float GetSpaceshipSpawnDelay() => spaceshipSpawnDelay;

    public void SetObstacleSpawnDelay(float spawnDelayInSeconds)
    {
        obstacleSpawnDelay = spawnDelayInSeconds;
    }

    public void SetSpaceshipSpawnDelay(float spawnDelayInSeconds)
    {
        spaceshipSpawnDelay = spawnDelayInSeconds;
    }

}
