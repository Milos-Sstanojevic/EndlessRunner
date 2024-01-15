using UnityEngine;

public class CollectableBase : EnviromentMovementBase
{
    protected float RotationSpeed = 80;

    protected override void Update()
    {
        base.Update();
        RotateCollectable();
    }

    private void RotateCollectable()
    {
        if (base.MovementEnabled == true)
            transform.Rotate(Time.deltaTime * RotationSpeed * Vector3.up, Space.World);
    }

}
