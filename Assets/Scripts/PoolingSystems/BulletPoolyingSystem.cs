using UnityEngine;
using UnityEngine.Pool;

public class BulletPoolyingSystem : MonoBehaviour
{
    public static BulletPoolyingSystem Instance { get; private set; }

    private const int defaultCapacityForBullets = 10;
    private const int maximumCapacityForBullets = 20;
    private ObjectPool<BulletController> _poolBullet;
    [SerializeField] private BulletController bulletController;
    [SerializeField] private Transform parentBulletPool;


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

    private void Start()
    {
        EventManager.Instance.OnBulletDestroyAction += DestroyBullet;
        CreateBulletPool();
    }

    private void DestroyBullet(BulletController bullet)
    {
        _poolBullet.Release(bullet);
    }

    public void CreateBulletPool()
    {
        _poolBullet = new ObjectPool<BulletController>(CreateBullet, bullet =>
        {
            bullet.gameObject.SetActive(true);
        }, bullet =>
        {
            bullet.gameObject.SetActive(false);
        }, bullet =>
        {
            Destroy(bullet.gameObject);
        }, true, defaultCapacityForBullets, maximumCapacityForBullets);
    }

    private BulletController CreateBullet()
    {
        BulletController bullet = Instantiate(bulletController);
        bullet.transform.SetParent(parentBulletPool);
        return bullet;
    }

    public BulletController GetBulletFromPool()
    {
        return _poolBullet.Get();
    }

    private void OnDisable()
    {
        EventManager.Instance.OnBulletDestroyAction -= DestroyBullet;
    }
}
