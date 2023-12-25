using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    private float heigthRepeat;
    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        if (IsItStage2())
        {
            startPos = new Vector3(0, -0.05f, 0); //nerviralo me treperenje 
        }
        else
        {
            startPos = Vector3.zero;
        }
        heigthRepeat = GetComponent<BoxCollider>().size.z;
    }

    bool IsItStage2()
    {
        return gameObject.CompareTag("1");      //ako nije stage 2 onda je stage 1, ovo treba podesiti kad napravim vise stage-ova koji se menjaju.
    }

    // Update is called once per frame
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
