using UnityEngine;

public class BulletController : MonoBehaviour
{
    public TrailRenderer bulletTrail;
    private float bulletSpeed = 50;

    private void Start()
    {
        bulletTrail = GetComponent<TrailRenderer>();

        ReturnBulletToPool();
    }

    private void ReturnBulletToPool()
    {
        if (transform.position.z > -7f)
        {
            EventManager.Instance.OnBulletDestroyed(this);
        }
    }

    public float GetSpeed() => bulletSpeed;
}
