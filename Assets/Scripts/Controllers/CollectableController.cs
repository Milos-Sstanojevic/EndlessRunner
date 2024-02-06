using UnityEngine;

public class CollectableController : MonoBehaviour, IDestroyable
{
    private const float RotationSpeed = 80;
    [SerializeField] private int chanceForThisCollectable;//70

    private void Update()
    {
        RotateCollectable();

        if (transform.position.z < MapEdgeConstants.PositionBehindPlayerAxisZ)
            Destroy();
    }

    private void RotateCollectable()
    {
        transform.Rotate(Time.deltaTime * RotationSpeed * Vector3.up, Space.World);
    }

    public void Destroy()
    {
        EventManager.Instance.OnSpaceshipDestroyed(this);
    }

    public int GetChanceForThisCollectable() => chanceForThisCollectable;
}
