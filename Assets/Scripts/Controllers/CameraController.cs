using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 offset;
    [SerializeField] private Transform player;

    void Awake()
    {
        offset = transform.position - player.position;
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(player.position.x, transform.position.y, transform.position.z);
    }
}
