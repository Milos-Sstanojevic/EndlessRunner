public class BulletPoolingSystem : BasePoolingSystem<BulletController>
{
    private void OnEnable()
    {
        EventManager.Instance.SubscribeToOnBulletDestroyAction(DestroyObjects);
    }

    private void OnDisable()
    {
        EventManager.Instance.UnsubscribeFromOnBulletDestroyAction(DestroyObjects);
    }
}
