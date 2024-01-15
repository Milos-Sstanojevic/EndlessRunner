using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class PoolingSystem : MonoBehaviour
{
    private const string obstacleTag = "Obstacle";
    private const string collectableTag = "Collectable";
    private const int chanceForRoadblock = 25;
    private const int chanceForDoubleRight = 50;
    private const int chanceForDoubleLeft = 75;
    private const int chanceForDoubleMid = 90;
    private const int chanceForSpaceship = 70;
    private const int chanceForGun = 85;
    private const int hundredPercent = 101;
    private const int zeroPercent = 0;
    private const int defaultCapacityForPool = 40;
    private const int maximumCapacityForPool = 100;
    [SerializeField] private Transform parentOfPool;
    [SerializeField] private EnviromentMovementBase[] obstacle;
    [SerializeField] private CollectableBase[] collectable;
    private ObjectPool<EnviromentMovementBase> _poolObstacle;
    private ObjectPool<CollectableBase> _poolCollectable;
    private List<CollectableBase> instantiatedCollectables;
    private List<EnviromentMovementBase> instantiatedObstacles;


    private void Start()
    {
        instantiatedCollectables = new List<CollectableBase>();
        instantiatedObstacles = new List<EnviromentMovementBase>();

        instantiatedObstacles = CreateObstaclePool();
        instantiatedCollectables = CreateCollectablePool();
    }

    private void OnEnable()
    {
        SubscribeToDestroyAction();
    }

    private void SubscribeToDestroyAction()
    {
        EventManager.Instance.OnDestroyAction += DestroyEnviroment;
    }

    private void DestroyEnviroment(EnviromentMovementBase movable)
    {


        if (movable.gameObject.CompareTag(obstacleTag))
        {
            _poolObstacle.Release(movable);
        }
        if (movable.gameObject.CompareTag(collectableTag))
        {
            movable.transform.SetParent(parentOfPool);
            _poolCollectable.Release((CollectableBase)movable);
        }
        else
        {
            Debug.Log("It just stage for now");
        }

    }

    private List<EnviromentMovementBase> CreateObstaclePool()
    {
        _poolObstacle = new ObjectPool<EnviromentMovementBase>(CreateObstacle, obstacle =>
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

    private EnviromentMovementBase CreateObstacle()
    {
        int index = GenerateObstacleIndexWithProbability();
        EnviromentMovementBase obs = Instantiate(obstacle[index]);
        obs.transform.SetParent(parentOfPool);
        instantiatedObstacles.Add(obs);

        return obs;
    }

    private int GenerateObstacleIndexWithProbability()
    {
        int index = Random.Range(zeroPercent, hundredPercent);
        int[] obstacleChances ={
            chanceForRoadblock,
            chanceForDoubleRight,
            chanceForDoubleLeft,
            chanceForDoubleMid
        };

        for (int i = 0; i < obstacleChances.Length; i++)
        {
            if (index < obstacleChances[i])
                return i;
        }

        return obstacleChances.Length;
    }

    private List<CollectableBase> CreateCollectablePool()
    {
        _poolCollectable = new ObjectPool<CollectableBase>(CreateCollectable, collectable =>
        {
            collectable.gameObject.SetActive(true);
        }, collectable =>
        {
            collectable.gameObject.SetActive(false);
        }, collectable =>
        {
            UnsubscribeFromDestroyAction();
            Destroy(collectable.gameObject);
        }, true, defaultCapacityForPool, maximumCapacityForPool);

        return instantiatedCollectables;
    }

    private CollectableBase CreateCollectable()
    {
        int index = GenerateCollectableIndexWithProbability();
        CollectableBase coll = Instantiate(collectable[1]);
        coll.transform.SetParent(parentOfPool);
        instantiatedCollectables.Add(coll);
        return coll;
    }

    private int GenerateCollectableIndexWithProbability()
    {
        int index = Random.Range(zeroPercent, hundredPercent);
        int[] collectableChances ={
            chanceForSpaceship,
            chanceForGun
        };

        for (int i = 0; i < collectableChances.Length; i++)
        {
            if (index < collectableChances[i])
                return i;
        }

        return collectableChances.Length;
    }

    private void OnDisable()
    {
        UnsubscribeFromDestroyAction();
    }

    private void UnsubscribeFromDestroyAction()
    {
        EventManager.Instance.OnDestroyAction -= DestroyEnviroment;
    }

    public EnviromentMovementBase GetObstacleFromPool()
    {
        return _poolObstacle.Get();
    }

    public CollectableBase GetCollectableFromPool()
    {
        return _poolCollectable.Get();
    }

    public List<CollectableBase> GetInstanciatedCollectables()
    {
        return instantiatedCollectables;
    }

    public List<EnviromentMovementBase> GetInstanciatedObstacles()
    {
        return instantiatedObstacles;
    }
}
