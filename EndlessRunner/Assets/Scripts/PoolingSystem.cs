using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class PoolingSystem : MonoBehaviour
{
    private const int DefaultCapacityForPool = 10;
    private const int MaximumCapacityForPool = 20;

    [SerializeField] private ObstacleController[] obstacle;
    [SerializeField] private SpaceshipController spaceship;
    private ObjectPool<ObstacleController> _poolObstacle;
    private ObjectPool<SpaceshipController> _poolSpaceship;


    private void Start()
    {

        CreateObstaclePool();
        CreateSpaceshipPool();

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

    private ObstacleController CreateObstacle()
    {
        return Instantiate(obstacle[0]);
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


    private SpaceshipController CreateSpaceship()
    {
        return Instantiate(spaceship);
    }

    private void SubscribeToDestroyActions()
    {
        ObstacleController.OnDestroyObstacle += DestroyObstacle;
        SpaceshipController.OnDestroySpaceship += DestroySpaceship;
    }

    private void OnDestroy()
    {
        UnsubscribeFromDestroyAction();
    }

    private void UnsubscribeFromDestroyAction()
    {
        ObstacleController.OnDestroyObstacle -= DestroyObstacle;
        SpaceshipController.OnDestroySpaceship -= DestroySpaceship;
    }

    public ObstacleController GetObstacleFromPool()
    {
        return _poolObstacle.Get();
    }

    public SpaceshipController GetSpaceshipFromPool()
    {
        return _poolSpaceship.Get();
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
