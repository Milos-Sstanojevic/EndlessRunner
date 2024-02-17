using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    private const int NumberOfStagesInScene = 2;
    private const float SpeedIncrease = 1f;
    private const float PlayerSpeedBalancer = 1;
    [SerializeField] private PlayerMovement player;
    private List<ChunkController> chunksInGame;
    [SerializeField] private List<EnvironmentMovementController> objectsMovements;
    private float speed;

    private void OnEnable()
    {
        EventManager.Instance.SubscribeToOnObjectsInSceneChangedAction(GetCollectablesAndObstaclesInGame);
        EventManager.Instance.SubscribeToOnIncreaseSpeedAction(IncreaseMovementSpeed);
    }

    private void Start()
    {
        chunksInGame = new List<ChunkController>();
        GetCollectablesAndObstaclesInGame();
        speed = 8;
    }

    public void GetCollectablesAndObstaclesInGame()
    {
        chunksInGame = PoolingSystemController.Instance.GetChunkPoolingSystem().GetInstantiatedObjects();
        AddObjectsInSceneToMovementList();
    }

    private void AddObjectsInSceneToMovementList()
    {
        objectsMovements = objectsMovements.Take(NumberOfStagesInScene).ToList();

        foreach (EnvironmentMovementController movement in chunksInGame.Select(chunk => chunk.GetComponent<EnvironmentMovementController>()))
            objectsMovements.Add(movement);
    }

    public void EnableMovementOfObjects(GameObject oneScreen)
    {
        EnableMovement();
        EnableEnvironmentMovementControllers(oneScreen.transform);
        player.EnableMovement();
    }

    private void EnableEnvironmentMovementControllers(Transform parent)
    {
        foreach (Transform child in parent)
        {
            EnvironmentMovementController environmentController = child.GetComponent<EnvironmentMovementController>();

            if (environmentController != null)
                environmentController.EnableMovement();

            EnableEnvironmentMovementControllers(child);
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
        player.SetMovementSpeed(speed + PlayerSpeedBalancer);
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

    public void DisableMovementOfMovableObjects(GameObject oneScreen)
    {
        DisableMovement();
        DisableEnvironmentMovementControllers(oneScreen.transform);
        player.DisableMovement();
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
    }
}
