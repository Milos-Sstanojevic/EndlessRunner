using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private const string flagNameForDoubleObstacle = "DoubleObstacle";
    private const int offsetZ = 3;
    private const float posY = 4f;
    private const float posZ = 35f;
    [SerializeField] private PoolingSystem _pool;
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
            HandleSpaceshipSpawning();
        }
    }

    private void HandleObstacleSpawning()
    {
        ObjectManager obstacle = _pool.GetObstacleFromPool();

        if (obstacle.name.StartsWith(flagNameForDoubleObstacle))
        {
            obstacle.transform.position = new Vector3(obstacle.transform.position.x, obstacle.transform.position.y, posZ);
        }
        else
        {
            float randomXObst = Random.Range(-GlobalConstants.EdgePosX, GlobalConstants.EdgePosX);
            obstacle.transform.position = new Vector3(randomXObst, obstacle.transform.position.y, posZ);
        }
    }

    private void HandleSpaceshipSpawning()
    {
        SpaceshipController spaceship = _pool.GetSpaceshipFromPool();

        float randomXShip = Random.Range(-GlobalConstants.EdgePosX, GlobalConstants.EdgePosX);
        float randomY = Random.Range(1f, posY);

        spaceship.transform.position = new Vector3(randomXShip, randomY, posZ + offsetZ);
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
