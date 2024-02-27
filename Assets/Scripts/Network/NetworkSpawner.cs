using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkSpawner Instance
    {
        get; set;
    }


    public Dictionary<PlayerRef, NetworkObject> SpawnedScreens = new Dictionary<PlayerRef, NetworkObject>();
    private NetworkRunner _runner;
    public PlayerRef playerRef;
    private List<OneScreenController> screensInGame = new List<OneScreenController>();
    private List<Canvas> playerCanvases = new List<Canvas>();
    private List<MovementManager> movementManagers = new List<MovementManager>();


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
    }

    async void StartGame(GameMode mode)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "RunnerRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void EnterAsHost()
    {
        if (_runner == null)
        {
            StartGame(GameMode.Host);
        }
    }

    public void EnterAsClient()
    {
        if (_runner == null)
        {
            StartGame(GameMode.Client);
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            NetworkObject screen = SplitScreenManager.Instance.SpawnOnlineScreen(runner, player);
            SpawnedScreens.Add(player, screen);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (SpawnedScreens.TryGetValue(player, out NetworkObject screen))
        {
            runner.Despawn(screen);
            SpawnedScreens.Remove(player);
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SpawnedObject(OneScreenController screen)
    {
        if (screensInGame.Contains(screen))
            return;

        screensInGame.Add(screen);
        if (screensInGame != null)
        {
            SetupListsForEvents();
            SetCamerasForPlayers();
            StartGames();
        }
    }

    private void SetupListsForEvents()
    {
        foreach (OneScreenController screen in screensInGame)
        {
            if (!playerCanvases.Contains(screen.GetComponentInChildren<Canvas>()))
                playerCanvases.Add(screen.GetComponentInChildren<Canvas>());

            if (!movementManagers.Contains(screen.GetComponentInChildren<MovementManager>()))
                movementManagers.Add(screen.GetComponentInChildren<MovementManager>());
        }
    }

    private void StartGames()
    {
        EventManager.Instance.OnNumberOfScreensChanged(screensInGame.ToArray());
        EventManager.Instance.OnNumberOfScoreScreensChanged(playerCanvases);
        EventManager.Instance.OnNumberOfMovementManagersChanged(movementManagers);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_DespawnedObject(OneScreenController screen)
    {
        foreach (OneScreenController s in screensInGame)
        {
            if (s == screen)
            {
                screensInGame.Remove(s);
                break;
            }
        }

        SetCamerasForPlayers();
    }

    private void SetCamerasForPlayers()
    {
        Rect[] rects = SplitScreenManager.Instance.CalculateSplitRectangles(screensInGame.Count);

        for (int i = 0; i < screensInGame.Count; i++)
            screensInGame[i].SetCameraRect(rects[i]);

        if (screensInGame.Count == 2)
        {
            playerCanvases[0].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerCanvases[0].GetComponent<RectTransform>().rect.width / 2);
            playerCanvases[1].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerCanvases[1].GetComponent<RectTransform>().rect.width / 2);
        }
    }


    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("FailedToConnect");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("TryingToConnect");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log("PlayerLeftTheServer");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }
}
