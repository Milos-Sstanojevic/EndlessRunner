using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public ScoreManager ScoreManager { get; private set; }
    [SerializeField] private Camera playerCamera;
    private bool isPlayerDead;
    private PlayerMovement playerMovement;
    private OneScreenController screenOfPlayer;
    private int playerId;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        ScoreManager = GetComponent<ScoreManager>();
        NetworkSpawner.Instance.RPC_SpawnedPlayer(this);
        playerId = NetworkSpawner.Instance.GetNetworkRunner().LocalPlayer.PlayerId;
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
    public void SetScreenOfPlayer(OneScreenController screen)
    {
        screenOfPlayer = screen;
    }

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

    public void SetCameraRect(Rect rect)
    {
        playerCamera.rect = rect;
    }

    public int GetPlayerId() => playerId;

    private void OnDisable()
    {
        EventManager.Instance.UnsubscribeFromStartAddingPointsAction(ScoreManager.StartAddingPoints);
        EventManager.Instance.UnsubscribeFromStopAddingPointsAction(ScoreManager.StopAddingPoints);
    }

    private void OnDestroy()
    {
        NetworkSpawner.Instance.RPC_DespawnedPlayer(this);
    }
}
