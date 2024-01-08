using UnityEngine;

public class StageController : MonoBehaviour
{
    private float heigthRepeat;
    private Vector3 startPos;
    private float movementSpeed;
    private bool canMove;

    public void SetMovementSpeed(float speed) => movementSpeed = speed;
    public void SetMovementDisabled() => canMove = false;
    public void SetMovementEnabled() => canMove = true;

    private void Awake()
    {
        heigthRepeat = GetComponent<BoxCollider>().size.z;
    }

    private void Start()
    {
        startPos = Vector3.zero;
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
        if (transform.position.z < startPos.z - heigthRepeat)
        {
            transform.position = startPos + new Vector3(0, 0, heigthRepeat);
        }
    }
}
