using UnityEngine;

public class BulletController : MonoBehaviour
{
    private const float rangeOfBullet = 10f;
    private float bulletSpeed = 100;
    private int bulletDamage = 20;
    private void Update()
    {
        ReturnBulletToPool();
    }

    private void ReturnBulletToPool()
    {
        if (transform.position.z > rangeOfBullet)
        {
            EventManager.Instance.OnBulletDestroyed(this);
        }
    }

    public float GetSpeed() => bulletSpeed;
    public int GetBulletDamage() => bulletDamage;
}
