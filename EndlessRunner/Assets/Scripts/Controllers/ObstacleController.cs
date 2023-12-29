using System;
using UnityEngine;
using static GlobalConstants;
public class ObstacleController : MonoBehaviour, IDestroyable
{
    public static event Action<ObstacleController> OnDestroyObstacle;
    private float movementSpeed;
    private bool canMove;


    protected virtual void Update()
    {
        if (canMove)
        {
            MoveObstacle();
        }
    }

    private void MoveObstacle()
    {
        transform.Translate(Vector3.back * movementSpeed * Time.deltaTime, Space.World);

        if (transform.position.z < PositionBehindPlayerAxisZ)//ako prodje iza player-a pokreni akciju koja ce uraditi Release u pool za obstacle
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        OnDestroyObstacle?.Invoke(this);
    }

    public void SetMovementSpeed(float speed) => movementSpeed = speed;


    public void SetMovementDisabled() => canMove = false;
    public void SetMovementEnabled() => canMove = true;
}
