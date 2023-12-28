using System;
using UnityEngine;
using static GlobalConstants;
public class ObstacleController : MonoBehaviour, IDestroyable
{
    public static event Action<ObstacleController> OnDestroyObstacle;

    protected virtual void Update()
    {
        if (GameManager.Instance.IsGameActive)
        {
            MoveObstacle();
        }
    }

    void MoveObstacle()
    {
        transform.Translate(Vector3.back * GameManager.Instance.MovingSpeed * Time.deltaTime, Space.World);

        if (transform.position.z < PositionBehindPlayerAxisZ)//ako prodje iza player-a pokreni akciju koja ce uraditi Release u pool za obstacle
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        OnDestroyObstacle?.Invoke(this);
    }
}
