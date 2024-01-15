using System.Collections;
using UnityEngine;

public class GunController : CollectableBase
{
    //private AnimationManager gunAnimation;
    private static Vector3 rotateAroundX = new Vector3(90, 0, 0);
    private const int gunTimeToLive = 5;
    public bool HasGun { get; private set; }

    private void Awake()
    {
        //gunAnimation = GetComponent<AnimationManager>();
    }

    protected override void Update()
    {
        if (!HasGun)
        {
            base.Update();
        }

        //ShootFromGun();
    }

    // private void ShootFromGun()
    // {
    //     if (HasGun)
    //     {
    //         if (Input.GetKeyDown(KeyCode.Mouse0))
    //         {
    //             muzzleFlash.SetActive(true);
    //             gunAnimation.StartShootingAnimation();
    //         }
    //         if (Input.GetKeyUp(KeyCode.Mouse0))
    //         {
    //             muzzleFlash.SetActive(false);
    //             gunAnimation.StopShootingAnimation();
    //         }
    //     }
    // }

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
        EventManager.Instance.OnObjectDestroyed(this);
    }

    public void ReleaseGunInPool()
    {
        EventManager.Instance.OnObjectDestroyed(this);
    }

}
