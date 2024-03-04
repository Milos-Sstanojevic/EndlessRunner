using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    private const string IsRestarted = "IsRestarted";
    private const string NumberOfPlayers = "NumberOfPlayers";
    [SerializeField] private UIManager uiManager;
    [SerializeField] private int gravityModifier;
    [SerializeField] private List<MovementManager> movementManagers = new List<MovementManager>();
    [SerializeField] private List<SpawnManager> spawnManagers = new List<SpawnManager>();
    private List<OneScreenController> screensInGame = new List<OneScreenController>();
    private GameStates CurrentState;
    private bool canEnableSpawnManagers = true;
    private int numberOfPlayers = 0;
    private int numberOfDeadPlayers = 0;
    private bool isGameOnline;

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

    private void Start()
    {
        // if (isGameOnline)
        //     return;

        // if (PlayerPrefs.GetInt(IsRestarted) != 1)
        //     return;


        // numberOfPlayers = PlayerPrefs.GetInt(NumberOfPlayers);

        // EventManager.Instance.OnLoadNumberOfPlayers(numberOfPlayers);
        // SplitScreenManager.Instance.Split(numberOfPlayers);
    }

    private void OnEnable()
    {
        SubscribeToEvents();
        Physics.gravity *= gravityModifier;
    }

    private void SubscribeToEvents()
    {
        EventManager.Instance.SubscribeToOnStartGamesAction(StartGame);
        EventManager.Instance.SubscribeToOnNumberOfScreensChangedAction(SetSpawnManagers);
        EventManager.Instance.SubscribeToOnPlayerDeadAction(GameOver);
        EventManager.Instance.SubscribeToChangeScoreOnScreen(SetScoreOnScreen);
        EventManager.Instance.SubscribeToOnNumberOfMovementManagersChanged(SetMovementMangers);
        EventManager.Instance.SubscribeToOnNumberOfScreensChangedAction(SetScreensInGame);
        EventManager.Instance.SubscribeToOnObjectsInSceneChangedAction(EnableMovementForNewObjects);
        EventManager.Instance.SubscribeToOnChangeNumberOfPlayersAction(SetNumberOfPlayers);
        EventManager.Instance.SubscribeToOnRestartButtonClickedAction(RestartGameAction);
        EventManager.Instance.SubscribeToOnSettingsButtonClickedAction(OpenSettingsAction);
        EventManager.Instance.SubscribeToOnExitButtonClickedAction(QuitGameAction);
    }

    private void SetSpawnManagers(OneScreenController[] screens)
    {
        spawnManagers.Clear();
        foreach (OneScreenController screen in screens)
        {
            SpawnManager managersInScreen = screen.GetComponent<OneScreenController>().GetSpawnManagerOfOneScreen();
            spawnManagers.Add(managersInScreen);
        }
    }

    //Unity event, called when player is dead
    public void GameOver(int id, PlayerController player, GameObject endScreen)
    {
        numberOfDeadPlayers++;
        if (numberOfDeadPlayers == screensInGame.Count)
            SetGameState(GameStates.GameOver);
        uiManager.SetEndScreenActive(endScreen);
        uiManager.SetScoreScreenInactive(player);
    }

    private void SetScoreOnScreen(int score, TextMeshProUGUI playerScoreText)
    {
        uiManager.SetScoreOnScoreScreen(score, playerScoreText);
    }

    private void SetMovementMangers(List<MovementManager> managers)
    {
        movementManagers.Clear();
        foreach (MovementManager manager in managers)
            movementManagers.Add(manager);
    }

    private void SetScreensInGame(OneScreenController[] screens)
    {
        screensInGame.Clear();
        foreach (OneScreenController screen in screens)
            screensInGame.Add(screen.GetComponent<OneScreenController>());
    }

    private void EnableMovementForNewObjects(SpawnManager spawnManager)
    {
        if (CurrentState != GameStates.Playing)
            return;
        StartCoroutine(EnableMovementForNewObjectsCoroutine());
    }

    private IEnumerator EnableMovementForNewObjectsCoroutine()
    {
        yield return new WaitForEndOfFrame();
        if (IsThereMovementAndSpawnManagersInLists())
        {
            for (int i = 0; i < movementManagers.Count; i++)
            {
                movementManagers[i].EnableMovementOfObjects(screensInGame[i]);
                movementManagers[i].SetMovementSpeedOfObjects(screensInGame[i]);
            }
        }
    }

    private bool IsThereMovementAndSpawnManagersInLists() => movementManagers != null && screensInGame != null;

    private void SetNumberOfPlayers(int number)
    {
        numberOfPlayers = number;
    }

    private void RestartGameAction()
    {
        if (numberOfDeadPlayers == screensInGame.Count)
            RestartGame();
    }

    private void OpenSettingsAction()
    {
        if (numberOfDeadPlayers == screensInGame.Count)
            OpenSettings();
    }

    private void QuitGameAction()
    {
        if (numberOfDeadPlayers == screensInGame.Count)
            QuitGame();
    }

    public void Update()
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
        if (CurrentState == GameStates.Playing || CurrentState == GameStates.MainMenu || CurrentState == GameStates.Playing)
            return;

        if (IsThereMovementAndSpawnManagersInLists())
            for (int i = 0; i < movementManagers.Count; i++)
                movementManagers[i].DisableMovementOfMovableObjects(screensInGame[i]);

        EventManager.Instance.StopAddingPoints();
        DisableSpawnManagers();
    }

    private void SetupPlayingScreen()
    {
        uiManager.SetScoreScreenActive();
        uiManager.SetStartScreenInactive();
    }

    private void EnableSpawnManagers()
    {
        if (!canEnableSpawnManagers)
            return;

        if (spawnManagers == null)
            return;

        foreach (SpawnManager spawnManager in spawnManagers)
        {
            if (spawnManager != null)
            {
                spawnManager.EnableSpawning();
                spawnManager.gameObject.SetActive(true);
            }
        }
        canEnableSpawnManagers = false;
    }

    private void DisableSpawnManagers()
    {
        if (spawnManagers == null)
            return;

        foreach (SpawnManager spawnManager in spawnManagers)
        {
            spawnManager.DisableSpawning();
            spawnManager.gameObject.SetActive(false);
        }
        canEnableSpawnManagers = true;
    }

    //Bind with Unity event, on start game button
    public void StartGame()
    {
        if (PlayerPrefs.GetInt(IsRestarted) == 0 || PlayerPrefs.GetInt(NumberOfPlayers) != numberOfPlayers || isGameOnline)
            EventManager.Instance.OnNumberOfPlayersChosen();
        else
            PlayerPrefs.SetInt(IsRestarted, 0);
        if (!isGameOnline)
            StartCoroutine(StartPlaying());
        else
        {
            uiManager.SetScoreScreenActive();
            uiManager.SetNumberOfPlayersScreenInactive();
            EventManager.Instance.StartAddingPoints();
            SetGameState(GameStates.Playing);
        }
    }

    private IEnumerator StartPlaying()
    {
        yield return new WaitForEndOfFrame();
        SetGameState(GameStates.Playing);
        uiManager.SetScoreScreenActive();
        uiManager.SetNumberOfPlayersScreenInactive();
        EventManager.Instance.StartAddingPoints();
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
        // PlayerPrefs.SetInt(IsRestarted, 1);
    }

    //Bind with Unity event, on exit game button
    public void QuitGame()
    {
        Debug.Log("Exit");
        Application.Quit();
    }

    //Go to online pannel when Online button is pressed
    public void OpenOnlinePannel()
    {
        isGameOnline = true;
        EventManager.Instance.OnSetGameToOnline();

        uiManager.OpenOnlinePannel();
    }

    //Go back to main menu from Online menu when Back button is pressed
    public void GoBackToMainMenu()
    {
        if (isGameOnline)
        {
            isGameOnline = false;
            EventManager.Instance.OnSetGameToOffline();
        }
        uiManager.GoBackToMainMenu();
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
        EventManager.Instance.UnsubscribeFromOnRestartButtonClickedAction(RestartGame);
        EventManager.Instance.UnsubscribeFromOnSettingsButtonClickedAction(OpenSettings);
        EventManager.Instance.UnsubscribeFromOnExitButtonClickedAction(QuitGame);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(NumberOfPlayers, 1);
    }
}

public enum GameStates
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}
