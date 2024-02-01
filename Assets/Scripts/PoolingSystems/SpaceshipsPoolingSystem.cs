
public class SpaceshipPoolingSystem : BasePoolingSystem<CollectableController>
{
    private void OnEnable()
    {
        EventManager.Instance.SubscribeToOnSpaceshipDestroyAction(DestroyObjects);
    }

    private void OnDisable()
    {
        EventManager.Instance.UnsubscribeFromOnSpaceshipDestroyAction(DestroyObjects);
    }
}
