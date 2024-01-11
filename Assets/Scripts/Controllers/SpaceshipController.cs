using UnityEngine;

public class SpaceshipController : ObjectManager, ICollectible, IDestroyable
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
    }

    private void RotateSpaceship()
    {
        if (base.MovementEnabled == true)
            transform.Rotate(Time.deltaTime * RotationSpeed * Vector3.forward);
    }

    public void Collect(ICollectible collectible)
    {
        if (collectible == this)
            DestroyWhenCollected();
    }

    public void DestroyWhenCollected()
    {
        EventManager.Instance.OnObjectDestroyed(this);
    }
}
