using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public ScoreManager ScoreManager { get; private set; }
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject readyScreen;
    private bool isPlayerDead;
    private PlayerMovement playerMovement;
    private OneScreenController screenOfPlayer;
    [Networked] public int NumberOfPlayersReady { get; set; }
    [Networked] public int PlayerId { get; set; }

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        ScoreManager = GetComponent<ScoreManager>();
        NetworkSpawner.Instance.RPC_SpawnedPlayer(this);
        ScoreManager.Initialize();
        SetReadyScreenActive();
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

    public int GetPlayerId() => PlayerId;

    public void SetPlayerId(int id)
    {
        PlayerId = id;
    }

    public void StartGame(List<OneScreenController> screensInGame, List<Canvas> playerCanvases, List<MovementManager> movementManagers)
    {

    }

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
