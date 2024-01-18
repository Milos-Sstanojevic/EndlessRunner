using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    private float lastShootTime;
    private float shootDelay = 0.1f;
    private float range = 15;
    private bool enemyHit;
    [SerializeField] private BulletPoolyingSystem bulletPoolyingSystem;

    public void ShootBullet()
    {
        if (lastShootTime + shootDelay < Time.time)
        {
            RaycastHit hit;
            BulletController bullet = bulletPoolyingSystem.GetBulletFromPool();

            bullet.transform.position = transform.position;
            Debug.Log($"Pozicija spawna: {transform.position} ::: pozicija: {bullet.transform.position}");

            if (Physics.Raycast(transform.position, Vector3.forward, out hit, range))
            {
                enemyHit = false;
                StartCoroutine(MakeTrail(bullet, hit.point));
                if (hit.transform.CompareTag("Enemy"))
                {
                    Debug.Log("Enemy hit");
                    enemyHit = true;
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

        if (enemyHit)
        {
            EventManager.Instance.OnBulletDestroyed(bullet);
        }
    }
}
