using UnityEngine;

public class StageController : MonoBehaviour
{
    private const string GroundTag = "Ground";
    private float totalStageLength;
    [SerializeField] private GameObject spawnStagePoint;

    private void Awake()
    {
        CalculateTotalStageLength();
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
            {
                totalStageLength += collider.bounds.size.z;
            }
        }
    }

    private void Update()
    {
        MoveStageToTheEnd();
    }

    private void MoveStageToTheEnd()
    {
        if (transform.position.z > -totalStageLength)
            return;

        float errorOffset = totalStageLength + transform.position.z;
        transform.position = new Vector3(0, 0, spawnStagePoint.transform.position.z + errorOffset);
    }
}
