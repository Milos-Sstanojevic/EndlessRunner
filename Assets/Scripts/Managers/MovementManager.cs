using System;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public static MovementManager Instance { get; private set; }
    private const float speedIncrease = 1f;
    private const float playerSpeedBalancer = 1;
    [SerializeField] private PlayerController player;
    [SerializeField] private List<StageController> stagesInGame;
    private List<EnvironmentMovementController> obstaclesInGame;
    private List<GunController> gunsInGame;
    private List<JetController> jetsInGame;
    private List<CollectableController> spaceshipsInGame;
    private List<EnemyController> enemiesInGame;
    private List<ChunkController> chunksInGame;
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
        obstaclesInGame = new List<EnvironmentMovementController>();
        gunsInGame = new List<GunController>();
        jetsInGame = new List<JetController>();
        spaceshipsInGame = new List<CollectableController>();
        enemiesInGame = new List<EnemyController>();

        chunksInGame = new List<ChunkController>();

        speed = 8;
    }

    private void Update()
    {
        GetCollectablesAndObstaclesInGame();
    }

    public void GetCollectablesAndObstaclesInGame()
    {
        obstaclesInGame = ObstaclesPoolingSystem.Instance.GetInstantiatedObjects();
        gunsInGame = GunPoolingSystem.Instance.GetInstantiatedObjects();
        jetsInGame = JetPoolingSystem.Instance.GetInstantiatedObjects();
        spaceshipsInGame = SpaceshipPoolingSystem.Instance.GetInstantiatedObjects();
        enemiesInGame = EnemyPoolingSystem.Instance.GetInstantiatedObjects();
        chunksInGame = ChunkPoolingSystem.Instance.GetInstantiatedObjects();
    }

    private void PerformActionOnObjects<T>(List<T> objects, Action<T> action) where T : MonoBehaviour
    {
        foreach (T obj in objects)
        {
            EnvironmentMovementController envMovement = obj.GetComponent<EnvironmentMovementController>();
            if (envMovement != null)
            {
                action.Invoke(obj);
            }
        }
    }

    public void EnableMovement()
    {
        EnableMovementForObjects(chunksInGame);
        foreach (ChunkController chunk in chunksInGame)
        {
            foreach (Transform child in chunk.transform)
            {
                EnvironmentMovementController movement = child.GetComponent<EnvironmentMovementController>();
                if (movement != null)
                {
                    movement.EnableMovement();
                }
            }
        }

        // EnableMovementForObjects(obstaclesInGame);
        // EnableMovementForObjects(gunsInGame, gun => gun.UnpauseCoroutine());
        EnableMovementForObjects(stagesInGame);
        // EnableMovementForObjects(jetsInGame, jet => jet.UnpauseCoroutine());
        // EnableMovementForObjects(spaceshipsInGame);
        // EnableMovementForObjects(enemiesInGame);

        player.EnableMovement();
    }

    private void EnableMovementForObjects<T>(List<T> objects, Action<T> additionalAction = null) where T : MonoBehaviour
    {
        PerformActionOnObjects(objects, obj =>
        {
            obj.GetComponent<EnvironmentMovementController>().EnableMovement();
            additionalAction?.Invoke(obj);
        });
    }

    public void SetMovementSpeed()
    {
        SetMovementSpeedForObjects(chunksInGame);
        // foreach (ChunkController chunk in chunksInGame)
        // {
        //     foreach (Transform child in chunk.transform)
        //     {
        //         EnvironmentMovementController movement = child.GetComponent<EnvironmentMovementController>();
        //         if (movement != null)
        //         {
        //             movement.SetMovementSpeed(speed);
        //         }
        //     }
        // }

        // SetMovementSpeedForObjects(obstaclesInGame);
        SetMovementSpeedForObjects(stagesInGame);
        // SetMovementSpeedForObjects(gunsInGame);
        // SetMovementSpeedForObjects(jetsInGame);
        // SetMovementSpeedForObjects(spaceshipsInGame);
        // SetMovementSpeedForObjects(enemiesInGame);

        player.SetMovementSpeed(speed + playerSpeedBalancer);
    }

    private void SetMovementSpeedForObjects<T>(List<T> objects) where T : MonoBehaviour
    {
        PerformActionOnObjects(objects, obj => obj.GetComponent<EnvironmentMovementController>().SetMovementSpeed(speed));
    }


    public void DisableMovementOfMovableObjects()
    {
        DisableMovementForObjects(chunksInGame);
        foreach (ChunkController chunk in chunksInGame)
        {
            foreach (Transform child in chunk.transform)
            {
                EnvironmentMovementController movement = child.GetComponent<EnvironmentMovementController>();
                if (movement != null)
                {
                    movement.DisableMovement();
                }
            }
        }

        DisableMovementForObjects(stagesInGame);
        // DisableMovementForObjects(obstaclesInGame);
        // DisableMovementForObjects(jetsInGame, jet => jet.PauseCoroutine());
        // DisableMovementForObjects(gunsInGame, gun => gun.PauseCoroutine());
        // DisableMovementForObjects(spaceshipsInGame);
        // DisableMovementForObjects(enemiesInGame);

        player.DisableMovement();
    }

    private void DisableMovementForObjects<T>(List<T> objects, Action<T> additionalAction = null) where T : MonoBehaviour
    {
        PerformActionOnObjects(objects, obj =>
        {
            obj.GetComponent<EnvironmentMovementController>().DisableMovement();
            additionalAction?.Invoke(obj);
        });
    }

    public void IncreaseMovementSpeed()
    {
        speed += speedIncrease;
        SetMovementSpeed();
    }

}
