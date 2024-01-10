using UnityEngine;

public class MovementManager : MonoBehaviour
{
    private bool movementEnabled;
    private float movementSpeed;

    protected virtual void Update()
    {
        MoveObject();
    }

    private void MoveObject()
    {
        if (movementEnabled)
        {
            transform.Translate(Vector3.back * movementSpeed * Time.deltaTime, Space.World);
        }
    }

    public void EnableMovement()
    {
        movementEnabled = true;
    }

    public void DisableMovement()
    {
        movementEnabled = false;
    }

    public void SetMovementSpeed(float speed)
    {
        movementSpeed = speed;
    }
}
