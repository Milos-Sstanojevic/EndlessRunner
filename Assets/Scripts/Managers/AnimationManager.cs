using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private Animator animator;
    private const string deadAnimation = "Dead";
    private const string jumpAnimation = "Jump";
    private const string runAnimation = "Run";
    private const string walkAnimation = "Walk";
    private const string runWithGunAnimation = "HasGun";
    private const string gunShootingAnimation = "Shoot";

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayRunAnimation()
    {
        animator.SetBool(runAnimation, true);
    }

    public void StopRunAnimation()
    {
        animator.SetBool(runAnimation, false);
    }

    //Happens when space is pressed
    public void PlayJumpAnimation()
    {
        animator.SetBool(jumpAnimation, true);
    }

    public void StopJumpAnimation()
    {
        animator.SetBool(jumpAnimation, false);
    }

    public void PlayDeadAnimation()
    {
        animator.SetBool(deadAnimation, true);
    }

    public void StartWalkAnimation()
    {
        animator.SetBool(walkAnimation, true);
    }

    public void StopWalkAnimation()
    {
        animator.SetBool(walkAnimation, false);
    }

    public void StartRunningWithGunAnimation()
    {
        animator.SetBool(runWithGunAnimation, true);
    }

    public void StopRunningWithGunAnimation()
    {
        animator.SetBool(runWithGunAnimation, false);
    }

    //Happens while left mouse button is pressed
    public void StartShootingAnimation()
    {
        animator.SetBool(gunShootingAnimation, true);
    }

    //Happens when left mouse button is released
    public void StopShootingAnimation()
    {
        animator.SetBool(gunShootingAnimation, false);
    }
}

