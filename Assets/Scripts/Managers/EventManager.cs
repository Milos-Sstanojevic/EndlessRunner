using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    public event Action<CollectableController, int> OnCollectAction;
    public event Action<EnvironmentMovementController> OnDestroyAction;
    public event Action<GunController> OnDestroyGunAction;
    public event Action<JetController> OnDestroyJetAction;
    public event Action<EnemyController> OnDestroyEnemyAction;
    public event Action<BulletController> OnBulletDestroyAction;
    public event Action<CollectableController> OnSpaceshipDestroyAction;
    public event Action<int> OnEnemyKilledAction;
    public event Action OnPlayerDeadAction;

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

    public void OnCollectibleCollected(CollectableController collectible, int pointsWorth)
    {
        OnCollectAction?.Invoke(collectible, pointsWorth);
    }

    public void OnEnviromentDestroyed(EnvironmentMovementController movable)
    {
        OnDestroyAction?.Invoke(movable);
    }

    public void OnGunDestroyed(GunController gun)
    {
        OnDestroyGunAction?.Invoke(gun);
    }

    public void OnJetDestroyed(JetController jet)
    {
        OnDestroyJetAction?.Invoke(jet);
    }

    public void OnSpaceshipDestroyed(CollectableController spaceship)
    {
        OnSpaceshipDestroyAction?.Invoke(spaceship);
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
