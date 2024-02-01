using UnityEngine;

public class ObstaclesPoolingSystem : BasePoolingSystem<EnvironmentMovementController>
{
    private void OnEnable()
    {
        EventManager.Instance.SubscribeToOnDestroyAction(DestroyObjects);
    }

    private void OnDisable()
    {
        EventManager.Instance.UnsubscribeFromOnDestroyAction(DestroyObjects);
    }
}
