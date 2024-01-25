using System.Collections.Generic;

public class EnemyPoolingSystem : PoolingSystemBase<EnemyController>
{
    public static EnemyPoolingSystem Instance { get; private set; }
    private List<EnemyController> instantiatedObstacles;

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

    private void Start()
    {
        instantiatedObstacles = new List<EnemyController>();
    }

    protected override EnemyController SpawnObject()
    {
        EnemyController obj = base.SpawnObject();
        instantiatedObstacles.Add(obj);
        return obj;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnDestroyEnemyAction -= DestroyObjects;
    }


    public List<EnemyController> GetInstanciatedObstacles() => instantiatedObstacles;

}
