using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public static MovementManager Instance { get; private set; }
    private const float speedIncrease = 1f;
    private const float playerSpeedBalancer = 1;
    [SerializeField] private PlayerController player;
    [SerializeField] private PoolingSystem poolingSystem;
    [SerializeField] private List<StageController> stagesInGame;
    private List<EnviromentMovementBase> movabelsInGame;
    private List<CollectableBase> spaceshipsInGame;
    private List<EnviromentMovementBase> obstaclesInGame;
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
        spaceshipsInGame = new List<CollectableBase>();
        obstaclesInGame = new List<EnviromentMovementBase>();
        movabelsInGame = new List<EnviromentMovementBase>();

        speed = 8;
    }

    private void Update()
    {
        GetSpaceshipsAndObstaclesInGame();
    }

    public void GetSpaceshipsAndObstaclesInGame()
    {
        spaceshipsInGame = poolingSystem.GetInstanciatedCollectables();
        obstaclesInGame = new List<EnviromentMovementBase>();
        foreach (EnviromentMovementBase env in EnemyPoolingSystem.Instance.GetInstanciatedObstacles())
            obstaclesInGame.Add(env);

        CreateListOfMovablesInGame();
    }

    private void CreateListOfMovablesInGame()
    {
        movabelsInGame = new List<EnviromentMovementBase>();

        foreach (CollectableBase ship in spaceshipsInGame)
        {
            movabelsInGame.Add(ship);
        }
        foreach (EnviromentMovementBase obstacle in obstaclesInGame)
        {
            movabelsInGame.Add(obstacle);
        }
        foreach (StageController stage in stagesInGame)
        {
            movabelsInGame.Add(stage);
        }
    }

    public void EnableMovement()
    {
        foreach (EnviromentMovementBase movable in movabelsInGame)
        {
            movable.EnableMovement();

            if (movable is JetController jet)
            {
                jet.UnpauseCoroutine();
            }
        }
        player.EnableMovement();
    }

    public void SetMovementSpeed()
    {
        foreach (EnviromentMovementBase movable in movabelsInGame)
        {
            movable.SetMovementSpeed(speed);
        }

        player.SetMovementSpeed(speed + playerSpeedBalancer);
    }

    public void DisableMovementOfMovableObjects()
    {
        foreach (EnviromentMovementBase movable in movabelsInGame)
        {
            movable.DisableMovement();

            if (movable is JetController jet)
            {
                jet.PauseCoroutine();
            }
        }
        player.DisableMovement();
    }


    public void IncreaseMovementSpeed()
    {
        speed += speedIncrease;
        SetMovementSpeed();
    }

}
