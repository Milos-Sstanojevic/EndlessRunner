
public class JetPoolingSystem : PoolingSystemBase<JetController>
{
    public static JetPoolingSystem Instance { get; private set; }

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
        EventManager.Instance.OnDestroyJetAction += DestroyObjects;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnDestroyJetAction -= DestroyObjects;
    }
}
