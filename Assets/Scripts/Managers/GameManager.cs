
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private const int ScoreStep = 100;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private UIManager uiManager;
    private int speedupRound = 1;
    private GameStates CurrentState;
    private int score;


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
        score = 0;
    }

    private void SetGameState(GameStates state)
    {
        CurrentState = state;
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        EventManager.Instance.SubscribeToOnPlayerDeadAction(GameOver);
        EventManager.Instance.SubscribeToOnCollectAction(CollectCollectable);
        EventManager.Instance.SubscribeToOnEnemyKilledAction(EnemyKilled);
        EventManager.Instance.SubscribeToChangeScoreOnScreen(SetScoreOnScreen);
    }

    //Unity event, called when player is dead
    public void GameOver()
    {
        SetGameState(GameStates.GameOver);
        EventManager.Instance.StopAddingPoints();
        uiManager.SetEndScreenActive();
        uiManager.SetScoreScreenInactive();
    }

    public void CollectCollectable(CollectableController collectible, int pointsWorth)
    {
        score += pointsWorth;
        uiManager.SetScoreOnScoreScreen(score);
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
        if (CurrentState != GameStates.Playing)
            return;

        SetGameState(GameStates.Paused);
        EventManager.Instance.StopAddingPoints();
        uiManager.SetPauseScreenActive();
    }

    //Bind with Unity event, on continue game button
    public void ResumeGame()
    {
        SetGameState(GameStates.Playing);
        EventManager.Instance.StartAddingPoints();
        uiManager.SetPauseScreenInactive();
    }

    private void HandlePlayingState()
    {
        if (CurrentState != GameStates.Playing)
            return;

        EnableSpawnManager();
        MovementManager.Instance.EnableMovementOfObjects();
        MovementManager.Instance.SetMovementSpeedOfObjects();
        SetupPlayingScreen();
    }

    private void HandlePauseOrGameOverState()
    {
        if (CurrentState == GameStates.Playing || CurrentState == GameStates.MainMenu)
            return;

        MovementManager.Instance.DisableMovementOfMovableObjects();
        DisableSpawnManager();
    }

    private void SpeedupGame()
    {
        if (score < ScoreStep * speedupRound)
            return;

        MovementManager.Instance.IncreaseMovementSpeed();
        speedupRound++;
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

    //Bind with Unity event, on start game button
    public void StartGame()
    {
        SetGameState(GameStates.Playing);
        EventManager.Instance.StartAddingPoints();
    }

    private void SetScoreOnScreen(int score)
    {
        uiManager.SetScoreOnScoreScreen(score);
    }

    //Unity event, this is called when settings button is clicked
    public void OpenSettings()
    {
        uiManager.SetSettingsScreenActive();

        if (CurrentState == GameStates.MainMenu)
            uiManager.SetStartScreenInactive();

        if (CurrentState == GameStates.Paused)
            uiManager.SetPauseScreenInactive();

        if (CurrentState == GameStates.GameOver)
            uiManager.SetGameOverScreenInactive();
    }

    //Unity event, this is called when save or cancel buttons are clicked in settings menu
    public void SaveOrCancelSettings()
    {
        uiManager.SetSettingsScreenInactive();
        if (CurrentState == GameStates.MainMenu)
            uiManager.SetStartScreenActive();

        if (CurrentState == GameStates.Paused)
            uiManager.SetPauseScreenActive();

        if (CurrentState == GameStates.GameOver)
            uiManager.SetGameOverScreenActive();
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
        UnsubscribeFromEvents();
    }

    private void UnsubscribeFromEvents()
    {
        EventManager.Instance.UnsubscribeFromChangeScoreOnScreen(SetScoreOnScreen);
        EventManager.Instance.UnsubscribeFromOnPlayerDeadAction(GameOver);
        EventManager.Instance.UnsubscribeFromOnCollectAction(CollectCollectable);
    }
}


public enum GameStates
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}
