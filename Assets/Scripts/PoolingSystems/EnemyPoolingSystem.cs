
using UnityEngine;

public class EnemyPoolingSystem : PoolingSystemBase<EnemyController>
{
    public static EnemyPoolingSystem Instance { get; private set; }

    [SerializeField] private EnemyController flyingEnemy;
    [SerializeField] private EnemyController groundEnemy;

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
        EventManager.Instance.OnDestroyEnemyAction += DestroyObjects;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnDestroyEnemyAction -= DestroyObjects;
    }

    public void SetFlyingEnemyAsBasePrefab()
    {
        base.objectPrefab = flyingEnemy;
    }

    public void SetGroundEnemyAsBasePrefab()
    {
        base.objectPrefab = groundEnemy;
    }
}
