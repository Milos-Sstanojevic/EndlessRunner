using UnityEngine;

public class MovementManager : MonoBehaviour, IDestroyable
{
    private bool movementEnabled;
    private float movementSpeed;

    protected virtual void Update()
    {
        MoveObject();
        Destroy();
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

    public void Destroy()
    {
        if (transform.position.z < GlobalConstants.PositionBehindPlayerAxisZ)
            EventManager.Instance.OnObjectDestroyed(this);
    }
}
