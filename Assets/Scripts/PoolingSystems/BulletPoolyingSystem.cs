public class BulletPoolyingSystem : PoolingSystemBase<BulletController>
{
    public static BulletPoolyingSystem Instance { get; private set; }

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
        EventManager.Instance.OnBulletDestroyAction += DestroyObjects;
    }
    // protected override void Start()
    // {
    //     base.Start();
    // }

    private void OnDisable()
    {
        EventManager.Instance.OnBulletDestroyAction -= DestroyObjects;
    }
}
