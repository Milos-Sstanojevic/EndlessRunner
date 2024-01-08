using UnityEngine;
using static GlobalConstants;
public class ObstacleController : MonoBehaviour, IDestroyable
{
    private float movementSpeed;
    private bool canMove;

    public void SetMovementSpeed(float speed) => movementSpeed = speed;
    public void SetMovementDisabled() => canMove = false;
    public void SetMovementEnabled() => canMove = true;

    private void Update()
    {
        MoveObstacle();
    }

    private void MoveObstacle()
    {
        if (canMove)
        {
            transform.Translate(Vector3.back * movementSpeed * Time.deltaTime, Space.World);
            Destroy();
        }
    }

    public void Destroy()
    {
        if (transform.position.z < PositionBehindPlayerAxisZ)
            EventManager.Instance.Destroy(this);
    }
}
