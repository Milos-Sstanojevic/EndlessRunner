using UnityEngine;

public class StageSpawnPointController : MonoBehaviour
{
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
        EventManager.Instance.OnNewStagePositionSpawned(startPosition);
    }

}
