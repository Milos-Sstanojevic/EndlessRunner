using System.Collections;
using UnityEngine;
public class ShootingController : MonoBehaviour
{
    private float lastShootTime;
    private float shootDelay = 0.1f;
    private float range = 15;
    private bool enemyIsHit;
    [SerializeField] private BulletPoolyingSystem bulletPoolyingSystem;

    public void ShootBullet()
    {
        if (lastShootTime + shootDelay < Time.time)
        {
            RaycastHit hit;
            BulletController bullet = bulletPoolyingSystem.GetBulletFromPool();

            bullet.transform.position = transform.position;

            if (Physics.Raycast(transform.position, Vector3.forward, out hit, range))
            {
                enemyIsHit = false;
                StartCoroutine(MakeTrail(bullet, hit.point));

                EnemyController enemy = hit.transform.GetComponent<EnemyController>();

                if (enemy != null)
                {
                    enemy.TakeDamage(bullet.GetBulletDamage());
                    enemyIsHit = true;
                }

                lastShootTime = Time.time;
            }
            else
            {
                StartCoroutine(MakeTrail(bullet, transform.position + Vector3.forward * range));
                lastShootTime = Time.time;
            }
        }
    }

    public IEnumerator MakeTrail(BulletController bullet, Vector3 hitPoint)
    {
        Vector3 startPosition = bullet.transform.position;
        float distance = Vector3.Distance(bullet.transform.position, hitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            bullet.transform.position = Vector3.Lerp(startPosition, hitPoint, 1 - (remainingDistance / distance));
            remainingDistance -= bullet.GetSpeed() * Time.deltaTime;

            yield return null;
        }

        bullet.transform.position = hitPoint;

        if (enemyIsHit)
        {
            EventManager.Instance.OnBulletDestroyed(bullet);
        }
    }
}
