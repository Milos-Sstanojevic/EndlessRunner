using UnityEngine;

public class SpaceshipController : MovementManager, ICollectible, IDestroyable
{
    public float RotationSpeed;

    private void Awake()
    {
        EventManager.Instance.OnCollectAction += Collect;
    }

    protected override void Update()
    {
        base.Update();

        RotateSpaceship();
        if (transform.position.z < GlobalConstants.PositionBehindPlayerAxisZ)
        {
            Destroy();
        }
    }

    private void RotateSpaceship()
    {
        transform.Rotate(Time.deltaTime * RotationSpeed * Vector3.forward);
    }

    public void Collect(ICollectible collectible)
    {
        if (collectible == this)
            Destroy();
    }

    public void Destroy()
    {
        EventManager.Instance.OnObjectDestroyed(this);
    }
}
