using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class PoolingSystem : MonoBehaviour
{
    private const int DefaultCapacityForPool = 10;
    private const int MaximumCapacityForPool = 20;
    private const int OffsetZ = 4;

    [SerializeField] private ObstacleController[] obstacle;
    [SerializeField] private SpaceshipController spaceship;
    private ObjectPool<ObstacleController> _poolObstacle;
    private ObjectPool<SpaceshipController> _poolSpaceship;
    public float ObstacleSpawnDelay = 1;
    public float SpaceshipSpawnDelay = 2;
    private float edgePosX = 4f;
    private float posY = 4f;
    private float posZ = 23f;

    void Start()
    {

        CreateObstaclePool();
        CreateSpaceshipPool();

        StartCoroutine(SpawnObstacle());
        StartCoroutine(SpawnSpaceship());

        SubscribeToDestroyActions();
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
        }, true, DefaultCapacityForPool, MaximumCapacityForPool);
    }

    ObstacleController CreateObstacle()
    {
        return Instantiate(obstacle[0]);        //priprema za kad bude bilo vise prepreka
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
        }, true, DefaultCapacityForPool, MaximumCapacityForPool);
    }


    SpaceshipController CreateSpaceship()
    {
        return Instantiate(spaceship);
    }

    void SubscribeToDestroyActions()
    {
        ObstacleController.OnDestroyObstacle += DestroyObstacle;
        SpaceshipController.OnDestroySpaceship += DestroySpaceship;
    }

    private void OnDestroy()
    {
        UnsubscribeFromDestroyAction();
    }

    void UnsubscribeFromDestroyAction()
    {
        ObstacleController.OnDestroyObstacle -= DestroyObstacle;
        SpaceshipController.OnDestroySpaceship -= DestroySpaceship;
    }


    void Update()
    {
    }

    public IEnumerator SpawnObstacle()
    {
        while (GameManager.Instance.IsGameActive)
        {
            yield return new WaitForSeconds(ObstacleSpawnDelay);

            HandleObstacleSpawning();
        }
    }

    public IEnumerator SpawnSpaceship()
    {
        while (GameManager.Instance.IsGameActive)
        {
            yield return new WaitForSeconds(SpaceshipSpawnDelay);

            HandleSpaceshipSpawning();
        }
    }

    private void HandleObstacleSpawning()
    {
        var obstacle = _poolObstacle.Get();
        float randomXObst = Random.Range(-edgePosX, edgePosX);

        obstacle.transform.position = new Vector3(randomXObst, obstacle.transform.position.y, posZ);
    }


    private void HandleSpaceshipSpawning()
    {
        var spaceship = _poolSpaceship.Get();

        float randomXShip = Random.Range(-edgePosX, edgePosX);
        float randomY = Random.Range(1f, posY);

        spaceship.transform.position = new Vector3(randomXShip, randomY, posZ + OffsetZ);
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
