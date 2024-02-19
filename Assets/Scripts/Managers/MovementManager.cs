using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    private const int NumberOfStagesInScene = 2;
    private const float SpeedIncrease = 1f;
    private const float PlayerSpeedBalancer = 1;
    [SerializeField] private PlayerMovement playerMovement;
    private List<ChunkController> chunksInGame;
    [SerializeField] private List<EnvironmentMovementController> objectsMovements;
    private float speed;

    private void OnEnable()
    {
        EventManager.Instance.SubscribeToOnObjectsInSceneChangedAction(GetCollectablesAndObstaclesInGame);
        EventManager.Instance.SubscribeToOnIncreaseSpeedAction(IncreaseMovementSpeed);
        EventManager.Instance.SubscribeToOnPlayerDeadAction(DisableMovementForOneScreen);
    }

    private void Start()
    {
        chunksInGame = new List<ChunkController>();
        objectsMovements = objectsMovements.Take(NumberOfStagesInScene).ToList();
        speed = 8;
    }

    public void GetCollectablesAndObstaclesInGame(SpawnManager spawnManager)
    {
        if (spawnManager == transform.parent.gameObject.GetComponentInChildren<SpawnManager>())
        {
            chunksInGame = PoolingSystemController.Instance.GetChunkPoolingSystem().GetInstantiatedObjects();
            AddObjectsInSceneToMovementList(spawnManager);
        }
    }

    private void AddObjectsInSceneToMovementList(SpawnManager spawnManager)
    {
        objectsMovements.RemoveRange(NumberOfStagesInScene, objectsMovements.Count - NumberOfStagesInScene);

        foreach (EnvironmentMovementController movement in chunksInGame.Where(chunk => chunk.GetSpawnManagerOfChunk() == spawnManager).Select(chunk => chunk.GetComponent<EnvironmentMovementController>()))
            objectsMovements.Add(movement);

    }

    public void EnableMovementOfObjects(GameObject oneScreen)
    {
        if (!playerMovement.GetComponent<PlayerController>().IsPlayerDead() && playerMovement == oneScreen.GetComponentInChildren<PlayerMovement>())
        {
            EnableMovement();
            playerMovement.EnableMovement();
        }
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

    public void SetMovementSpeedOfObjects(GameObject oneScreen)
    {
        SetMovementSpeed();
        SetMovementSpeedOfEnvironments(oneScreen.transform);
        playerMovement.SetMovementSpeed(speed + PlayerSpeedBalancer);
    }

    private void SetMovementSpeedOfEnvironments(Transform parent)
    {
        foreach (Transform child in parent)
        {
            EnvironmentMovementController environmentController = child.GetComponent<EnvironmentMovementController>();

            if (environmentController != null)
                environmentController.SetMovementSpeed(speed);

            SetMovementSpeedOfEnvironments(child);
        }
    }

    private void SetMovementSpeed()
    {
        foreach (EnvironmentMovementController movement in objectsMovements)
            movement.SetMovementSpeed(speed);
    }

    private void DisableMovementForOneScreen(PlayerController playerController)
    {
        if (playerMovement == playerController.GetComponent<PlayerMovement>())
        {
            DisableMovementOfMovableObjects(playerMovement.transform.parent.gameObject);
        }
    }

    public void DisableMovementOfMovableObjects(GameObject oneScreen)
    {
        DisableMovement();
        DisableEnvironmentMovementControllers(oneScreen.transform);
        playerMovement.DisableMovement();
    }

    private void DisableEnvironmentMovementControllers(Transform parent)
    {
        foreach (Transform child in parent)
        {
            EnvironmentMovementController environmentController = child.GetComponent<EnvironmentMovementController>();

            if (environmentController != null)
                environmentController.DisableMovement();

            DisableEnvironmentMovementControllers(child);
        }
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

    public void IncreaseMovementSpeed(GameObject oneScreen)
    {
        if (oneScreen.GetComponentInChildren<MovementManager>() == this)
        {
            speed += SpeedIncrease;
            SetMovementSpeedOfObjects(oneScreen);
        }
    }

    private void OnDisable()
    {
        EventManager.Instance.UnsubscribeFromOnObjectsInSceneChangedAction(GetCollectablesAndObstaclesInGame);
        EventManager.Instance.UnsubscribeToOnIncreaseSpeedAction(IncreaseMovementSpeed);
        EventManager.Instance.UnsubscribeFromOnPlayerDeadAction(DisableMovementForOneScreen);
    }
}
