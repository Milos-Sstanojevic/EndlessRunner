using UnityEngine;
using UnityEngine.Pool;

public class BulletPoolyingSystem : PoolingSystemBase<BulletController>
{
    public static BulletPoolyingSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected override void Start()
    {
        base.Start();
        EventManager.Instance.OnBulletDestroyAction += DestroyObject;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnBulletDestroyAction -= DestroyObject;
    }
}
