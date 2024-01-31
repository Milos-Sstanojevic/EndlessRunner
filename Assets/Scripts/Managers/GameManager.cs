
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private const int scoreStep = 100;
    private const float minimumObstacleSpawningDelay = 0.5f;
    private const float spacingDecreaser = 0.14f;
    private const float minimumSpacing = 6f;
    private const float spawningDelayDecreaser = 0.07f;
    private const float addPointsDelay = 0.5f;
    private const int oneScorePoint = 1;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private UIManager uiManager;
    private int speedupRound = 1;
    private GameStates CurrentState;


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
        SubscribeToDeadCollectKilledEvents();
    }

    private void SubscribeToDeadCollectKilledEvents()
    {
        EventManager.Instance.OnPlayerDeadAction += GameOver;
        EventManager.Instance.OnCollectAction += CollectCollectable;
        EventManager.Instance.OnEnemyKilledAction += EnemyKilled;
    }

    //Unity event, called when player is dead
    public void GameOver()
    {
        SetGameState(GameStates.GameOver);
        uiManager.SetEndScreenActive();
        uiManager.SetScoreScreenInactive();
    }

    public void CollectCollectable(CollectableController collectible, int pointsWorth)
    {
        uiManager.SetScoreOnScoreScreen(pointsWorth);
    }

    public void EnemyKilled(int enemyWorth)
    {
        uiManager.SetScoreOnScoreScreen(enemyWorth);
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
            EnableSpawnManager();
            MovementManager.Instance.EnableMovementOfObjects();
            MovementManager.Instance.SetMovementSpeedOfObjects();
            SetupPlayingScreen();
        }
    }

    private void HandlePauseOrGameOverState()
    {
        if (CurrentState == GameStates.Paused || CurrentState == GameStates.GameOver)
        {
            MovementManager.Instance.DisableMovementOfMovableObjects();
            DisableSpawnManager();
        }
    }

    private void SpeedupGame()
    {
        int score = uiManager.GetScore();

        if (score >= scoreStep * speedupRound)
        {
            MovementManager.Instance.IncreaseMovementSpeed();
            //DecreaseSpawningTime();
            speedupRound++;
        }
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

    private void DecreaseSpawningTime()
    {
        float chunkSpawnDelay = spawnManager.GetChunkSpawnDelay();
        float spacingBetweenObstacles = spawnManager.GetSpacingBetweenObstacles();
        float spawnDelay;
        float spacing;

        if (chunkSpawnDelay > minimumObstacleSpawningDelay)
        {
            spawnDelay = HandleDecreasingSpawnDelay(chunkSpawnDelay, minimumObstacleSpawningDelay);
            spacing = HandleDecreasingSpacing(spacingBetweenObstacles, minimumSpacing);

            spawnManager.SetChunkSpawnDelay(spawnDelay);
            spawnManager.SetSpacingBetweenObstacles(spacing);
        }
    }

    private float HandleDecreasingSpacing(float spacing, float minimumSpacing)
    {
        float space = spacing - spacingDecreaser;

        if (space < minimumSpacing)
        {
            space = minimumSpacing;
        }

        return space;
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
