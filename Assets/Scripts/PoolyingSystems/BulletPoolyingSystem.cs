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
        Debug.Log("Vracam u pool");
        _poolBullet.Release(bullet);
    }


    public void CreateBulletPool()
    {
        _poolBullet = new ObjectPool<BulletController>(CreateBullet, bullet =>
        {
            Debug.Log("Postavljam na aktive");
            bullet.gameObject.SetActive(true);
        }, bullet =>
        {
            Debug.Log("Postavljam na deaktive");
            bullet.gameObject.SetActive(false);
        }, bullet =>
        {
            Debug.Log("Unistavam bullet");
            Destroy(bullet.gameObject);
        }, true, 2, 4);
    }

    private BulletController CreateBullet()
    {
        Debug.Log("Kreiram bullet");
        BulletController bullet = Instantiate(bulletController);
        bullet.transform.SetPositionAndRotation(bulletSpawnPoint.position, Quaternion.identity);
        bullet.transform.SetParent(parentBulletPool);
        return bullet;
    }

    public BulletController GetBulletFromPool()
    {
        Debug.Log("Uzimam iz pool-a");
        return _poolBullet.Get();
    }

    private void OnDisable()
    {
        EventManager.Instance.OnBulletDestroyAction -= DestroyBullet;
    }
}
