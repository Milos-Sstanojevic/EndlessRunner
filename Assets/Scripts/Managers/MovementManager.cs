using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public static MovementManager Instance { get; private set; }
    private const float speedIncrease = 1f;
    private const float playerSpeedBalancer = 1;
    [SerializeField] private PlayerController player;
    [SerializeField] private List<StageController> stagesInGame;
    private List<ChunkController> chunksInGame;
    private List<EnvironmentMovementController> objectsMovements;
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

    private void Start()
    {
        chunksInGame = new List<ChunkController>();
        objectsMovements = new List<EnvironmentMovementController>();


        speed = 8;
    }

    private void Update()
    {
        GetCollectablesAndObstaclesInGame();
    }

    public void GetCollectablesAndObstaclesInGame()
    {
        chunksInGame = ChunkPoolingSystem.Instance.GetInstantiatedObjects();
        AddObjectsInSceneToMovementList();
    }

    private void AddObjectsInSceneToMovementList()
    {
        objectsMovements.Clear();
        foreach (EnvironmentMovementController movement in chunksInGame.Select(chunk => chunk.GetComponent<EnvironmentMovementController>()))
            objectsMovements.Add(movement);

        foreach (EnvironmentMovementController movement in stagesInGame.Select(stage => stage.GetComponent<EnvironmentMovementController>()))
            objectsMovements.Add(movement);
    }

    public void EnableMovementOfObjects()
    {
        EnableMovement();
        EnableMovementOfChildrenInChunk();

        player.EnableMovement();
    }

    private void EnableMovement()
    {
        foreach (EnvironmentMovementController movement in objectsMovements)
        {
            movement.EnableMovement();
        }
    }


    private void EnableMovementOfChildrenInChunk()
    {
        foreach (ChunkController chunk in chunksInGame)
        {
            EnvironmentMovementController[] objectsInChunk = chunk.GetComponentsInChildren<EnvironmentMovementController>();
            foreach (EnvironmentMovementController movementInChunk in objectsInChunk)
            {
                movementInChunk.EnableMovement();
            }
        }
    }

    public void SetMovementSpeedOfObjects()
    {
        SetMovementSpeed();
        player.SetMovementSpeed(speed + playerSpeedBalancer);
    }

    private void SetMovementSpeed()
    {
        foreach (EnvironmentMovementController movement in objectsMovements)
            movement.SetMovementSpeed(speed);
    }

    public void DisableMovementOfMovableObjects()
    {
        DisableMovement();
        DisableMovemenOfChildrenInChunk();
        player.DisableMovement();
    }

    private void DisableMovement()
    {
        foreach (EnvironmentMovementController movement in objectsMovements)
        {
            movement.DisableMovement();
        }
    }

    private void DisableMovemenOfChildrenInChunk()
    {
        foreach (ChunkController chunk in chunksInGame)
        {
            EnvironmentMovementController[] objectsInChunk = chunk.GetComponentsInChildren<EnvironmentMovementController>();
            foreach (EnvironmentMovementController movementInChunk in objectsInChunk)
            {
                movementInChunk.DisableMovement();
            }
        }
    }

    public void IncreaseMovementSpeed()
    {
        speed += speedIncrease;
        SetMovementSpeedOfObjects();
    }
}
