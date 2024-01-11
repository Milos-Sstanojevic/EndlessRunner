using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private Animator animator;
    private const string DeadAnimation = "Dead";
    private const string JumpAnimation = "Jump";
    private const string RunAnimation = "Run";

    private void Start()
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
}
