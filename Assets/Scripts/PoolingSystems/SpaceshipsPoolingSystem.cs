
public class SpaceshipPoolingSystem : PoolingSystemBase<CollectableBase>
{
    public static SpaceshipPoolingSystem Instance { get; private set; }

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
        EventManager.Instance.OnSpaceshipDestroyAction += DestroyObjects;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnSpaceshipDestroyAction -= DestroyObjects;
    }
}
