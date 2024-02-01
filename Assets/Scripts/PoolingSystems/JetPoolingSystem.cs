
public class JetPoolingSystem : BasePoolingSystem<JetController>
{
    private void OnEnable()
    {
        EventManager.Instance.SubscribeToOnDestroyJetAction(DestroyObjects);
    }

    private void OnDisable()
    {
        EventManager.Instance.SubscribeToOnDestroyJetAction(DestroyObjects);
    }
}
