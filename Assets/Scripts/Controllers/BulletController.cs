using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float bulletSpeed = 100;
    private void Update()
    {
        ReturnBulletToPool();
    }

    private void ReturnBulletToPool()
    {
        if (transform.position.z > 10f)
        {
            EventManager.Instance.OnBulletDestroyed(this);
        }
    }

    public float GetSpeed() => bulletSpeed;
}
