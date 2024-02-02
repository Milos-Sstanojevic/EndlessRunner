using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    private const int GunTimeToLive = 5;
    private static Vector3 RotateAroundX = new Vector3(90, 0, 0);
    private static Vector3 DefaultOrientation = new Vector3(0, 0, 0);
    [SerializeField] private GameObject muzzleParticleObject;
    private float lastShootTime;
    private float shootDelay = 0.1f;
    private float range = 15;
    private bool enemyIsHit;
    public bool HasGun { get; private set; }
    private bool gunExpired;
    private EnvironmentMovementController environmentComponent;
    private CollectableController collectableComponent;


    private void Awake()
    {
        environmentComponent = GetComponent<EnvironmentMovementController>();
        collectableComponent = GetComponent<CollectableController>();
    }

    private void Update()
    {
        if (!HasGun)
        {
            environmentComponent.enabled = true;
            collectableComponent.enabled = true;
        }

        if (gunExpired)
        {
            EventManager.Instance.OnGunDestroyed(this);
            gunExpired = false;
        }

        ShootFromGun();

        Destroy();
    }

    private void Destroy()
    {
        if (transform.position.z < MapEdgeConstants.PositionBehindPlayerAxisZ)
            EventManager.Instance.OnGunDestroyed(this);
    }

    public void ShootFromGun()
    {
        if (!HasGun)
            return;

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

    public void ShootBullet()
    {
        if (lastShootTime + shootDelay < Time.time)
        {
            RaycastHit hit;
            BulletController bullet = PoolingSystemController.Instance.GetBulletPoolingSystem().GetObjectFromPool();

            bullet.transform.SetPositionAndRotation(muzzleParticleObject.transform.position, Quaternion.identity);

            if (Physics.Raycast(transform.position, Vector3.forward, out hit, range))
            {
                enemyIsHit = false;
                EnemyController enemy = hit.transform.GetComponent<EnemyController>();

                if (enemy != null)
                {
                    enemy.TakeDamage(bullet.GetBulletDamage());
                    enemy.PlayBloodParticles();
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
        transform.eulerAngles = RotateAroundX;
        HasGun = true;
        environmentComponent.enabled = false;
        collectableComponent.enabled = false;
        StartCoroutine(GunExpiration());
    }

    private IEnumerator GunExpiration()
    {
        yield return new WaitForSeconds(GunTimeToLive);
        HasGun = false;
        transform.eulerAngles = DefaultOrientation;
        muzzleParticleObject.SetActive(false);
        DeactivateAllBullets();
        gunExpired = true;
    }


    private void DeactivateAllBullets()
    {
        List<BulletController> bullets = PoolingSystemController.Instance.GetBulletPoolingSystem().GetInstantiatedObjects();

        foreach (BulletController bullet in bullets)
            if (bullet.gameObject.activeSelf)
                EventManager.Instance.OnBulletDestroyed(bullet);
    }

    public void ReleaseGunInPool()
    {
        transform.eulerAngles = DefaultOrientation;
        muzzleParticleObject.SetActive(false);
        HasGun = false;
        DeactivateAllBullets();
        EventManager.Instance.OnGunDestroyed(this);
    }

    public void PauseCoroutine()
    {
        Time.timeScale = 0f;
    }

    public void UnpauseCoroutine()
    {
        Time.timeScale = 1f;
    }
}
