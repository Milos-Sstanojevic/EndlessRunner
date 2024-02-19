using System.Collections;
using UnityEngine;

public class PlayerGunHandler : MonoBehaviour
{
    private const int CollectablePointsWorth = 5;
    [SerializeField] private GameObject gunPosition;
    private PlayerMovement playerMovement;
    private GunController gunInHands;
    private PlayerJetHandler jetHandler;

    private void Awake()
    {
        jetHandler = GetComponent<PlayerJetHandler>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (playerMovement.IsMovementEnabled())
            ContinueGunCoroutine();
        else
            StopGunCoroutine();
    }

    public int CollectGun(GunController collectable)
    {
        GunController gun = collectable;
        if ((gunInHands == null || gunInHands.HasGun == false) && !jetHandler.HasJetOnBack())
        {
            gunInHands = gun;
            gun.MoveToPlayerHand(this, gunPosition.transform.position);
        }
        else
        {
            gun.ReleaseGunInPool();
        }

        AudioManager.Instance.PlayGunCollectedSound();

        return CollectablePointsWorth * 2;
    }

    private void ContinueGunCoroutine()
    {
        if (gunInHands != null)
            gunInHands.UnpauseCoroutine();
    }

    private void StopGunCoroutine()
    {
        if (gunInHands != null)
            gunInHands.PauseCoroutine();
    }

    public void ReleaseGunIfHaveOne()
    {
        if (gunInHands != null)
            StartCoroutine(ReleaseGunCoroutine());
    }

    private IEnumerator ReleaseGunCoroutine()
    {
        gunInHands.ReleaseGunInPool();
        yield return null;
        gunInHands = null;
    }

    public GunController GetGunInHands() => gunInHands;
    public bool HasGun() => gunInHands != null;
    public void SetGunInHandsToNull()
    {
        gunInHands = null;
    }
}
