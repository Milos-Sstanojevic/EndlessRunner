using Fusion;
using UnityEngine;

public class EnvironmentMovementController : MonoBehaviour, IDestroyable
{
    [SerializeField] private int chanceForThisObstacle;
    [SerializeField] private bool isStage;
    public bool MovementEnabled { get; private set; }
    public float MovementSpeed { get; private set; }

    public void Update()
    {
        MoveObject();
    }

    public void MoveObject()
    {
        if (MovementEnabled)
            transform.Translate(Vector3.back * MovementSpeed * Time.deltaTime, Space.World);
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

    public int GetChanceForThisObstacle() => chanceForThisObstacle;

    public void Destroy()
    {
        EventManager.Instance.OnEnvironmentDestroyed(this);
    }
}
