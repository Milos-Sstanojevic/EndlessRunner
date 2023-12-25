using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolingSystem : MonoBehaviour
{
    public ObstacleController obstacle;
    public SpaceshipController spaceship;
    private ObjectPool<ObstacleController> _poolObstacle;
    private ObjectPool<SpaceshipController> _poolSpaceship;
    public float obstacleSpawnDelay = 1;
    public float spaceshipSpawnDelay = 2;
    private float posX = 4f;
    private float posY = 3f;
    private float posZ = 23f;

    // Start is called before the first frame update
    void Start()
    {

        CreateObstaclePool();
        CreateSpaceshipPool();


        StartCoroutine(SpawnObstacle());
        StartCoroutine(SpawnSpaceship());
    }


    void Update()
    {
    }

    public IEnumerator SpawnObstacle()
    {
        while (GameManager.Instance.IsGameActive)
        {
            yield return new WaitForSeconds(obstacleSpawnDelay);

            HandleObstacleSpawning();
        }
    }

    public IEnumerator SpawnSpaceship()
    {
        while (GameManager.Instance.IsGameActive)
        {
            yield return new WaitForSeconds(spaceshipSpawnDelay);

            HandleSpaceshipSpawning();
        }
    }


    private void HandleObstacleSpawning()
    {
        var obstacle = _poolObstacle.Get();
        float randomXObst = Random.Range(-posX, posX);

        obstacle.transform.position = new Vector3(randomXObst, 0, posZ);
        obstacle.Init(DestroyObstacle);
    }


    private void HandleSpaceshipSpawning()
    {

        var spaceship = _poolSpaceship.Get();

        float randomXShip = Random.Range(-posX, posX);
        float randomY = Random.Range(1f, posY);

        spaceship.transform.position = new Vector3(randomXShip, randomY, posZ + 4);
        spaceship.Init(DestroySpaceship);
    }


    private void CreateObstaclePool()
    {
        _poolObstacle = new ObjectPool<ObstacleController>(CreateObstacle, obstacle =>
        {
            obstacle.gameObject.SetActive(true);

        }, obstacle =>
        {
            obstacle.gameObject.SetActive(false);
        }, obstacle =>
        {
            Destroy(obstacle.gameObject);
        }, true, 10, 20);

    }

    private void CreateSpaceshipPool()
    {
        _poolSpaceship = new ObjectPool<SpaceshipController>(CreateSpaceship, spaceship =>
        {
            spaceship.gameObject.SetActive(true);

        }, spaceship =>
        {
            spaceship.gameObject.SetActive(false);
        }, spaceship =>
        {
            Destroy(spaceship.gameObject);
        }, true, 10, 20);
    }

    ObstacleController CreateObstacle()
    {
        return Instantiate(obstacle);
    }

    SpaceshipController CreateSpaceship()
    {
        return Instantiate(spaceship);
    }

    private void DestroyObstacle(ObstacleController obstacle)
    {
        _poolObstacle.Release(obstacle);
    }

    private void DestroySpaceship(SpaceshipController spaceship)
    {
        _poolSpaceship.Release(spaceship);
    }
}
