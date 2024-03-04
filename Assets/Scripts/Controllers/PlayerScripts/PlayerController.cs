using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public ScoreManager ScoreManager { get; private set; }
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject readyScreen;
    [SerializeField] private NetworkObject playerCanvas;
    private bool isPlayerDead;
    [SerializeField] private bool isGameOnline;
    private PlayerMovement playerMovement;
    private OneScreenController screenOfPlayer;
    [Networked] public int NumberOfPlayersReady { get; set; }
    [Networked] public int PlayerId { get; set; }

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        ScoreManager = GetComponent<ScoreManager>();
        if (isGameOnline)
            NetworkSpawner.Instance.RPC_SpawnedPlayer(this);
        ScoreManager.Initialize();
    }

    public override void Spawned()
    {
        SetGameToOnline();
    }

    private void Start()
    {
        Time.timeScale = 1f;

        if (isGameOnline)
            SetReadyScreenActive();
    }

    private void OnEnable()
    {
        EventManager.Instance.SubscribeToStartAddingPointsAction(ScoreManager.StartAddingPoints);
        EventManager.Instance.SubscribeToStopAddingPointsAction(ScoreManager.StopAddingPoints);
        EventManager.Instance.SubscribeToOnSetGameToOnlineAction(SetGameToOnline);
        EventManager.Instance.SubscribeToOnSetGameToOfflineAction(SetGameToOffline);
    }

    private void SetGameToOnline()
    {
        isGameOnline = true;
    }

    private void SetGameToOffline()
    {
        isGameOnline = false;
    }

    public void PlayerDied()
    {
        isPlayerDead = true;
    }

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

    //Called when player presses Ready button
    public void Ready()
    {
        if (Runner.LocalPlayer.PlayerId == PlayerId)
            RPC_ReadyIsPressed();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ReadyIsPressed()
    {
        NumberOfPlayersReady++;
        NetworkSpawner.Instance.RPC_StartGames(NumberOfPlayersReady);
        readyScreen.SetActive(false);
    }

    public void IncreaseNumberOfPlayersReady()
    {
        NumberOfPlayersReady++;
    }

    public void SetReadyScreenActive()
    {
        EventManager.Instance.OnSetOnlineScreenInactive();
        readyScreen.SetActive(true);
    }

    public void SetCameraRect(Rect rect)
    {
        playerCamera.rect = rect;
    }

    public void SetCanvasRect()
    {
        playerCanvas.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerCanvas.gameObject.GetComponent<RectTransform>().rect.width / 2);
    }

    public void SetPlayerId(int id)
    {
        PlayerId = id;
    }

    public int GetPlayerId() => PlayerId;
    public bool IsPlayerDead() => isPlayerDead;
    public PlayerMovement GetPlayerMovementComponentOfPlayer() => playerMovement;
    public OneScreenController GetScreenOfPlayer() => screenOfPlayer;

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
