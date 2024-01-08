using UnityEngine;
using static GlobalConstants;

public class StageController : MonoBehaviour
{
    private float totalStageLength;
    private Vector3 startPos;
    private float movementSpeed;
    private bool canMove;

    public void SetMovementSpeed(float speed) => movementSpeed = speed;
    public void SetMovementDisabled() => canMove = false;
    public void SetMovementEnabled() => canMove = true;

    private void Awake()
    {
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
        startPos = new Vector3(0, 0, 0);
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
        Debug.Log("Length: " + totalStageLength);
        if (transform.position.z <= startPos.z - totalStageLength)
        {
            float offset = totalStageLength * 2;
            transform.position = new Vector3(0, 0, transform.position.z + offset - CorrectiveOffset);

        }
    }
}
