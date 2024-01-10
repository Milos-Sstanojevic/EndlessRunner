using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static GlobalConstants;

public class PoolingSystem : MonoBehaviour
{
    private const int ChanceForRoadblock = 80;
    private const int HundredPercent = 100;
    private const int ZeroPercent = 0;
    private const int RoadblockIndex = 0;
    private const int DoubleConeIndex = 1;
    private const int DefaultCapacityForPool = 10;
    private const int MaximumCapacityForPool = 20;

    [SerializeField] private Transform parentOfPool;
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
    }

    private void OnEnable()
    {
        SubscribeToDestroyAction();
    }

    private void SubscribeToDestroyAction()
    {
        EventManager.Instance.OnDestroyAction += DestroyObject;
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
            UnsubscribeFromDestroyAction();
            Destroy(obstacle.gameObject);
        }, true, DefaultCapacityForPool, MaximumCapacityForPool);

        return instantiatedObstacles;
    }

    private ObstacleController CreateObstacle()
    {
        int index = GenerateIndexWithProbability();
        ObstacleController obs = Instantiate(obstacle[index]);
        obs.transform.SetParent(parentOfPool);
        instantiatedObstacles.Add(obs);

        return obs;
    }

    private int GenerateIndexWithProbability()
    {
        int index = Random.Range(ZeroPercent, HundredPercent);
        if (index <= ChanceForRoadblock)
            return RoadblockIndex;
        else
            return DoubleConeIndex;
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
            UnsubscribeFromDestroyAction();
            Destroy(spaceship.gameObject);
        }, true, DefaultCapacityForPool, MaximumCapacityForPool);

        return instantiatedSpaceships;
    }

    private SpaceshipController CreateSpaceship()
    {
        SpaceshipController ship = Instantiate(spaceship);
        ship.transform.SetParent(parentOfPool);
        instantiatedSpaceships.Add(ship);
        return ship;
    }

    private void OnDisable()
    {
        UnsubscribeFromDestroyAction();
    }

    private void UnsubscribeFromDestroyAction()
    {
        EventManager.Instance.OnDestroyAction -= DestroyObject;
    }

    public ObstacleController GetObstacleFromPool()
    {
        return _poolObstacle.Get();
    }

    public SpaceshipController GetSpaceshipFromPool()
    {
        return _poolSpaceship.Get();
    }

    private void DestroyObject(IDestroyable destroyable)
    {
        if (destroyable is ObstacleController obstacle)
        {
            _poolObstacle.Release(obstacle);
        }
        else if (destroyable is SpaceshipController spaceship)
        {
            _poolSpaceship.Release(spaceship);
        }
        else
        {
            Debug.LogError("Invalid destroyable type");
        }
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
