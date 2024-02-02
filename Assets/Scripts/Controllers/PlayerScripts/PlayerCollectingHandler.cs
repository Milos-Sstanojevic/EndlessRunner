using UnityEngine;

public class PlayerCollectingHandler : MonoBehaviour
{
    private const int CollectablePointsWorth = 5;
    private PlayerJetHandler jetHandler;
    private PlayerGunHandler gunHandler;

    private void Awake()
    {
        jetHandler = GetComponent<PlayerJetHandler>();
        gunHandler = GetComponent<PlayerGunHandler>();
    }

    public int Collect(CollectableController collectable)
    {
        JetController jet = collectable.GetComponent<JetController>();
        GunController gun = collectable.GetComponent<GunController>();

        if (jet != null)
        {
            if (gunHandler.HasGun() && gunHandler.GetGunInHands().HasGun)
            {
                gunHandler.GetGunInHands().ReleaseGunInPool();
                gunHandler.SetGunInHandsToNull();
            }

            return jetHandler.CollectJet(jet);
        }
        else if (gun != null)
        {
            return gunHandler.CollectGun(gun);
        }
        else
        {
            AudioManager.Instance.PlaySpaceshipCollectedSound();
            EventManager.Instance.OnSpaceshipDestroyed(collectable);
            return CollectablePointsWorth;
        }
    }
}
