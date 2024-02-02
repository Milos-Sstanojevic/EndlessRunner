using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private AnimationManager animationManager;
    private PlayerMovement playerMovement;
    private PlayerGunHandler playerGunHandler;
    private PlayerJetHandler playerJetHandler;

    private void Awake()
    {
        animationManager = GetComponent<AnimationManager>();
        playerMovement = GetComponent<PlayerMovement>();
        playerJetHandler = GetComponent<PlayerJetHandler>();
        playerGunHandler = GetComponent<PlayerGunHandler>();
    }

    private void Update()
    {
        StartPlayerAnimations();
        StopPlayerAnimations();
    }

    private void StartPlayerAnimations()
    {
        if (playerMovement.IsMovementEnabled())
        {
            animationManager.PlayRunAnimation();

            if (playerGunHandler.GetGunInHands()?.HasGun == true)
                animationManager.StartRunningWithGunAnimation();
            else
                animationManager.StopRunningWithGunAnimation();

            if (playerJetHandler.GetJetOnBack()?.HasJet == true)
                animationManager.StartFlyingAnimation();
            else
                animationManager.StopFlyingAnimation();
        }
    }

    private void StopPlayerAnimations()
    {
        if (!playerMovement.IsMovementEnabled())
        {
            animationManager.StopRunAnimation();
            animationManager.StopRunningWithGunAnimation();
        }
    }

    public void PlayJumpAnimation()
    {
        animationManager.PlayJumpAnimation();
    }
}
