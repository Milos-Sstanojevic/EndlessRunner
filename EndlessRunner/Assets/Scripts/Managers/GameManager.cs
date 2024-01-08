
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GlobalConstants;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private PlayerController player;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private PoolingSystem poolingSystem;
    [SerializeField] private List<StageController> stagesInGame;
    [SerializeField] private MovementManager movementManager;
    private bool isPaused = false;
    private List<SpaceshipController> spaceshipsInGame;
    private List<ObstacleController> obstaclesInGame;



    //SINGLETON
    public static GameManager Instance { get; private set; }
    private GameStates CurrentState;
    public float MovingSpeed { get; private set; }

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

        SetGameState(GameStates.Paused);
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
    public void GameOver()
    {
        SetGameState(GameStates.GameOver);
        uiManager.SetEndScreenActive();
        uiManager.SetScoreScreenInactive();
    }

    private void SubscribeToCollectAction()
    {
        EventManager.Instance.OnCollectAction += CollectSpaceship;
    }

    public void CollectSpaceship(ICollectible collectible)
    {
        uiManager.SetScoreOnScoreScreen(ShipsWorth);
    }

    private void Start()
    {
        MovingSpeed = 8;
        spaceshipsInGame = new List<SpaceshipController>();
        obstaclesInGame = new List<ObstacleController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (CurrentState == GameStates.Playing || CurrentState == GameStates.Paused))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        HandlePlayingState();
        HandlePauseOrGameOverState();
    }

    private void HandlePauseOrGameOverState()
    {
        if (CurrentState == GameStates.Paused || CurrentState == GameStates.GameOver)
        {
            movementManager.DisableAllMovements(player, obstaclesInGame, stagesInGame, spaceshipsInGame);
            DisableSpawnManager();
        }
    }
    private void DisableSpawnManager()
    {
        spawnManager.DisableSpawning();
        spawnManager.gameObject.SetActive(false);
    }

    public void PauseGame()
    {
        SetGameState(GameStates.Paused);
        uiManager.SetPauseScreenActive();
        isPaused = true;
    }

    public void ResumeGame()
    {
        SetGameState(GameStates.Playing);
        StartCoroutine(AddPointsEachHalfSecond());
        uiManager.SetPauseScreenInactive();
        isPaused = false;
    }

    private void HandlePlayingState()
    {
        if (CurrentState == GameStates.Playing)
        {
            GetSpaceshipsAndObstaclesInGame();

            movementManager.EnableAllMovement(player, obstaclesInGame, stagesInGame, spaceshipsInGame);
            movementManager.SetSpeedForAll(player, obstaclesInGame, stagesInGame, spaceshipsInGame, MovingSpeed);

            SetupPlayingScreen();

            EnableSpawnManager();
        }
    }

    private void EnableSpawnManager()
    {
        spawnManager.EnableSpawning();
        spawnManager.gameObject.SetActive(true);
    }

    private void SetupPlayingScreen()
    {
        uiManager.SetScoreScreenActive();
        uiManager.SetStartScreenInactive();
    }

    public void GetSpaceshipsAndObstaclesInGame()
    {
        spaceshipsInGame = poolingSystem.GetInstanciatedSpaceships();
        obstaclesInGame = poolingSystem.GetInstanciatedObstacles();
    }



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

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

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
    Playing,
    Paused,
    GameOver
}
