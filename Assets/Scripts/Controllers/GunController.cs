using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
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
    [SerializeField] private int active;
    [SerializeField] private int inactive;
    [SerializeField] private int all;
    private EnvironmentMovementBase environmentComponent;
    private CollectableBase collectableComponent;
    private int bulletsFiredInOneSecond = 0;
    private float timer = 0f;

    private void Awake()
    {
        environmentComponent = GetComponent<EnvironmentMovementBase>();
        collectableComponent = GetComponent<CollectableBase>();
    }

    private void Update()
    {
        if (!HasGun)
        {
            environmentComponent.enabled = true;
            collectableComponent.enabled = true;
        }
        ShootFromGun();

        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            Debug.Log($"Bullets fired in last second: {bulletsFiredInOneSecond}");
            bulletsFiredInOneSecond = 0;
            timer = 0f;
        }

        Destroy();
    }

    private void Destroy()
    {
        if (transform.position.z < GlobalConstants.PositionBehindPlayerAxisZ)
            EventManager.Instance.OnGunDestroyed(this);
    }

    public void ShootFromGun()
    {
        if (HasGun)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                muzzleParticleObject.SetActive(true);
                ShootBullet();

                bulletsFiredInOneSecond++;
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

            all = BulletPoolyingSystem.Instance.GetCountAll();
            active = BulletPoolyingSystem.Instance.GetCountActive();
            inactive = BulletPoolyingSystem.Instance.GetCountInactive();


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
        environmentComponent.enabled = false;
        collectableComponent.enabled = false;
        StartCoroutine(GunExpiration());
    }

    private IEnumerator GunExpiration()
    {
        yield return new WaitForSeconds(gunTimeToLive);
        HasGun = false;
        transform.eulerAngles = defaultOrientation;
        muzzleParticleObject.SetActive(false);
        EventManager.Instance.OnGunDestroyed(this);
    }

    public void ReleaseGunInPool()
    {
        transform.eulerAngles = defaultOrientation;
        muzzleParticleObject.SetActive(false);
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
