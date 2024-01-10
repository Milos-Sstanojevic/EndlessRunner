
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private const int ScoreStep = 100;
    private const float SpeedIncrease = 1f;
    private const float MinimumSpaceshipSpawningDelay = 0.3f;
    private const float MinimumObstacleSpawningDelay = 0.2f;
    private const float SpawningDelayDecreaser = 0.05f;
    private const float AddPointsDelay = 0.5f;
    private const int OneScorePoint = 1;
    private const int ShipsWorth = 20;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private PlayerController player;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private PoolingSystem poolingSystem;
    [SerializeField] private List<StageController> stagesInGame;
    private List<MovementManager> movabelsInGame;
    // [SerializeField] private MovementManager movementManager;
    private List<SpaceshipController> spaceshipsInGame;
    private List<ObstacleController> obstaclesInGame;
    private int speedupRound = 1;
    private GameStates CurrentState;
    public float MovingSpeed { get; private set; }


    private void Start()
    {
        MovingSpeed = 8;
        spaceshipsInGame = new List<SpaceshipController>();
        obstaclesInGame = new List<ObstacleController>();
        movabelsInGame = new List<MovementManager>();
    }

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

        SetGameState(GameStates.MainMenu);
    }

    private void SetGameState(GameStates state)
    {
        CurrentState = state;
    }

    private void OnEnable()
    {
        SubscribeToCollectAction();
        SubscribeToDeadPlayerAction();
    }

    private void SubscribeToDeadPlayerAction()
    {
        EventManager.Instance.OnPlayerDeadAction += GameOver;
    }

    private void SubscribeToCollectAction()
    {
        EventManager.Instance.OnCollectAction += CollectSpaceship;
    }

    //Unity event, called when player is dead
    public void GameOver()
    {
        SetGameState(GameStates.GameOver);
        uiManager.SetEndScreenActive();
        uiManager.SetScoreScreenInactive();
    }

    public void CollectSpaceship(ICollectible collectible)
    {
        uiManager.SetScoreOnScoreScreen(ShipsWorth);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentState == GameStates.Paused)
                ResumeGame();
            else
                PauseGame();
        }

        HandlePlayingState();
        HandlePauseOrGameOverState();
        SpeedupGame();
    }

    public void PauseGame()
    {
        if (CurrentState == GameStates.Playing)
        {
            SetGameState(GameStates.Paused);
            uiManager.SetPauseScreenActive();
        }
    }

    //Bind with Unity event, on continue game button
    public void ResumeGame()
    {
        SetGameState(GameStates.Playing);
        StartCoroutine(AddPointsEachHalfSecond());
        uiManager.SetPauseScreenInactive();
    }

    private void HandlePlayingState()
    {
        if (CurrentState == GameStates.Playing)
        {
            GetSpaceshipsAndObstaclesInGame();

            EnableMovementAndSetSpeedOfMovableObjects();

            SetupPlayingScreen();
            EnableSpawnManager();
        }
    }

    private void HandlePauseOrGameOverState()
    {
        if (CurrentState == GameStates.Paused || CurrentState == GameStates.GameOver)
        {
            DisableMovementOfMovableObjects();
            DisableSpawnManager();
        }
    }

    private void SpeedupGame()
    {
        int score = uiManager.GetScore();

        if (score >= ScoreStep * speedupRound)
        {
            IncreaseMovementSpeed();
            DecreaseSpawningTime();
        }
    }

    public void GetSpaceshipsAndObstaclesInGame()
    {
        spaceshipsInGame = poolingSystem.GetInstanciatedSpaceships();
        obstaclesInGame = poolingSystem.GetInstanciatedObstacles();

        CreateListOfMovablesInGame();
    }

    private void CreateListOfMovablesInGame()
    {
        foreach (SpaceshipController ship in spaceshipsInGame)
        {
            movabelsInGame.Add(ship);
        }
        foreach (ObstacleController obstacle in obstaclesInGame)
        {
            movabelsInGame.Add(obstacle);
        }
        foreach (StageController stage in stagesInGame)
        {
            movabelsInGame.Add(stage);
        }
    }

    private void EnableMovementAndSetSpeedOfMovableObjects()
    {
        foreach (MovementManager movable in movabelsInGame)
        {
            movable.EnableMovement();
            movable.SetMovementSpeed(MovingSpeed);
        }
        player.EnableMovement();
        player.SetMovementSpeed(MovingSpeed);
    }

    private void SetupPlayingScreen()
    {
        uiManager.SetScoreScreenActive();
        uiManager.SetStartScreenInactive();
    }

    private void EnableSpawnManager()
    {
        spawnManager.EnableSpawning();
        spawnManager.gameObject.SetActive(true);
    }

    private void DisableSpawnManager()
    {
        spawnManager.DisableSpawning();
        spawnManager.gameObject.SetActive(false);
    }


    private void DisableMovementOfMovableObjects()
    {
        foreach (MovementManager movable in movabelsInGame)
        {
            movable.DisableMovement();
        }
        player.DisableMovement();
    }

    private void IncreaseMovementSpeed()
    {
        MovingSpeed += SpeedIncrease;
        speedupRound++;

        EnableMovementAndSetSpeedOfMovableObjects();
    }

    private void DecreaseSpawningTime()
    {
        float spaceshipSpawnDelay = spawnManager.GetSpaceshipSpawnDelay();
        float obstacleSpawnDelay = spawnManager.GetObstacleSpawnDelay();

        if (spaceshipSpawnDelay > MinimumSpaceshipSpawningDelay)
            spawnManager.SetSpaceshipSpawnDelay(spaceshipSpawnDelay - SpawningDelayDecreaser);

        if (obstacleSpawnDelay > MinimumObstacleSpawningDelay)
            spawnManager.SetObstacleSpawnDelay(obstacleSpawnDelay - SpawningDelayDecreaser);
    }

    //Bind with Unity event, on start game button
    public void StartGame()
    {
        SetGameState(GameStates.Playing);
        StartCoroutine(AddPointsEachHalfSecond());
    }

    public IEnumerator AddPointsEachHalfSecond()
    {
        while (CurrentState == GameStates.Playing)
        {
            uiManager.SetScoreOnScoreScreen(OneScorePoint);
            yield return new WaitForSeconds(AddPointsDelay);
        }
    }

    //Bind with Unity event, on restart game button
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Bind with Unity event, on exit game button
    public void QuitGame()
    {
        Debug.Log("Exit");
        Application.Quit();
    }

    private void OnDisable()
    {
        UnsubscribeFromCollectAction();
        UnsubscribeFromPlayerDeadAction();
    }

    private void UnsubscribeFromPlayerDeadAction()
    {
        EventManager.Instance.OnPlayerDeadAction -= GameOver;
    }

    private void UnsubscribeFromCollectAction()
    {
        EventManager.Instance.OnCollectAction -= CollectSpaceship;
    }
}


public enum GameStates
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}
