using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private Animator animator;
    private const string DeadAnimation = "Dead";
    private const string JumpAnimation = "Jump";
    private const string RunAnimation = "Run";
    private const string WalkAnimation = "Walk";
    private const string RunWithGunAnimation = "HasGun";
    private const string FlyingAnimation = "Flying";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayRunAnimation()
    {
        animator.SetBool(RunAnimation, true);
    }

    public void StopRunAnimation()
    {
        animator.SetBool(RunAnimation, false);
    }

    //Happens when space is pressed
    public void PlayJumpAnimation()
    {
        animator.SetBool(JumpAnimation, true);
    }

    public void StopJumpAnimation()
    {
        animator.SetBool(JumpAnimation, false);
    }

    public void PlayDeadAnimation()
    {
        animator.SetBool(DeadAnimation, true);
    }

    public void StartWalkAnimation()
    {
        animator.SetBool(WalkAnimation, true);
    }

    public void StopWalkAnimation()
    {
        animator.SetBool(WalkAnimation, false);
    }

    public void StartRunningWithGunAnimation()
    {
        animator.SetBool(RunWithGunAnimation, true);
    }

    public void StopRunningWithGunAnimation()
    {
        animator.SetBool(RunWithGunAnimation, false);
    }

    public void StartFlyingAnimation()
    {
        animator.SetBool(FlyingAnimation, true);
    }

    public void StopFlyingAnimation()
    {
        animator.SetBool(FlyingAnimation, false);
    }

}

