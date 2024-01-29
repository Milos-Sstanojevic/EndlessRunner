using UnityEngine;

public class StageController : MonoBehaviour
{
    private const string groundTag = "Ground";
    private const float stagePostitionOffset = 14.91158f;
    private float totalStageLength;
    private GameObject spawnStagePoint;

    private void Awake()
    {
        spawnStagePoint = GameObject.FindGameObjectWithTag("EndOfRoad");

        CalculateTotalStageLength();
    }

    private void CalculateTotalStageLength()
    {
        totalStageLength = 0f;

        foreach (Transform child in transform)
        {
            if (child.CompareTag(groundTag))
            {
                MeshCollider collider = child.GetComponent<MeshCollider>();

                if (collider != null)
                {
                    totalStageLength += collider.bounds.size.z;
                }
            }
        }
    }

    private void Update()
    {
        MoveStageToTheEnd();
    }

    private void MoveStageToTheEnd()
    {
        if (transform.position.z < -totalStageLength)
        {
            float errorOffset = totalStageLength + transform.position.z;
            transform.position = new Vector3(0, 0, spawnStagePoint.transform.position.z + stagePostitionOffset + errorOffset);
        }
    }
}
