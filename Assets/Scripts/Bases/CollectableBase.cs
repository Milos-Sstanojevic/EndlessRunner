using UnityEngine;

public class CollectableBase : MonoBehaviour
{
    private float RotationSpeed = 80;

    private void Update()
    {
        RotateCollectable();
        Destroy();
    }

    private void RotateCollectable()
    {
        transform.Rotate(Time.deltaTime * RotationSpeed * Vector3.up, Space.World);
    }

    private void Destroy()
    {
        if (transform.position.z < GlobalConstants.PositionBehindPlayerAxisZ)
            EventManager.Instance.OnSpaceshipDestroyed(this);
    }

}
