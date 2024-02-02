using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public static MovementManager Instance { get; private set; }
    private const float SpeedIncrease = 1f;
    private const float PlayerSpeedBalancer = 1;
    [SerializeField] private PlayerController player;
    private List<ChunkController> chunksInGame;
    [SerializeField] private List<EnvironmentMovementController> objectsMovements;
    private float speed;

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

    private void OnEnable()
    {
        EventManager.Instance.SubscribeToOnObjectsInSceneChangedAction(GetCollectablesAndObstaclesInGame);
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
        objectsMovements = objectsMovements.Take(2).ToList();

        foreach (EnvironmentMovementController movement in chunksInGame.Select(chunk => chunk.GetComponent<EnvironmentMovementController>()))
            objectsMovements.Add(movement);
    }

    public void EnableMovementOfObjects()
    {
        EnableMovement();
        player.EnableMovement();
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

    public void SetMovementSpeedOfObjects()
    {
        SetMovementSpeed();
        player.SetMovementSpeed(speed + PlayerSpeedBalancer);
    }

    private void SetMovementSpeed()
    {
        foreach (EnvironmentMovementController movement in objectsMovements)
            movement.SetMovementSpeed(speed);
    }

    public void DisableMovementOfMovableObjects()
    {
        DisableMovement();
        player.DisableMovement();
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

    public void IncreaseMovementSpeed()
    {
        speed += SpeedIncrease;
        SetMovementSpeedOfObjects();
    }

    private void OnDisable()
    {
        EventManager.Instance.UnsubscribeFromOnObjectsInSceneChangedAction(GetCollectablesAndObstaclesInGame);
    }
}
