using UnityEngine;

public class EnvironmentMovementController : MonoBehaviour
{
    public bool MovementEnabled { get; private set; }
    public float MovementSpeed { get; private set; }
    [SerializeField] private bool isStage;

    private void Update()
    {
        MoveObject();
    }

    public void MoveObject()
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
}
