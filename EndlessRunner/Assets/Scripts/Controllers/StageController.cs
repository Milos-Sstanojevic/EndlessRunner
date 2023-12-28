using UnityEngine;

public class StageController : MonoBehaviour
{
    private float heigthRepeat;
    private Vector3 startPos;

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
        if (GameManager.Instance.IsGameActive)
        {
            MoveStage();
            MoveStageToTheEnd();
        }
    }

    private void MoveStage()
    {
        transform.Translate(Vector3.back * GameManager.Instance.MovingSpeed * Time.deltaTime);
    }

    private void MoveStageToTheEnd()
    {
        if (transform.position.z < startPos.z - heigthRepeat)
        {
            transform.position = startPos + new Vector3(0, 0, heigthRepeat);
        }
    }
}
