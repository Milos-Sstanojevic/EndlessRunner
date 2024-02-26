using Fusion;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Transform player;
    private float startPositionZ;
    private float startPositionY;
    private Quaternion startRotation;

    private void Start()
    {
        startPositionZ = transform.position.z;
        startPositionY = transform.position.y;
        startRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        transform.SetPositionAndRotation(new Vector3(player.position.x, startPositionY, startPositionZ), startRotation);
    }
}
