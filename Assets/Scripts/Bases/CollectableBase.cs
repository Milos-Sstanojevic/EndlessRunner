using UnityEngine;

public class CollectableBase : ObjectMovementBase
{
    protected float RotationSpeed = 80;

    private void Awake()
    {
        EventManager.Instance.OnCollectAction += Collect;
    }

    protected override void Update()
    {
        base.Update();
        RotateSpaceship();
    }

    private void RotateSpaceship()
    {
        if (base.MovementEnabled == true)
            transform.Rotate(Time.deltaTime * RotationSpeed * Vector3.up, Space.World);
    }

    public void Collect(CollectableBase collectible, int pointsWorth)
    {
        if (collectible == this)
            DestroyWhenCollected();
    }

    public void DestroyWhenCollected()
    {
        EventManager.Instance.OnObjectDestroyed(this);
    }
}
