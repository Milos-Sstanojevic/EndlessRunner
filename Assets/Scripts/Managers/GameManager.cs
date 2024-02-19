using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private const int DefaultNumberOfManagers = 1;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private int gravityModifier;
    private GameStates CurrentState;
    [SerializeField] private List<MovementManager> movementManagers;
    [SerializeField] private List<GameObject> screensInGame;
    [SerializeField] private List<SpawnManager> spawnManagers;
    private int numberOfPlayersDead = 1;
    private bool canEnableSpawnManagers = true;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        SetGameState(GameStates.MainMenu);
    }

    private void SetGameState(GameStates state)
    {
        CurrentState = state;
    }

    private void OnEnable()
    {
        SubscribeToEvents();
        Physics.gravity *= gravityModifier;
    }

    private void SubscribeToEvents()
    {
        EventManager.Instance.SubscribeToOnNumberOfScreensChangedAction(SetSpawnManagers);
        EventManager.Instance.SubscribeToOnPlayerDeadAction(GameOver);
        EventManager.Instance.SubscribeToChangeScoreOnScreen(SetScoreOnScreen);
        EventManager.Instance.SubscribeToOnNumberOfMovementManagersChanged(SetMovementMangers);
        EventManager.Instance.SubscribeToOnNumberOfScreensChangedAction(SetScreensInGame);
        EventManager.Instance.SubscribeToOnObjectsInSceneChangedAction(EnableMovementForNewObjects);
    }

    private IEnumerator EnableMovementForNewObjectsCoroutine(SpawnManager spawnManager)
    {
        yield return new WaitForEndOfFrame();

        if (movementManagers != null && screensInGame != null)
        {
            for (int i = 0; i < movementManagers.Count; i++)
            {
                movementManagers[i].EnableMovementOfObjects(screensInGame[i]);
                movementManagers[i].SetMovementSpeedOfObjects(screensInGame[i]);
            }
        }
    }

    private void EnableMovementForNewObjects(SpawnManager spawnManager)
    {
        if (CurrentState != GameStates.Playing)
            return;
        StartCoroutine(EnableMovementForNewObjectsCoroutine(spawnManager));
    }

    private void SetScreensInGame(GameObject[] screens)
    {
        screensInGame.RemoveRange(DefaultNumberOfManagers, screensInGame.Count - DefaultNumberOfManagers);
        for (int i = 0; i < screens.Length; i++)
            screensInGame.Add(screens[i]);
    }

    private void SetMovementMangers(List<MovementManager> managers)
    {
        movementManagers.RemoveRange(DefaultNumberOfManagers, movementManagers.Count - DefaultNumberOfManagers);
        foreach (MovementManager manager in managers)
            movementManagers.Add(manager);
    }

    private void SetSpawnManagers(GameObject[] screens)
    {
        spawnManagers.RemoveRange(DefaultNumberOfManagers, spawnManagers.Count - DefaultNumberOfManagers);
        foreach (GameObject screen in screens)
        {
            SpawnManager managersInScreen = screen.GetComponentInChildren<SpawnManager>(true);
            spawnManagers.Add(managersInScreen);
        }
    }

    //Unity event, called when player is dead
    public void GameOver(PlayerController player)
    {
        if (numberOfPlayersDead == movementManagers.Count)
        {
            SetGameState(GameStates.GameOver);
            uiManager.SetEndScreenActive();
            uiManager.SetScoreScreenInactive();
        }
        numberOfPlayersDead++;
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
        uiManager.SetSettingsScreenInactive();
    }

    private void HandlePlayingState()
    {
        if (CurrentState != GameStates.Playing)
            return;

        EnableSpawnManagers();
        SetupPlayingScreen();
    }

    private void HandlePauseOrGameOverState()
    {
        if (CurrentState == GameStates.Playing || CurrentState == GameStates.MainMenu)
            return;

        if (CurrentState == GameStates.Paused)
        {
            if (movementManagers != null && screensInGame != null)
                for (int i = 0; i < movementManagers.Count; i++)
                    movementManagers[i].DisableMovementOfMovableObjects(screensInGame[i]);

            EventManager.Instance.StopAddingPoints();
            DisableSpawnManagers();
        }
    }

    private void SetupPlayingScreen()
    {
        uiManager.SetScoreScreenActive();
        uiManager.SetStartScreenInactive();
    }

    private void EnableSpawnManagers()
    {
        if (canEnableSpawnManagers)
        {
            if (spawnManagers != null)
            {
                foreach (SpawnManager spawnManager in spawnManagers)
                {
                    spawnManager.EnableSpawning();
                    spawnManager.gameObject.SetActive(true);
                }
            }
            canEnableSpawnManagers = false;
        }
    }

    private void DisableSpawnManagers()
    {
        if (spawnManagers != null)
        {
            foreach (SpawnManager spawnManager in spawnManagers)
            {
                spawnManager.DisableSpawning();
                spawnManager.gameObject.SetActive(false);
            }
        }
        canEnableSpawnManagers = true;
    }

    //Bind with Unity event, on start game button
    public void StartGame()
    {
        SetGameState(GameStates.Playing);
        uiManager.SetScoreScreenActive();
        EventManager.Instance.StartAddingPoints();
    }

    private void SetScoreOnScreen(int score, TextMeshProUGUI playerScoreText)
    {
        uiManager.SetScoreOnScoreScreen(score, playerScoreText);
    }

    public void OpenNumberOfPlayersScreen()
    {
        uiManager.SetNumberOfPlayersScreenActive();
        uiManager.SetStartScreenInactive();
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
        Physics.gravity /= gravityModifier;
    }

    private void UnsubscribeFromEvents()
    {
        EventManager.Instance.UnsubscribeFromChangeScoreOnScreen(SetScoreOnScreen);
        EventManager.Instance.UnsubscribeFromOnPlayerDeadAction(GameOver);
        EventManager.Instance.UnsubscribeFromOnNumberOfScreensChangedAction(SetScreensInGame);
        EventManager.Instance.UnsubscribeFromOnObjectsInSceneChangedAction(EnableMovementForNewObjects);
    }
}


public enum GameStates
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}
