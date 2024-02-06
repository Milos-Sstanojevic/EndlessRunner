using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    private event Action<EnvironmentMovementController> OnDestroyAction;
    private event Action<GunController> OnDestroyGunAction;
    private event Action<JetController> OnDestroyJetAction;
    private event Action<EnemyController> OnDestroyEnemyAction;
    private event Action<BulletController> OnBulletDestroyAction;
    private event Action<CollectableController> OnSpaceshipDestroyAction;
    private event Action<ChunkController> OnChunkDestroyAction;
    private event Action<int> OnEnemyKilledAction;
    private event Action OnPlayerDeadAction;
    private event Action OnObjectsInSceneChangedAction;
    private event Action<int> OnChangeScoreOnScreenAction;
    private event Action OnStartAddingPointsAction;
    private event Action OnStopAddingPointsAction;

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

    public void SubscribeToStopAddingPointsAction(Action action)
    {
        OnStopAddingPointsAction += action;
    }

    public void SubscribeToStartAddingPointsAction(Action action)
    {
        OnStartAddingPointsAction += action;
    }

    public void SubscribeToChangeScoreOnScreen(Action<int> action)
    {
        OnChangeScoreOnScreenAction += action;
    }

    public void SubscribeToOnDestroyAction(Action<EnvironmentMovementController> action)
    {
        OnDestroyAction += action;
    }

    public void SubscribeToOnDestroyGunAction(Action<GunController> action)
    {
        OnDestroyGunAction += action;
    }

    public void SubscribeToOnDestroyJetAction(Action<JetController> action)
    {
        OnDestroyJetAction += action;
    }

    public void SubscribeToOnDestroyEnemyAction(Action<EnemyController> action)
    {
        OnDestroyEnemyAction += action;
    }

    public void SubscribeToOnBulletDestroyAction(Action<BulletController> action)
    {
        OnBulletDestroyAction += action;
    }

    public void SubscribeToOnSpaceshipDestroyAction(Action<CollectableController> action)
    {
        OnSpaceshipDestroyAction += action;
    }

    public void SubscribeToOnChunkDestroyAction(Action<ChunkController> action)
    {
        OnChunkDestroyAction += action;
    }

    public void SubscribeToOnEnemyKilledAction(Action<int> action)
    {
        OnEnemyKilledAction += action;
    }

    public void SubscribeToOnPlayerDeadAction(Action action)
    {
        OnPlayerDeadAction += action;
    }

    public void SubscribeToOnObjectsInSceneChangedAction(Action action)
    {
        OnObjectsInSceneChangedAction += action;
    }

    public void UnsubscribeFromOnDestroyAction(Action<EnvironmentMovementController> action)
    {
        OnDestroyAction -= action;
    }

    public void UnsubscribeFromOnDestroyGunAction(Action<GunController> action)
    {
        OnDestroyGunAction -= action;
    }

    public void UnsubscribeFromOnDestroyJetAction(Action<JetController> action)
    {
        OnDestroyJetAction -= action;
    }

    public void UnsubscribeFromOnDestroyEnemyAction(Action<EnemyController> action)
    {
        OnDestroyEnemyAction -= action;
    }

    public void UnsubscribeFromOnBulletDestroyAction(Action<BulletController> action)
    {
        OnBulletDestroyAction -= action;
    }

    public void UnsubscribeFromOnSpaceshipDestroyAction(Action<CollectableController> action)
    {
        OnSpaceshipDestroyAction -= action;
    }

    public void UnsubscribeFromOnChunkDestroyAction(Action<ChunkController> action)
    {
        OnChunkDestroyAction -= action;
    }

    public void UnsubscribeFromOnEnemyKilledAction(Action<int> action)
    {
        OnEnemyKilledAction -= action;
    }

    public void UnsubscribeFromOnPlayerDeadAction(Action action)
    {
        OnPlayerDeadAction -= action;
    }

    public void UnsubscribeFromOnObjectsInSceneChangedAction(Action action)
    {
        OnObjectsInSceneChangedAction -= action;
    }

    public void UnsubscribeFromChangeScoreOnScreen(Action<int> action)
    {
        OnChangeScoreOnScreenAction -= action;
    }

    public void UnsubscribeFromStopAddingPointsAction(Action action)
    {
        OnStopAddingPointsAction -= action;
    }

    public void UnsubscribeFromStartAddingPointsAction(Action action)
    {
        OnStartAddingPointsAction -= action;
    }

    public void OnObjectsInSceneChanged()
    {
        OnObjectsInSceneChangedAction?.Invoke();
    }

    public void OnChunkDestroyed(ChunkController chunk)
    {
        OnChunkDestroyAction?.Invoke(chunk);
    }


    public void OnEnvironmentDestroyed(EnvironmentMovementController movable)
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

    public void OnChangeScoreOnScreen(int score)
    {
        OnChangeScoreOnScreenAction?.Invoke(score);
    }

    public void StartAddingPoints()
    {
        OnStartAddingPointsAction?.Invoke();
    }

    public void StopAddingPoints()
    {
        OnStopAddingPointsAction?.Invoke();
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
