
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private const int scoreStep = 100;
    private const float speedIncrease = 1f;
    private const float minimumSpaceshipSpawningDelay = 0.3f;
    private const float minimumObstacleSpawningDelay = 0.2f;
    private const float spawningDelayDecreaser = 0.08f;
    private const float addPointsDelay = 0.5f;
    private const int oneScorePoint = 1;
    private const float playerSpeedBalancer = 1;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private PlayerController player;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private PoolingSystem poolingSystem;
    [SerializeField] private List<StageController> stagesInGame;
    private List<ObjectMovementBase> movabelsInGame;
    private List<CollectableBase> spaceshipsInGame;
    private List<ObjectMovementBase> obstaclesInGame;
    private int speedupRound = 1;
    private GameStates CurrentState;
    public float MovingSpeed { get; private set; }


    private void Start()
    {
        MovingSpeed = 8;
        spaceshipsInGame = new List<CollectableBase>();
        obstaclesInGame = new List<ObjectMovementBase>();
        movabelsInGame = new List<ObjectMovementBase>();
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
        EventManager.Instance.OnCollectAction += CollectCollectable;
    }

    //Unity event, called when player is dead
    public void GameOver()
    {
        SetGameState(GameStates.GameOver);
        uiManager.SetEndScreenActive();
        uiManager.SetScoreScreenInactive();
    }

    public void CollectCollectable(CollectableBase collectible, int pointsWorth)
    {
        uiManager.SetScoreOnScoreScreen(pointsWorth);
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

        if (score >= scoreStep * speedupRound)
        {
            IncreaseMovementSpeed();
            DecreaseSpawningTime();
        }
    }

    public void GetSpaceshipsAndObstaclesInGame()
    {
        spaceshipsInGame = poolingSystem.GetInstanciatedCollectables();
        obstaclesInGame = poolingSystem.GetInstanciatedObstacles();

        CreateListOfMovablesInGame();
    }

    private void CreateListOfMovablesInGame()
    {
        foreach (CollectableBase ship in spaceshipsInGame)
        {
            movabelsInGame.Add(ship);
        }
        foreach (ObjectMovementBase obstacle in obstaclesInGame)
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
        foreach (ObjectMovementBase movable in movabelsInGame)
        {
            movable.EnableMovement();
            movable.SetMovementSpeed(MovingSpeed);
        }
        player.EnableMovement();
        player.SetMovementSpeed(MovingSpeed + playerSpeedBalancer);
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
        foreach (ObjectMovementBase movable in movabelsInGame)
        {
            movable.DisableMovement();
        }
        player.DisableMovement();
    }

    private void IncreaseMovementSpeed()
    {
        MovingSpeed += speedIncrease;
        speedupRound++;

        EnableMovementAndSetSpeedOfMovableObjects();
    }

    private void DecreaseSpawningTime()
    {
        float spaceshipSpawnDelay = spawnManager.GetSpaceshipSpawnDelay();
        float obstacleSpawnDelay = spawnManager.GetObstacleSpawnDelay();
        float spawnDelay;

        if (spaceshipSpawnDelay > minimumSpaceshipSpawningDelay)
        {
            spawnDelay = HandleDecreasingSpawnDelay(spaceshipSpawnDelay, minimumSpaceshipSpawningDelay);

            spawnManager.SetSpaceshipSpawnDelay(spawnDelay);
        }

        if (obstacleSpawnDelay > minimumObstacleSpawningDelay)
        {
            spawnDelay = HandleDecreasingSpawnDelay(obstacleSpawnDelay, minimumObstacleSpawningDelay);

            spawnManager.SetObstacleSpawnDelay(spawnDelay);
        }
    }

    private float HandleDecreasingSpawnDelay(float spawnDelay, float minimumSpawnDelay)
    {
        float delay = spawnDelay - spawningDelayDecreaser;

        if (delay < minimumSpawnDelay)
            delay = minimumSpawnDelay;

        return delay;
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
            uiManager.SetScoreOnScoreScreen(oneScorePoint);
            yield return new WaitForSeconds(addPointsDelay);
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
        EventManager.Instance.OnCollectAction -= CollectCollectable;
    }
}


public enum GameStates
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}