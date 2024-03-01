using System;
using System.Collections.Generic;
using Fusion;
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
    private event Action<int, PlayerController, GameObject> onPlayerDeadAction;
    private event Action<SpawnManager> onObjectsInSceneChangedAction;
    private event Action<int, TextMeshProUGUI> onChangeScoreOnScreenAction;
    private event Action onStartAddingPointsAction;
    private event Action onStopAddingPointsAction;
    private event Action<int> onChangeNumberOfPlayersAction;
    private event Action onNumberOfPlayersSavedAction;
    private event Action<OneScreenController[]> onNumberOfScreensChangedAction;
    private event Action<Vector3> onNewStagePositionSpawnedAction;
    private event Action<List<Canvas>> onNumberOfScoreScreensChangedAction;
    private event Action<SpawnManager> onDecreaseSpawningTimeOfChunkAction;
    private event Action<List<MovementManager>> onNumberOfMovementManagersChangedAction;
    private event Action<OneScreenController> onIncreaseSpeedAction;
    private event Action onNumberOfPlayersChosenAction;
    private event Action<int> onLoadNumberOfPlayersAction;
    private event Action<GameObject> onEnableMovementOfObject;
    private event Action onRestartButtonClickedAction;
    private event Action onExitButtonClickedAction;
    private event Action onSettingsButtonClickedAction;


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

    public void SubscribeToOnSettingsButtonClickedAction(Action action)
    {
        onSettingsButtonClickedAction += action;
    }

    public void SubscribeToOnExitButtonClickedAction(Action action)
    {
        onExitButtonClickedAction += action;
    }

    public void SubscribeToOnRestartButtonClickedAction(Action action)
    {
        onRestartButtonClickedAction += action;
    }

    public void SubscribeToOnEnableMovementOfObject(Action<GameObject> action)
    {
        onEnableMovementOfObject += action;
    }

    public void SubscribeToOnNumberOfPlayersChosen(Action action)
    {
        onNumberOfPlayersChosenAction += action;
    }

    public void SubscribeToOnLoadNumberOfPlayersFromStart(Action<int> action)
    {
        onLoadNumberOfPlayersAction += action;
    }

    public void SubscribeToOnIncreaseSpeedAction(Action<OneScreenController> action)
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

    public void SubscribeToOnNumberOfScreensChangedAction(Action<OneScreenController[]> action)
    {
        onNumberOfScreensChangedAction += action;
    }

    public void SubscribeToOnNumberOfPlayersSavedAction(Action action)
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

    public void SubscribeToOnPlayerDeadAction(Action<int, PlayerController, GameObject> action)
    {
        onPlayerDeadAction += action;
    }

    public void SubscribeToOnObjectsInSceneChangedAction(Action<SpawnManager> action)
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

    public void UnsubscribeFromOnPlayerDeadAction(Action<int, PlayerController, GameObject> action)
    {
        onPlayerDeadAction -= action;
    }

    public void UnsubscribeFromOnObjectsInSceneChangedAction(Action<SpawnManager> action)
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

    public void UnsubscribeFromOnNumberOfPlayersSavedAction(Action action)
    {
        onNumberOfPlayersSavedAction -= action;
    }

    public void UnsubscribeFromOnNumberOfScreensChangedAction(Action<OneScreenController[]> action)
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

    public void UnsubscribeToOnIncreaseSpeedAction(Action<OneScreenController> action)
    {
        onIncreaseSpeedAction -= action;
    }

    public void UnsubscribeFromOnNumberOfPlayersChosen(Action action)
    {
        onNumberOfPlayersChosenAction -= action;
    }

    public void UnsubscribeFromOnLoadNumberOfPlayersFromStart(Action<int> action)
    {
        onLoadNumberOfPlayersAction -= action;
    }

    public void UnsubscribeFromOnEnableMovementOfObject(Action<GameObject> action)
    {
        onEnableMovementOfObject -= action;
    }

    public void UnsubscribeFromOnSettingsButtonClickedAction(Action action)
    {
        onSettingsButtonClickedAction -= action;
    }

    public void UnsubscribeFromOnExitButtonClickedAction(Action action)
    {
        onExitButtonClickedAction -= action;
    }

    public void UnsubscribeFromOnRestartButtonClickedAction(Action action)
    {
        onRestartButtonClickedAction -= action;
    }

    public void OnNumberOfPlayersChosen()
    {
        onNumberOfPlayersChosenAction?.Invoke();
    }

    public void OnNumberOfScreensChanged(OneScreenController[] screens)
    {
        onNumberOfScreensChangedAction?.Invoke(screens);
    }

    public void OnNewStagePositionSpawned(Vector3 position)
    {
        onNewStagePositionSpawnedAction?.Invoke(position);
    }

    public void OnObjectsInSceneChanged(SpawnManager spawnManager)
    {
        onObjectsInSceneChangedAction?.Invoke(spawnManager);
    }

    public void OnChunkDestroyed(ChunkController chunk)
    {
        onChunkDestroyAction?.Invoke(chunk);
    }

    public void OnNumberOfPlayersChanged(int number)
    {
        onChangeNumberOfPlayersAction?.Invoke(number);
    }

    public void OnNumberOfPlayersSaved()
    {
        onNumberOfPlayersSavedAction?.Invoke();
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

    public void OnPlayerDead(int id, PlayerController player, GameObject gameOverScreen)
    {
        onPlayerDeadAction?.Invoke(id, player, gameOverScreen);
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

    public void OnIncreaseSpeed(OneScreenController screen)
    {
        onIncreaseSpeedAction?.Invoke(screen);
    }

    public void OnLoadNumberOfPlayers(int numberOfPlayers)
    {
        onLoadNumberOfPlayersAction?.Invoke(numberOfPlayers);
    }

    public void OnEnableMovementOfObject(GameObject gameObject)
    {
        onEnableMovementOfObject?.Invoke(gameObject);
    }

    public void OnRestartButtonClicked()
    {
        onRestartButtonClickedAction?.Invoke();
    }
    public void OnExitButtonClicked()
    {
        onExitButtonClickedAction?.Invoke();
    }
    public void OnSettingsButtonClicked()
    {
        onSettingsButtonClickedAction?.Invoke();
    }
}
