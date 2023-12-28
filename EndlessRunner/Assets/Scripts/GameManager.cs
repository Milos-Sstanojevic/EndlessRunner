
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GlobalConstants;

public class GameManager : MonoBehaviour
{
    private const int shipsWorth = 20;

    public static GameManager Instance;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private PoolingSystem poolingSystem;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject scoreScreen;
    [SerializeField] private TextMeshProUGUI pointsText;

    private int points;


    //SINGLETON
    public bool IsGameActive { get; private set; }
    public GameStates CurrentState { get; private set; }
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
        SetGameState(GameStates.Pause);
        SubscribeToCollectAction();
    }

    private void OnDestroy()
    {
        UnsubscribeFromCollectAction();
    }

    void Start()
    {
        MovingSpeed = 8;
        points = 0;
        pointsText.text = ScoreText + points;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && CurrentState == GameStates.Playing)
        {
            PauseGame();
        }

        if (CurrentState == GameStates.Pause && Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeGame();
        }
    }

    private void SetGameState(GameStates state)
    {
        CurrentState = state;
    }

    public void StartGame()
    {
        IsGameActive = true;
        SetGameState(GameStates.Playing);
        poolingSystem.gameObject.SetActive(true);
        scoreScreen.SetActive(true);
        StartCoroutine(AddPointsEachHalfSecond());
        startScreen.SetActive(false);
    }

    public void PauseGame()
    {
        IsGameActive = false;
        SetGameState(GameStates.Pause);
        pauseScreen.SetActive(true);
    }

    public void ResumeGame()
    {
        IsGameActive = true;
        SetGameState(GameStates.Playing);
        pauseScreen.SetActive(false);
        StartSpawningCoroutines();
        StartCoroutine(AddPointsEachHalfSecond());
    }


    public void StartSpawningCoroutines()
    {
        StartCoroutine(poolingSystem.SpawnObstacle());
        StartCoroutine(poolingSystem.SpawnSpaceship());
    }

    public void GameOver()
    {
        SetGameState(GameStates.GameOver);
        endScreen.SetActive(true);
        scoreScreen.SetActive(false);
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
            points++;
            pointsText.text = ScoreText + points;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void CollectSpaceship()
    {
        points += shipsWorth;
        pointsText.text = ScoreText + points;
    }

    void UnsubscribeFromCollectAction()
    {
        SpaceshipController.OnSpaceshipCollected -= CollectSpaceship;
    }


    void SubscribeToCollectAction()
    {
        SpaceshipController.OnSpaceshipCollected += CollectSpaceship;
    }
}


public enum GameStates
{
    Playing,
    Pause,
    GameOver
}
