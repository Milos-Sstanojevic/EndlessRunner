using UnityEngine;

public class StageController : ObjectManager
{
    private const float StagePostitionOffset = 14.91158f;
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
            if (child.CompareTag("Ground"))
            {
                MeshCollider collider = child.GetComponent<MeshCollider>();

                if (collider != null)
                {
                    totalStageLength += collider.bounds.size.z;
                }
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        MoveStageToTheEnd();
    }

    private void MoveStageToTheEnd()
    {
        if (transform.position.z < -totalStageLength)
        {
            float errorOffset = totalStageLength + transform.position.z;
            transform.position = new Vector3(0, 0, spawnStagePoint.transform.position.z + StagePostitionOffset + errorOffset);
        }
    }
}
