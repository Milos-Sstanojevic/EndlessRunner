
using UnityEngine;

public class EnemyController : ObjectMovementBase
{
    [SerializeField] private AnimationManager enemyAnimator;
    private int isOnLeftEdge = -1;
    private float movementSpeed = 6.5f;

    protected override void Update()
    {
        base.Update();
        MoveLeftAndRight();
    }

    private void MoveLeftAndRight()
    {
        if (base.MovementEnabled == true)
        {
            enemyAnimator.StartWalkAnimation();
            transform.Translate((Vector3.left * isOnLeftEdge) * movementSpeed * Time.deltaTime, Space.World);
            RotateEnemy();
        }
        else
            enemyAnimator.StopWalkAnimation();
    }

    private void RotateEnemy()
    {
        if (transform.position.x < -GlobalConstants.EdgePosX)
        {
            transform.Rotate(0, -180, 0);
            isOnLeftEdge *= -1;
        }
        else if (transform.position.x > GlobalConstants.EdgePosX)
        {
            transform.Rotate(0, 180, 0);
            isOnLeftEdge *= -1;
        }
    }
}
