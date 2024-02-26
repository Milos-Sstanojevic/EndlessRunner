using Fusion;
using UnityEngine;

public class StageController : NetworkBehaviour
{
    private const string GroundTag = "Ground";
    private float totalStageLength;
    private Vector3 spawnStagePoint;

    private void Awake()
    {
        CalculateTotalStageLength();
    }

    private void OnEnable()
    {
        EventManager.Instance.SubscribeToOnNewStagePositionSpawnedAction(SetStageSpawnPosition);
    }

    private void SetStageSpawnPosition(Vector3 position)
    {
        spawnStagePoint = position;
    }

    private void CalculateTotalStageLength()
    {
        totalStageLength = 0f;

        totalStageLength += transform.GetChild(0).GetComponent<MeshCollider>().bounds.size.z;

        foreach (Transform child in transform)
        {
            if (!child.CompareTag(GroundTag))
                continue;

            MeshCollider collider = child.GetComponent<MeshCollider>();

            if (collider != null)
                totalStageLength += collider.bounds.size.z;
        }
    }

    public void Update()
    {
        MoveStageToTheEnd();
    }

    private void MoveStageToTheEnd()
    {
        if (transform.position.z > -totalStageLength)
            return;

        float errorOffset = totalStageLength + transform.position.z;
        transform.position = new Vector3(transform.position.x, 0, spawnStagePoint.z + errorOffset);
    }
}
