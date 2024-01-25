using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    public event Action<CollectableBase, int> OnCollectAction;
    public event Action<EnviromentMovementBase> OnDestroyAction;
    public event Action<EnemyController> OnDestroyEnemyAction;
    public event Action<BulletController> OnBulletDestroyAction;
    public event Action<int> OnEnemyKilledAction;
    public event Action OnPlayerDeadAction;
    public event Action OnGunCollectedAction;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnCollectibleCollected(CollectableBase collectible, int pointsWorth)
    {
        OnCollectAction?.Invoke(collectible, pointsWorth);
    }

    public void OnEnviromentDestroyed(EnviromentMovementBase destroyable)
    {
        OnDestroyAction?.Invoke(destroyable);
    }

    public void OnBulletDestroyed(BulletController bullet)
    {
        OnBulletDestroyAction?.Invoke(bullet);
    }

    public void OnPlayerDead()
    {
        OnPlayerDeadAction?.Invoke();
    }

    public void OnEnemyDestroyed(EnemyController enemy)
    {
        OnDestroyEnemyAction?.Invoke(enemy);
    }

    public void OnEnemyKilled(int enemyWorth)
    {
        OnEnemyKilledAction?.Invoke(enemyWorth);
    }
}
