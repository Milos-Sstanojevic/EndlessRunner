using System.Collections;
using UnityEngine;

public class GunController : CollectableBase
{
    private const int gunTimeToLive = 5;
    private static Vector3 rotateAroundX = new Vector3(90, 0, 0);
    private static Vector3 defaultOrientation = new Vector3(0, 0, 0);
    [SerializeField] private GameObject muzzleParticleObject;
    private float lastShootTime;
    private float shootDelay = 0.1f;
    private float range = 15;
    private bool enemyIsHit;
    public bool HasGun { get; private set; }


    protected override void Update()
    {
        if (!HasGun)
        {
            base.Update();
        }
        ShootFromGun();
    }

    public void ShootFromGun()
    {
        if (HasGun)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                muzzleParticleObject.SetActive(true);
                ShootBullet();
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                muzzleParticleObject.SetActive(false);
            }
        }
    }

    public void ShootBullet()
    {
        if (lastShootTime + shootDelay < Time.time)
        {
            RaycastHit hit;
            BulletController bullet = BulletPoolyingSystem.Instance.GetObjectFromPool();

            bullet.transform.SetPositionAndRotation(muzzleParticleObject.transform.position, Quaternion.identity);

            if (Physics.Raycast(transform.position, Vector3.forward, out hit, range))
            {
                enemyIsHit = false;
                EnemyController enemy = hit.transform.GetComponent<EnemyController>();

                if (enemy != null)
                {
                    enemy.TakeDamage(bullet.GetBulletDamage());
                    enemyIsHit = true;
                }

                StartCoroutine(MakeTrail(bullet, hit.point));

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

        EventManager.Instance.OnBulletDestroyed(bullet);
    }

    public void MoveToPlayerHand(PlayerController player, Vector3 gunPos)
    {
        transform.SetParent(player.transform);
        transform.position = gunPos;
        transform.eulerAngles = rotateAroundX;
        HasGun = true;
        StartCoroutine(GunExpiration());
    }

    private IEnumerator GunExpiration()
    {
        yield return new WaitForSeconds(gunTimeToLive);
        HasGun = false;
        transform.eulerAngles = defaultOrientation;
        muzzleParticleObject.SetActive(false);
        EventManager.Instance.OnEnviromentDestroyed(this);
    }

    public void ReleaseGunInPool()
    {
        transform.eulerAngles = defaultOrientation;
        muzzleParticleObject.SetActive(false);
        EventManager.Instance.OnEnviromentDestroyed(this);
    }

}
