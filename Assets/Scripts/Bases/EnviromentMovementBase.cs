using UnityEngine;

public class EnviromentMovementBase : MonoBehaviour
{
    protected bool MovementEnabled { get; private set; }
    protected float MovementSpeed { get; private set; }

    protected virtual void Update()
    {
        MoveObject();
        Destroy();
    }

    private void MoveObject()
    {

        if (MovementEnabled)
        {

            transform.Translate(Vector3.back * MovementSpeed * Time.deltaTime, Space.World);
        }
    }

    public void EnableMovement()
    {
        MovementEnabled = true;
    }

    public void DisableMovement()
    {
        MovementEnabled = false;
    }

    public void SetMovementSpeed(float speed)
    {
        MovementSpeed = speed;
    }

    protected virtual void Destroy()
    {
        if (transform.position.z < GlobalConstants.PositionBehindPlayerAxisZ)
            EventManager.Instance.OnEnviromentDestroyed(this);
    }
}
