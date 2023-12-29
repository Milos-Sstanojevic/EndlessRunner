using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private const int OffsetZ = 4;
    private const float edgePosX = 4f;
    private const float posY = 4f;
    private const float posZ = 35f;
    public const float ObstacleSpawnDelay = 1;
    public const float SpaceshipSpawnDelay = 2;
    [SerializeField] private PoolingSystem _pool;
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
            yield return new WaitForSeconds(ObstacleSpawnDelay);
            HandleObstacleSpawning();
        }
    }

    public IEnumerator SpawnSpaceship()
    {
        while (canSpawn)
        {
            yield return new WaitForSeconds(SpaceshipSpawnDelay);
            HandleSpaceshipSpawning();
        }
    }

    private void HandleObstacleSpawning()
    {
        ObstacleController obstacle = _pool.GetObstacleFromPool();

        float randomXObst = Random.Range(-edgePosX, edgePosX);

        obstacle.transform.position = new Vector3(randomXObst, obstacle.transform.position.y, posZ);
    }


    private void HandleSpaceshipSpawning()
    {
        SpaceshipController spaceship = _pool.GetSpaceshipFromPool();

        float randomXShip = Random.Range(-edgePosX, edgePosX);
        float randomY = Random.Range(1f, posY);

        spaceship.transform.position = new Vector3(randomXShip, randomY, posZ + OffsetZ);
    }


    public void EnableSpawning() => canSpawn = true;
    public void DisableSpawning() => canSpawn = false;
}
