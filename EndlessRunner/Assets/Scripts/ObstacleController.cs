using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private Action<ObstacleController> destroyObstacle;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsGameActive)
        {
            MoveObstacle();
        }
    }

    void MoveObstacle()
    {
        transform.Translate(Vector3.left * GameManager.Instance.MovingSpeed * Time.deltaTime);

        if (transform.position.z < -12)//ako prodje iza player-a pokreni akciju koja ce uraditi Release u pool za obstacle
        {
            destroyObstacle(this);
        }
    }

    public void Init(Action<ObstacleController> action)
    {
        destroyObstacle = action;
    }
}
