using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject startScreen;
    public PlayerController player;
    public PoolingSystem poolingSystem;
    public GameObject endScreen;
    public GameObject pauseScreen;
    public GameObject scoreScreen;
    public AudioSource audioSource;
    public AudioClip spaceshipCollected;
    private int points;
    public TextMeshProUGUI pointsText;


    //SINGLETON
    public bool IsGameActive { get; private set; }
    public GameState CurrentState { get; private set; }
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

        SubscribeToCollectAction();
    }

    private void OnDestroy()
    {
        UnsubscribeFromCollectAction();
    }



    // Start is called before the first frame update
    void Start()
    {
        MovingSpeed = 8;
        audioSource = GetComponent<AudioSource>();
        points = 0;
        pointsText.text = "Score: " + points;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && CurrentState == GameState.Playing)
        {
            PauseGame();
        }

        if (CurrentState == GameState.Pause && Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeGame();
        }
    }

    public void SetGameState(GameState state)
    {
        CurrentState = state;
    }

    public void StartGame()
    {
        IsGameActive = true;
        SetGameState(GameState.Playing);
        player.animator.SetBool("Run", true);
        poolingSystem.gameObject.SetActive(true);
        scoreScreen.SetActive(true);
        StartCoroutine(AddPointsEachSecond());
        startScreen.SetActive(false);
    }

    public void PauseGame()
    {
        SetGameState(GameState.Pause);
        IsGameActive = false;
        player.animator.SetBool("Run", false);
        pauseScreen.SetActive(true);
        player.animator.enabled = false;
    }

    public void ResumeGame()
    {
        SetGameState(GameState.Playing);

        IsGameActive = true;
        pauseScreen.SetActive(false);
        player.animator.SetBool("Run", true);
        StartSpawningCoroutines();
        StartCoroutine(AddPointsEachSecond());
    }


    public void StartSpawningCoroutines()
    {
        StartCoroutine(poolingSystem.SpawnObstacle());
        StartCoroutine(poolingSystem.SpawnSpaceship());
    }

    public void GameOver()
    {
        SetGameState(GameState.GameOver);
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

    public IEnumerator AddPointsEachSecond()
    {
        while (IsGameActive)
        {
            points += 1;
            pointsText.text = "Score: " + points;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void CollectSpaceship()
    {
        points += 20;
        pointsText.text = "Score: " + points;
        audioSource.PlayOneShot(spaceshipCollected, 1.0f);
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


public enum GameState
{
    Playing,
    Pause,
    GameOver
}
