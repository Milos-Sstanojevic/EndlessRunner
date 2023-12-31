using UnityEngine;
using static GlobalConstants;

public class StageController : MonoBehaviour
{
    private float totalStageLength;
    private float movementSpeed;
    private bool canMove;
    private GameObject spawnStagePoint;
    public void SetMovementSpeed(float speed) => movementSpeed = speed;
    public void SetMovementDisabled() => canMove = false;
    public void SetMovementEnabled() => canMove = true;

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

    private void Start()
    {
    }

    private void Update()
    {
        MoveStage();
        MoveStageToTheEnd();
    }

    private void MoveStage()
    {
        if (canMove)
        {
            transform.Translate(Vector3.back * movementSpeed * Time.deltaTime);
        }
    }

    private void MoveStageToTheEnd()
    {
        if (transform.position.z < -totalStageLength)
        {
            transform.position = new Vector3(0, 0, spawnStagePoint.transform.position.z + StagePostitionOffset);
        }
    }
}
