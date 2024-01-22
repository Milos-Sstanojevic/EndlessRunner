using System.Collections;
using UnityEngine;

public class GunController : CollectableBase
{
    private const int gunTimeToLive = 5;
    private static Vector3 rotateAroundX = new Vector3(90, 0, 0);
    [SerializeField] private GameObject muzzleParticleObject;
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
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                muzzleParticleObject.SetActive(false);
            }
        }
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
        transform.Rotate(-rotateAroundX);
        muzzleParticleObject.SetActive(false);
        EventManager.Instance.OnEnviromentDestroyed(this);
    }

    public void ReleaseGunInPool()
    {
        transform.Rotate(-rotateAroundX);
        muzzleParticleObject.SetActive(false);
        EventManager.Instance.OnEnviromentDestroyed(this);
    }

}
