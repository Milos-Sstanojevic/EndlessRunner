using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public ScoreManager ScoreManager { get; private set; }
    private bool isPlayerDead;
    private PlayerMovement playerMovement;
    [SerializeField] private OneScreenController screenOfPlayer;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        ScoreManager = GetComponent<ScoreManager>();
        ScoreManager.Initialize();

    }

    private void Start()
    {
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        EventManager.Instance.SubscribeToStartAddingPointsAction(ScoreManager.StartAddingPoints);
        EventManager.Instance.SubscribeToStopAddingPointsAction(ScoreManager.StopAddingPoints);
    }

    public void PlayerDied()
    {
        isPlayerDead = true;
    }

    public bool IsPlayerDead() => isPlayerDead;

    public PlayerMovement GetPlayerMovementComponentOfPlayer() => playerMovement;

    public OneScreenController GetScreenOfPlayer() => screenOfPlayer;

    public void RestartGame()
    {
        EventManager.Instance.OnRestartButtonClicked();
    }

    public void ExitGame()
    {
        EventManager.Instance.OnExitButtonClicked();
    }

    public void OpenSettings()
    {
        EventManager.Instance.OnSettingsButtonClicked();
    }

    private void OnDisable()
    {
        EventManager.Instance.UnsubscribeFromStartAddingPointsAction(ScoreManager.StartAddingPoints);
        EventManager.Instance.UnsubscribeFromStopAddingPointsAction(ScoreManager.StopAddingPoints);
    }
}
