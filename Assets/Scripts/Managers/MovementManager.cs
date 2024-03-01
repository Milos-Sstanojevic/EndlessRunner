using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Fusion;
using UnityEngine;

public class MovementManager : NetworkBehaviour
{
    private const int NumberOfStagesInScene = 2;
    private const float SpeedIncrease = 1f;
    private const float PlayerSpeedBalancer = 1;
    private PlayerMovement playerMovement;
    private List<ChunkController> chunksInGame;
    [SerializeField] private List<EnvironmentMovementController> objectsMovements;
    private float speed;

    private void OnEnable()
    {
        EventManager.Instance.SubscribeToOnObjectsInSceneChangedAction(GetCollectablesAndObstaclesInGame);
        EventManager.Instance.SubscribeToOnIncreaseSpeedAction(IncreaseMovementSpeed);
        EventManager.Instance.SubscribeToOnPlayerDeadAction(DisableMovementForOneScreen);
        EventManager.Instance.SubscribeToOnEnableMovementOfObject(EnableMovementOfObject);
    }

    private void Start()
    {
        chunksInGame = new List<ChunkController>();
        objectsMovements = objectsMovements.Take(NumberOfStagesInScene).ToList();
        speed = 8;
    }

    private void EnableMovementOfObject(GameObject gameObject)
    {
        gameObject.GetComponent<EnvironmentMovementController>().EnableMovement();
        gameObject.GetComponent<EnvironmentMovementController>().SetMovementSpeed(speed);
    }

    public void GetCollectablesAndObstaclesInGame(SpawnManager spawnManager)
    {
        if (spawnManager != transform.parent.gameObject.GetComponentInChildren<SpawnManager>())
            return;

        chunksInGame = PoolingSystemController.Instance.GetChunkPoolingSystem().GetInstantiatedObjects();
        AddObjectsInSceneToMovementList(spawnManager);
    }

    private void AddObjectsInSceneToMovementList(SpawnManager spawnManager)
    {
        objectsMovements.RemoveRange(NumberOfStagesInScene, objectsMovements.Count - NumberOfStagesInScene);

        List<EnvironmentMovementController> selectedMovements = chunksInGame
                            .Where(chunk => chunk.GetSpawnManagerOfChunk() == spawnManager)
                            .Select(chunk => chunk.GetComponent<EnvironmentMovementController>())
                            .ToList();

        foreach (EnvironmentMovementController movement in selectedMovements)
            objectsMovements.Add(movement);
    }

    public void EnableMovementOfObjects(OneScreenController oneScreen)
    {
        if (playerMovement.GetComponent<PlayerController>().IsPlayerDead() || playerMovement != oneScreen.GetPlayerControllerInOneScreen().GetPlayerMovementComponentOfPlayer())
            return;

        EnableMovement();
        playerMovement.EnableMovement();
    }

    private void EnableMovement()
    {
        foreach (EnvironmentMovementController movement in objectsMovements)
        {
            movement.EnableMovement();
            EnableMovementOfChildrenInChunk(movement);
        }
    }

    private void EnableMovementOfChildrenInChunk(EnvironmentMovementController chunk)
    {
        EnvironmentMovementController[] objectsInChunk = chunk.GetComponentsInChildren<EnvironmentMovementController>();

        foreach (EnvironmentMovementController movementInChunk in objectsInChunk)
            movementInChunk.EnableMovement();
    }

    public void SetMovementSpeedOfObjects(OneScreenController oneScreen)
    {
        SetMovementSpeed();
        SetMovementSpeedOfEnvironments(oneScreen);
        playerMovement.SetMovementSpeed(speed + PlayerSpeedBalancer);
    }

    private void SetMovementSpeedOfEnvironments(OneScreenController oneScreen)
    {
        if (oneScreen == null)
            return;

        foreach (EnvironmentMovementController movable in oneScreen.GetEnvironmentMovementControllersInOneScreen())
        {
            if (movable != null)
            {
                movable.SetMovementSpeed(speed);
            }
        }
    }

    private void SetMovementSpeed()
    {
        foreach (EnvironmentMovementController movement in objectsMovements)
            movement.SetMovementSpeed(speed);
    }

    private void DisableMovementForOneScreen(int id, PlayerController playerController, GameObject endScreen)
    {
        if (playerMovement == playerController.GetPlayerMovementComponentOfPlayer() && playerMovement.GetComponent<PlayerController>().GetPlayerId() == id)
            DisableMovementOfMovableObjects(playerController.GetScreenOfPlayer());
    }

    public void DisableMovementOfMovableObjects(OneScreenController oneScreen)
    {
        DisableMovement();
        DisableEnvironmentMovementControllers(oneScreen);
        playerMovement.DisableMovement();
    }

    private void DisableEnvironmentMovementControllers(OneScreenController oneScreen)
    {
        if (oneScreen != null)
            foreach (EnvironmentMovementController movable in oneScreen.GetEnvironmentMovementControllersInOneScreen())
                movable.DisableMovement();
    }

    private void DisableMovement()
    {
        foreach (EnvironmentMovementController movement in objectsMovements)
        {
            movement.DisableMovement();
            DisableMovemenOfChildrenInChunk(movement);
        }
    }

    private void DisableMovemenOfChildrenInChunk(EnvironmentMovementController chunk)
    {
        EnvironmentMovementController[] objectsInChunk = chunk.GetComponentsInChildren<EnvironmentMovementController>();

        foreach (EnvironmentMovementController movementInChunk in objectsInChunk)
            movementInChunk.DisableMovement();
    }

    public void IncreaseMovementSpeed(OneScreenController oneScreen)
    {
        if (oneScreen.GetMovementManagerInOneScreen() != this)
            return;

        speed += SpeedIncrease;
        SetMovementSpeedOfObjects(oneScreen);
    }


    public void SetPlayerMovementComponentOnScreen(PlayerMovement player)
    {
        playerMovement = player;
    }

    private void OnDisable()
    {
        EventManager.Instance.UnsubscribeFromOnObjectsInSceneChangedAction(GetCollectablesAndObstaclesInGame);
        EventManager.Instance.UnsubscribeToOnIncreaseSpeedAction(IncreaseMovementSpeed);
        EventManager.Instance.UnsubscribeFromOnPlayerDeadAction(DisableMovementForOneScreen);
        EventManager.Instance.UnsubscribeFromOnEnableMovementOfObject(EnableMovementOfObject);
    }
}
