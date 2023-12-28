using UnityEngine;

public class StageController : MonoBehaviour
{
    private float heigthRepeat;
    private Vector3 startPos;

    private void Awake()
    {
        heigthRepeat = GetComponent<BoxCollider>().size.z;
    }

    void Start()
    {
        if (IsItStage2())
        {
            startPos = new Vector3(0, -0.05f, 0);
        }
        else
        {
            startPos = Vector3.zero;
        }
    }

    bool IsItStage2()
    {
        return gameObject.CompareTag("1");      //ako nije stage 2 onda je stage 1, ovo treba podesiti kad napravim vise stage-ova koji se menjaju.
    }

    void Update()
    {
        if (GameManager.Instance.IsGameActive)
        {
            MoveStage();
            MoveStageToTheEnd();
        }
    }

    void MoveStage()
    {
        transform.Translate(Vector3.back * GameManager.Instance.MovingSpeed * Time.deltaTime);
    }

    void MoveStageToTheEnd()
    {
        if (transform.position.z < startPos.z - heigthRepeat)
        {
            transform.position = startPos + new Vector3(0, 0, heigthRepeat);
        }
    }
}
