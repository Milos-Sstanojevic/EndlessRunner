using UnityEngine;
using UnityEngine.Pool;

public class BulletPoolyingSystem : MonoBehaviour
{
    private ObjectPool<BulletController> _poolBullet;
    [SerializeField] private BulletController bulletController;
    [SerializeField] private Transform parentBulletPool;
    [SerializeField] private Transform bulletSpawnPoint;

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
        }, true, 10, 20);
    }

    private BulletController CreateBullet()
    {
        BulletController bullet = Instantiate(bulletController);
        bullet.transform.SetPositionAndRotation(bulletSpawnPoint.position, Quaternion.identity);
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
