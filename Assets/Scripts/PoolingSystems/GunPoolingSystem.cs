
public class GunPoolingSystem : BasePoolingSystem<GunController>
{
    private void OnEnable()
    {
        EventManager.Instance.SubscribeToOnDestroyGunAction(DestroyObjects);
    }

    private void OnDisable()
    {
        EventManager.Instance.UnsubscribeFromOnDestroyGunAction(DestroyObjects);
    }
}
