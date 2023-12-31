using System.Collections;
using UnityEngine;
using static GlobalConstants;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private PoolingSystem _pool;
    private const int OffsetZ = 4;
    private const float edgePosX = 4f;
    private const float posY = 4f;
    private const float posZ = 35f;
    private float obstacleSpawnDelay = 1f;
    private float spaceshipSpawnDelay = 2f;
    private bool canSpawn;

    public void EnableSpawning() => canSpawn = true;
    public void DisableSpawning() => canSpawn = false;
    public float GetObstacleSpawnDelay() => obstacleSpawnDelay;
    public float GetSpaceshipSpawnDelay() => spaceshipSpawnDelay;
    public void SetObstacleSpawnDelay(float spawnDelayInSeconds) => obstacleSpawnDelay = spawnDelayInSeconds;
    public void SetSpaceshipSpawnDelay(float spawnDelayInSeconds) => spaceshipSpawnDelay = spawnDelayInSeconds;

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

    private void HandleObstacleSpawning()
    {
        ObstacleController obstacle = _pool.GetObstacleFromPool();

        if (obstacle.name == "DoubleObstacle(Clone)")
        {
            obstacle.transform.position = new Vector3(obstacle.transform.position.x, obstacle.transform.position.y, posZ);
        }
        else
        {
            float randomXObst = Random.Range(-edgePosX, edgePosX);

            obstacle.transform.position = new Vector3(randomXObst, obstacle.transform.position.y, posZ);
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

    private void HandleSpaceshipSpawning()
    {
        SpaceshipController spaceship = _pool.GetSpaceshipFromPool();

        float randomXShip = Random.Range(-edgePosX, edgePosX);
        float randomY = Random.Range(1f, posY);

        spaceship.transform.position = new Vector3(randomXShip, randomY, posZ + OffsetZ);
    }
}
