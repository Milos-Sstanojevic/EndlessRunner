
public class GunPoolingSystem : PoolingSystemBase<GunController>
{
    public static GunPoolingSystem Instance { get; private set; }

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
        EventManager.Instance.OnDestroyGunAction += DestroyObjects;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnDestroyGunAction -= DestroyObjects;
    }
}
