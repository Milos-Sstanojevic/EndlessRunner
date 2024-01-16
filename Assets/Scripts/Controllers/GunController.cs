using System.Collections;
using UnityEngine;

public class GunController : CollectableBase
{
    private const int gunTimeToLive = 5;
    private static Vector3 rotateAroundX = new Vector3(90, 0, 0);
    private ParticleSystemManager gunMuzzleParticle;
    [SerializeField] private GameObject muzzleParticleObject;
    [SerializeField] private Transform bulletStartPoint;
    [SerializeField] private BulletController bulletController;
    private BulletPoolyingSystem bulletPoolyingSystem;
    private float range = 60f;

    public bool HasGun { get; private set; }

    private void Awake()
    {
        gunMuzzleParticle = GetComponent<ParticleSystemManager>();
        bulletPoolyingSystem = GetComponent<BulletPoolyingSystem>();
    }

    protected override void Update()
    {
        if (!HasGun)
        {
            base.Update();
        }
    }

    public void ShootFromGun()
    {
        muzzleParticleObject.SetActive(true);
        gunMuzzleParticle.PlayMuzzleParticleEffect();

        RaycastHit hit;
        if (Physics.Raycast(bulletStartPoint.position, Vector3.forward, out hit, range))
        {
            BulletController trail = bulletPoolyingSystem.GetBulletFromPool();

            StartCoroutine(MakeTrail(trail, hit.point));

            EnemyController enemy = hit.transform.GetComponent<EnemyController>();
            if (enemy != null)
            {
                Debug.Log("Enemy hit");
            }
        }
    }


    public void StopShooting()
    {
        muzzleParticleObject.SetActive(false);
        gunMuzzleParticle.StopMuzzleParticleEffect();
    }

    public IEnumerator MakeTrail(BulletController trail, Vector3 hitPoint)
    {
        Vector3 startPosition = trail.transform.position;
        float distance = Vector3.Distance(trail.transform.position, hitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hitPoint, 1 - (remainingDistance / distance));
            remainingDistance -= bulletController.GetSpeed() * Time.deltaTime;

            yield return null;
        }

        trail.transform.position = hitPoint;
    }


    public void MoveToPlayerHand(PlayerController player, Vector3 gunPos)
    {
        if (!HasGun)
        {
            transform.SetParent(player.transform);
            transform.position = gunPos;
            transform.eulerAngles = rotateAroundX;
            HasGun = true;
            StartCoroutine(GunExpiration());
        }
    }

    private IEnumerator GunExpiration()
    {
        yield return new WaitForSeconds(gunTimeToLive);
        HasGun = false;
        transform.Rotate(-90, 0, 0);
        EventManager.Instance.OnEnviromentDestroyed(this);
    }

    public void ReleaseGunInPool()
    {
        transform.Rotate(-90, 0, 0);
        EventManager.Instance.OnEnviromentDestroyed(this);
    }

}
