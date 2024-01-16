using UnityEngine;
using UnityEngine.Pool;

public class BulletPoolyingSystem : MonoBehaviour
{
    private ObjectPool<BulletController> _poolBullet;
    [SerializeField] private BulletController bulletController;
    [SerializeField] private Transform parentBulletPool;
    [SerializeField] private Transform bulletSpawnPoint;

    private void OnEnable()
    {
        EventManager.Instance.OnBulletDestroyAction += DestroyBullet;
    }

    private void DestroyBullet(BulletController bullet)
    {
        Debug.Log(bullet.transform.position);
        _poolBullet.Release(bullet);
    }

    private void Start()
    {
        CreateBulletPool();
    }

    public void CreateBulletPool()
    {
        _poolBullet = new ObjectPool<BulletController>(CreateBullet, bullet =>
        {
            bullet.transform.position = new Vector3(bulletSpawnPoint.position.x, bulletSpawnPoint.position.y, bulletSpawnPoint.position.z + 1f);
            bullet.gameObject.SetActive(true);
        }, bullet =>
        {
            bullet.gameObject.SetActive(false);
        }, bullet =>
        {
            Destroy(bullet.gameObject);
        }, true, 1000, 2000);
    }

    private BulletController CreateBullet()
    {
        BulletController bullet = Instantiate(bulletController, new Vector3(bulletSpawnPoint.position.x, bulletSpawnPoint.position.y, bulletSpawnPoint.position.z + 1f), Quaternion.identity);
        return bullet;
    }

    public BulletController GetBulletFromPool() => _poolBullet.Get();

    private void OnDisable()
    {
        EventManager.Instance.OnBulletDestroyAction -= DestroyBullet;
    }
}
