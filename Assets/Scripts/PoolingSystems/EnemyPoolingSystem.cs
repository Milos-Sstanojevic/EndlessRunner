
using UnityEngine;

public class EnemyPoolingSystem : BasePoolingSystem<EnemyController>
{
    private void OnEnable()
    {
        EventManager.Instance.SubscribeToOnDestroyEnemyAction(DestroyObjects);
    }

    private void OnDisable()
    {
        EventManager.Instance.UnsubscribeFromOnDestroyEnemyAction(DestroyObjects);
    }
}
