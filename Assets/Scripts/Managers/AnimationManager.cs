using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private Animator animator;
    private const string deadAnimation = "Dead";
    private const string jumpAnimation = "Jump";
    private const string runAnimation = "Run";
    private const string walkAnimation = "Walk";

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
}
