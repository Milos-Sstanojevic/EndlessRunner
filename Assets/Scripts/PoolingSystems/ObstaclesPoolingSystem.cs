using System.Collections.Generic;
using UnityEngine;

public class ObstaclesPoolingSystem : PoolingSystemBase<EnvironmentMovementController>
{
    public static ObstaclesPoolingSystem Instance { get; private set; }

    [SerializeField] private EnvironmentMovementController leftAndRightObstacle;
    [SerializeField] private EnvironmentMovementController leftObstacle;
    [SerializeField] private EnvironmentMovementController rightObstacle;
    [SerializeField] private EnvironmentMovementController roadblockObstacle;

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
