using System.Collections;
using System.Collections.Generic;
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

    private List<SpaceshipController> instantiatedSpaceships;
    private List<ObstacleController> instantiatedObstacles;


    private void Start()
    {
        instantiatedSpaceships = new List<SpaceshipController>();
        instantiatedObstacles = new List<ObstacleController>();

        instantiatedObstacles = CreateObstaclePool();
        instantiatedSpaceships = CreateSpaceshipPool();

        SubscribeToDestroyObstacleAction();
    }

    private void SubscribeToDestroyObstacleAction()
    {
        ObstacleController.OnDestroyObstacle += DestroyObstacle;
    }

    private List<ObstacleController> CreateObstaclePool()
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

        return instantiatedObstacles;
    }

    private ObstacleController CreateObstacle()
    {
        ObstacleController obs = Instantiate(obstacle[0]);
        instantiatedObstacles.Add(obs);

        return obs;
    }

    private List<SpaceshipController> CreateSpaceshipPool()
    {
        _poolSpaceship = new ObjectPool<SpaceshipController>(CreateSpaceship, spaceship =>
        {
            spaceship.gameObject.SetActive(true);
        }, spaceship =>
        {
            spaceship.gameObject.SetActive(false);
        }, spaceship =>
        {
            UnubscribeFromDestroySpaceshipAction(spaceship);
            Destroy(spaceship.gameObject);
        }, true, DefaultCapacityForPool, MaximumCapacityForPool);

        return instantiatedSpaceships;
    }

    private void UnubscribeFromDestroySpaceshipAction(SpaceshipController spaceship)
    {
        spaceship.OnDestroySpaceship -= DestroySpaceship;
    }

    private SpaceshipController CreateSpaceship()
    {
        SpaceshipController ship = Instantiate(spaceship);
        SubscribeToDestroySpaceshipAction(ship);

        instantiatedSpaceships.Add(ship);

        return ship;
    }

    private void SubscribeToDestroySpaceshipAction(SpaceshipController ship)
    {
        ship.OnDestroySpaceship += DestroySpaceship;
    }

    private void OnDestroy()
    {
        UnsubscribeFromDestroyObstacleAction();
    }

    private void UnsubscribeFromDestroyObstacleAction()
    {
        ObstacleController.OnDestroyObstacle -= DestroyObstacle;

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


    public List<SpaceshipController> GetInstanciatedSpaceships()
    {
        return instantiatedSpaceships;
    }

    public List<ObstacleController> GetInstanciatedObstacles()
    {
        return instantiatedObstacles;
    }
}
