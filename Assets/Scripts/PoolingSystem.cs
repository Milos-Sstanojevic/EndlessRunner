using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class PoolingSystem : MonoBehaviour
{
    private const int chanceForRoadblock = 25;
    private const int chanceForDoubleRight = 50;
    private const int chanceForDoubleLeft = 75;
    private const int chanceForDoubleMid = 90;
    private const int hundredPercent = 101;
    private const int zeroPercent = 0;
    private const int roadblockIndex = 0;
    private const int doubleObstacleMiddleIndex = 1;
    private const int doubleObstacleRightIndex = 2;
    private const int enemyIndex = 3;
    private const int doubleObstacleLeftIndex = 4;
    private const int defaultCapacityForPool = 40;
    private const int maximumCapacityForPool = 100;
    [SerializeField] private Transform parentOfPool;
    [SerializeField] private ObjectManager[] obstacle;
    [SerializeField] private SpaceshipController spaceship;
    private ObjectPool<ObjectManager> _poolObstacle;
    private ObjectPool<SpaceshipController> _poolSpaceship;
    private List<SpaceshipController> instantiatedSpaceships;
    private List<ObjectManager> instantiatedObstacles;


    private void Start()
    {
        instantiatedSpaceships = new List<SpaceshipController>();
        instantiatedObstacles = new List<ObjectManager>();

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

    private List<ObjectManager> CreateObstaclePool()
    {
        _poolObstacle = new ObjectPool<ObjectManager>(CreateObstacle, obstacle =>
        {
            obstacle.gameObject.SetActive(true);
        }, obstacle =>
        {
            obstacle.gameObject.SetActive(false);
        }, obstacle =>
        {
            UnsubscribeFromDestroyAction();
            Destroy(obstacle.gameObject);
        }, true, defaultCapacityForPool, maximumCapacityForPool);

        return instantiatedObstacles;
    }

    private ObjectManager CreateObstacle()
    {
        int index = GenerateIndexWithProbability();
        ObjectManager obs = Instantiate(obstacle[index]);
        obs.transform.SetParent(parentOfPool);
        instantiatedObstacles.Add(obs);

        return obs;
    }

    private int GenerateIndexWithProbability()
    {
        //ovo mi se ne svidja daj nekako dinamicnije da bude
        //smisli kako ces sansu da se stvori enemy

        int index = Random.Range(zeroPercent, hundredPercent);

        if (index <= chanceForRoadblock)
            return roadblockIndex;

        else if (index >= chanceForRoadblock && index < chanceForDoubleRight)
            return doubleObstacleRightIndex;

        else if (index >= chanceForDoubleRight && index < chanceForDoubleLeft)
            return doubleObstacleLeftIndex;

        else if (index >= chanceForDoubleLeft && index < chanceForDoubleMid)
            return doubleObstacleMiddleIndex;

        else
            return enemyIndex;
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
        }, true, defaultCapacityForPool, maximumCapacityForPool);

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

    public ObjectManager GetObstacleFromPool()
    {
        return _poolObstacle.Get();
    }

    public SpaceshipController GetSpaceshipFromPool()
    {
        return _poolSpaceship.Get();
    }

    private void DestroyObject(IDestroyable destroyable)
    {
        if (destroyable is ObjectManager movable)
        {
            if (movable.gameObject.CompareTag("Obstacle"))
            {
                _poolObstacle.Release(movable);
            }
            if (movable.gameObject.CompareTag("Spaceship"))
            {
                _poolSpaceship.Release((SpaceshipController)movable);
            }
            else
            {
                Debug.Log("It just stage for now");
            }
        }
        else
        {
            Debug.LogError("Invalid destryable type");
        }
    }

    public List<SpaceshipController> GetInstanciatedSpaceships()
    {
        return instantiatedSpaceships;
    }

    public List<ObjectManager> GetInstanciatedObstacles()
    {
        return instantiatedObstacles;
    }
}
