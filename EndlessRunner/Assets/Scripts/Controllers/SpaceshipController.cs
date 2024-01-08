using UnityEngine;
using static GlobalConstants;

public class SpaceshipController : MonoBehaviour, ICollectible, IDestroyable
{
    public float RotationSpeed;
    private float movementSpeed;
    private bool canMove;

    public void SetMovementSpeed(float speed) => movementSpeed = speed;
    public void SetMovementDisabled() => canMove = false;
    public void SetMovementEnabled() => canMove = true;

    private void Awake()
    {
        EventManager.Instance.OnCollectAction += Collect;
    }

    private void Update()
    {
        if (canMove)
        {
            RotateSpaceship();
            MoveSpaceship();

            if (transform.position.z < PositionBehindPlayerAxisZ)
            {
                Destroy();
            }
        }
    }

    private void RotateSpaceship()
    {
        transform.Rotate(Time.deltaTime * RotationSpeed * Vector3.forward);
    }

    private void MoveSpaceship()
    {
        transform.Translate(Vector3.back * movementSpeed * Time.deltaTime, Space.World);
    }

    public void Collect(ICollectible collectible)
    {
        if (collectible == this)
            Destroy();
    }

    public void Destroy()
    {
        EventManager.Instance.Destroy(this);
    }
}
