
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GlobalConstants;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private PlayerController player;
    [SerializeField] private UIManager uiManager;
    private bool isPaused = false;


    //SINGLETON
    public static GameManager Instance { get; private set; }
    public bool IsGameActive { get; private set; }
    public GameStates CurrentState { get; private set; } //da li ovime mogu da se posluzim u playeru
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

    private void OnEnable()
    {
        SubscribeToCollectAction();
    }

    private void SubscribeToCollectAction()
    {
        SpaceshipController.OnSpaceshipCollected += CollectSpaceship;
    }

    private void OnDisable()
    {
        UnsubscribeFromCollectAction();
    }


    private void UnsubscribeFromCollectAction()
    {
        SpaceshipController.OnSpaceshipCollected -= CollectSpaceship;
    }


    private void Start()
    {
        MovingSpeed = 8;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void SetGameState(GameStates state)
    {
        CurrentState = state;
    }

    public void StartGame()
    {
        if (CurrentState == GameStates.Paused)
        {
            IsGameActive = true;
            SetGameState(GameStates.Playing);

            //ovakve enable cemo verovatno u neku funkciju koja ce na osnovu GameState da aktivira ili deaktivira sta treba
            // a bice pozivana u update stalno, dok ce u ovim startGame i slicnim funkcijama da ostane samo promena state game
            EnableCharacterMovement();

            uiManager.SetScoreScreenActive();
            uiManager.SetStartScreenInactive();

            StartCoroutine(AddPointsEachHalfSecond());
            StartSpawningCoroutines();
        }
    }

    private void EnableCharacterMovement() => player.SetMovementEnabled();
    private void DisableCharacterMovement() => player.SetMovementDisabled();

    public void PauseGame()
    {
        IsGameActive = false;
        SetGameState(GameStates.Paused);
        DisableCharacterMovement();
        uiManager.SetPauseScreenActive();
        isPaused = true;
    }

    public void ResumeGame()
    {
        IsGameActive = true;

        SetGameState(GameStates.Playing);
        EnableCharacterMovement();
        uiManager.SetPauseScreenInactive();

        StartSpawningCoroutines();
        StartCoroutine(AddPointsEachHalfSecond());

        isPaused = false;
    }


    public void StartSpawningCoroutines()
    {
        StartCoroutine(spawnManager.SpawnObstacle());
        StartCoroutine(spawnManager.SpawnSpaceship());
    }

    public void GameOver()
    {
        SetGameState(GameStates.GameOver);
        DisableCharacterMovement();
        uiManager.SetEndScreenActive();
        uiManager.SetScoreScreenInactive();

        IsGameActive = false;
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

    public IEnumerator AddPointsEachHalfSecond()
    {
        while (IsGameActive)
        {
            uiManager.SetScoreOnScoreScreen(1);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void CollectSpaceship()
    {
        uiManager.SetScoreOnScoreScreen(ShipsWorth);
    }
}


public enum GameStates
{
    Playing,
    Paused,
    GameOver
}
