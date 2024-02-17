using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    private event Action<EnvironmentMovementController> onDestroyAction;
    private event Action<GunController> onDestroyGunAction;
    private event Action<JetController> onDestroyJetAction;
    private event Action<EnemyController> onDestroyEnemyAction;
    private event Action<BulletController> onBulletDestroyAction;
    private event Action<CollectableController> onSpaceshipDestroyAction;
    private event Action<ChunkController> onChunkDestroyAction;
    private event Action<int> onEnemyKilledAction;
    private event Action onPlayerDeadAction;
    private event Action onObjectsInSceneChangedAction;
    private event Action<int, TextMeshProUGUI> onChangeScoreOnScreenAction;
    private event Action onStartAddingPointsAction;
    private event Action onStopAddingPointsAction;
    private event Action<int> onChangeNumberOfPlayersAction;
    private event Action<int> onNumberOfPlayersSavedAction;
    private event Action<GameObject[]> onNumberOfScreensChangedAction;
    private event Action<Vector3> onNewStagePositionSpawnedAction;
    private event Action<List<Canvas>> onNumberOfScoreScreensChangedAction;
    private event Action<SpawnManager> onDecreaseSpawningTimeOfChunkAction;
    private event Action<List<MovementManager>> onNumberOfMovementManagersChangedAction;
    private event Action<GameObject> onIncreaseSpeedAction;

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

    public void SubscribeToOnIncreaseSpeedAction(Action<GameObject> action)
    {
        onIncreaseSpeedAction += action;
    }

    public void SubscribeToOnNumberOfMovementManagersChanged(Action<List<MovementManager>> action)
    {
        onNumberOfMovementManagersChangedAction += action;
    }

    public void SubscribeToOnDecreaseSpawningTimeOfChunk(Action<SpawnManager> action)
    {
        onDecreaseSpawningTimeOfChunkAction += action;
    }

    public void SubscribeToNumberOfScoreScreensChangedAction(Action<List<Canvas>> action)
    {
        onNumberOfScoreScreensChangedAction += action;
    }

    public void SubscribeToOnNewStagePositionSpawnedAction(Action<Vector3> action)
    {
        onNewStagePositionSpawnedAction += action;
    }

    public void SubscribeToOnNumberOfScreensChangedAction(Action<GameObject[]> action)
    {
        onNumberOfScreensChangedAction += action;
    }

    public void SubscribeToOnNumberOfPlayersSavedAction(Action<int> action)
    {
        onNumberOfPlayersSavedAction += action;
    }


    public void SubscribeToOnChangeNumberOfPlayersAction(Action<int> action)
    {
        onChangeNumberOfPlayersAction += action;
    }

    public void SubscribeToStopAddingPointsAction(Action action)
    {
        onStopAddingPointsAction += action;
    }

    public void SubscribeToStartAddingPointsAction(Action action)
    {
        onStartAddingPointsAction += action;
    }

    public void SubscribeToChangeScoreOnScreen(Action<int, TextMeshProUGUI> action)
    {
        onChangeScoreOnScreenAction += action;
    }

    public void SubscribeToOnDestroyAction(Action<EnvironmentMovementController> action)
    {
        onDestroyAction += action;
    }

    public void SubscribeToOnDestroyGunAction(Action<GunController> action)
    {
        onDestroyGunAction += action;
    }

    public void SubscribeToOnDestroyJetAction(Action<JetController> action)
    {
        onDestroyJetAction += action;
    }

    public void SubscribeToOnDestroyEnemyAction(Action<EnemyController> action)
    {
        onDestroyEnemyAction += action;
    }

    public void SubscribeToOnBulletDestroyAction(Action<BulletController> action)
    {
        onBulletDestroyAction += action;
    }

    public void SubscribeToOnSpaceshipDestroyAction(Action<CollectableController> action)
    {
        onSpaceshipDestroyAction += action;
    }

    public void SubscribeToOnChunkDestroyAction(Action<ChunkController> action)
    {
        onChunkDestroyAction += action;
    }

    public void SubscribeToOnEnemyKilledAction(Action<int> action)
    {
        onEnemyKilledAction += action;
    }

    public void SubscribeToOnPlayerDeadAction(Action action)
    {
        onPlayerDeadAction += action;
    }

    public void SubscribeToOnObjectsInSceneChangedAction(Action action)
    {
        onObjectsInSceneChangedAction += action;
    }

    public void UnsubscribeFromOnDestroyAction(Action<EnvironmentMovementController> action)
    {
        onDestroyAction -= action;
    }

    public void UnsubscribeFromOnDestroyGunAction(Action<GunController> action)
    {
        onDestroyGunAction -= action;
    }

    public void UnsubscribeFromOnDestroyJetAction(Action<JetController> action)
    {
        onDestroyJetAction -= action;
    }

    public void UnsubscribeFromOnDestroyEnemyAction(Action<EnemyController> action)
    {
        onDestroyEnemyAction -= action;
    }

    public void UnsubscribeFromOnBulletDestroyAction(Action<BulletController> action)
    {
        onBulletDestroyAction -= action;
    }

    public void UnsubscribeFromOnSpaceshipDestroyAction(Action<CollectableController> action)
    {
        onSpaceshipDestroyAction -= action;
    }

    public void UnsubscribeFromOnChunkDestroyAction(Action<ChunkController> action)
    {
        onChunkDestroyAction -= action;
    }

    public void UnsubscribeFromOnEnemyKilledAction(Action<int> action)
    {
        onEnemyKilledAction -= action;
    }

    public void UnsubscribeFromOnPlayerDeadAction(Action action)
    {
        onPlayerDeadAction -= action;
    }

    public void UnsubscribeFromOnObjectsInSceneChangedAction(Action action)
    {
        onObjectsInSceneChangedAction -= action;
    }

    public void UnsubscribeFromChangeScoreOnScreen(Action<int, TextMeshProUGUI> action)
    {
        onChangeScoreOnScreenAction -= action;
    }

    public void UnsubscribeFromStopAddingPointsAction(Action action)
    {
        onStopAddingPointsAction -= action;
    }

    public void UnsubscribeFromStartAddingPointsAction(Action action)
    {
        onStartAddingPointsAction -= action;
    }

    public void UnsubscribeFromOnChangeNumberOfPlayersAction(Action<int> action)
    {
        onChangeNumberOfPlayersAction -= action;
    }

    public void UnsubscribeFromOnNumberOfPlayersSavedAction(Action<int> action)
    {
        onNumberOfPlayersSavedAction -= action;
    }

    public void UnsubscribeFromOnNumberOfScreensChangedAction(Action<GameObject[]> action)
    {
        onNumberOfScreensChangedAction -= action;
    }

    public void UnsubscribeFromOnNewStagePositionSpawnedAction(Action<Vector3> action)
    {
        onNewStagePositionSpawnedAction -= action;
    }

    public void UnsubscribeFromAddScoreScreenAction(Action<List<Canvas>> action)
    {
        onNumberOfScoreScreensChangedAction -= action;
    }

    public void UnsubscribeToOnDecreaseSpawningTimeOfChunk(Action<SpawnManager> action)
    {
        onDecreaseSpawningTimeOfChunkAction -= action;
    }

    public void UnsubscribeToOnNumberOfMovementManagersChanged(Action<List<MovementManager>> action)
    {
        onNumberOfMovementManagersChangedAction -= action;
    }

    public void UnsubscribeToOnIncreaseSpeedAction(Action<GameObject> action)
    {
        onIncreaseSpeedAction -= action;
    }

    public void OnNumberOfScreensChanged(GameObject[] screens)
    {
        onNumberOfScreensChangedAction?.Invoke(screens);
    }

    public void OnNewStagePositionSpawned(Vector3 position)
    {
        onNewStagePositionSpawnedAction?.Invoke(position);
    }

    public void OnObjectsInSceneChanged()
    {
        onObjectsInSceneChangedAction?.Invoke();
    }

    public void OnChunkDestroyed(ChunkController chunk)
    {
        onChunkDestroyAction?.Invoke(chunk);
    }

    public void OnNumberOfPlayersChanged(int number)
    {
        onChangeNumberOfPlayersAction?.Invoke(number);
    }

    public void OnNumberOfPlayersSaved(int number)
    {
        onNumberOfPlayersSavedAction?.Invoke(number);
    }
    public void OnEnvironmentDestroyed(EnvironmentMovementController movable)
    {
        onDestroyAction?.Invoke(movable);
    }

    public void OnGunDestroyed(GunController gun)
    {
        onDestroyGunAction?.Invoke(gun);
    }

    public void OnJetDestroyed(JetController jet)
    {
        onDestroyJetAction?.Invoke(jet);
    }

    public void OnSpaceshipDestroyed(CollectableController spaceship)
    {
        onSpaceshipDestroyAction?.Invoke(spaceship);
    }

    public void OnBulletDestroyed(BulletController bullet)
    {
        onBulletDestroyAction?.Invoke(bullet);
    }

    public void OnChangeScoreOnScreen(int score, TextMeshProUGUI playerScoreText)
    {
        onChangeScoreOnScreenAction?.Invoke(score, playerScoreText);
    }

    public void StartAddingPoints()
    {
        onStartAddingPointsAction?.Invoke();
    }

    public void StopAddingPoints()
    {
        onStopAddingPointsAction?.Invoke();
    }

    public void OnPlayerDead()
    {
        onPlayerDeadAction?.Invoke();
    }

    public void OnEnemyDestroyed(EnemyController enemy)
    {
        onDestroyEnemyAction?.Invoke(enemy);
    }

    public void OnEnemyKilled(int enemyWorth)
    {
        onEnemyKilledAction?.Invoke(enemyWorth);
    }

    public void OnNumberOfScoreScreensChanged(List<Canvas> canvas)
    {
        onNumberOfScoreScreensChangedAction?.Invoke(canvas);
    }

    public void OnDecreaseSpawningTimeOfChunk(SpawnManager spawnManager)
    {
        onDecreaseSpawningTimeOfChunkAction?.Invoke(spawnManager);
    }

    public void OnNumberOfMovementManagersChanged(List<MovementManager> movementManagers)
    {
        onNumberOfMovementManagersChangedAction?.Invoke(movementManagers);
    }

    public void OnIncreaseSpeed(GameObject screen)
    {
        onIncreaseSpeedAction?.Invoke(screen);
    }
}
