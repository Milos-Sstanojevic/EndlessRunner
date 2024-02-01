using UnityEngine;

public class CollectableController : MonoBehaviour
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
        if (transform.position.z < MapEdgeConstants.PositionBehindPlayerAxisZ)
            EventManager.Instance.OnSpaceshipDestroyed(this);
    }

}
