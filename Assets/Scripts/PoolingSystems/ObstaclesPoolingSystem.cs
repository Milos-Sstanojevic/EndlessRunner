using System.Collections.Generic;
using UnityEngine;

public class ObstaclesPoolingSystem : PoolingSystemBase<EnvironmentMovementBase>
{
    public static ObstaclesPoolingSystem Instance { get; private set; }

    [SerializeField] private EnvironmentMovementBase leftAndRightObstacle;
    [SerializeField] private EnvironmentMovementBase leftObstacle;
    [SerializeField] private EnvironmentMovementBase rightObstacle;
    [SerializeField] private EnvironmentMovementBase roadblockObstacle;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        EventManager.Instance.OnDestroyAction += DestroyObjects;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnDestroyAction -= DestroyObjects;
    }

    public void SetRoadblockAsBasePrefab()
    {
        base.objectPrefab = roadblockObstacle;
    }

    public void SetDoubleLeftAsBasePrefab()
    {
        base.objectPrefab = leftObstacle;
    }

    public void SetDoubleRightAsBasePrefab()
    {
        base.objectPrefab = rightObstacle;
    }

    public void SetLeftAndRightAsBasePrefab()
    {
        base.objectPrefab = leftAndRightObstacle;
    }
}
