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
    public Dictionary<PlayerRef, NetworkObject> SpawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();
    private NetworkRunner runner;
    public PlayerRef playerRef;
    private List<OneScreenController> screensInGame = new List<OneScreenController>();
    private List<Canvas> playerCanvases = new List<Canvas>();
    private List<MovementManager> movementManagers = new List<MovementManager>();
    private List<PlayerController> playersInGame = new List<PlayerController>();


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
    }

    async void StartGame(GameMode mode)
    {
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "RunnerRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void EnterAsHost()
    {
        if (runner == null)
        {
            StartGame(GameMode.Host);
        }
    }

    public void EnterAsClient()
    {
        if (runner == null)
        {
            StartGame(GameMode.Client);
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            NetworkObject screen = SplitScreenManager.Instance.SpawnOnlineScreen(runner, player);
            NetworkObject playerOnScreen = SplitScreenManager.Instance.SpawnOnlinePlayer(runner, player);
            SplitScreenManager.Instance.InitializeNetworkedScreen(screen.GetComponent<OneScreenController>(), playerOnScreen.GetComponent<PlayerController>(), player);
            SpawnedScreens.Add(player, screen);
            SpawnedPlayers.Add(player, playerOnScreen);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (SpawnedScreens.TryGetValue(player, out NetworkObject screen))
        {
            runner.Despawn(screen);
            SpawnedScreens.Remove(player);
        }

        if (SpawnedPlayers.TryGetValue(player, out NetworkObject playerOnScreen))
        {
            runner.Despawn(playerOnScreen);
            SpawnedPlayers.Remove(player);
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public static NetworkInputData BufferedInput = new NetworkInputData();
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(BufferedInput);
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SpawnedScreen(OneScreenController screen)
    {
        if (screensInGame.Contains(screen))
            return;

        screensInGame.Add(screen);

        if (screensInGame != null)
            SetupListsForEvents();

        GameManager.Instance.StartGame();
        StartGames();
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_DespawnedScreen(OneScreenController screen)
    {
        foreach (OneScreenController s in screensInGame)
        {
            if (s == screen)
            {
                screensInGame.Remove(s);
                break;
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SpawnedPlayer(PlayerController player)
    {
        if (playersInGame.Contains(player))
            return;

        playersInGame.Add(player);
        if (playersInGame != null)
            SetCamerasForPlayers();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_DespawnedPlayer(PlayerController player)
    {
        foreach (PlayerController p in playersInGame)
        {
            if (p == player)
            {
                playersInGame.Remove(p);
                break;
            }
        }

        SetCamerasForPlayers();
    }

    private void SetupListsForEvents()
    {
        int i = 0;
        foreach (OneScreenController screen in screensInGame)
        {
            if (!playerCanvases.Contains(screen.GetComponentInChildren<Canvas>()))
                playerCanvases.Add(screen.GetComponentInChildren<Canvas>());

            if (!movementManagers.Contains(screen.GetComponentInChildren<MovementManager>()))
                movementManagers.Add(screen.GetComponentInChildren<MovementManager>());

            PlayerRef[] players = runner.ActivePlayers.ToArray();

            screen.GetComponentInChildren<SpawnManager>(true).SetOffsetToRespectedStage(new Vector3(players[i].PlayerId * SplitScreenManager.SpaceBetweenStages, 0, 0));

            SplitScreenManager.Instance.InitializeNetworkedScreen(screen, playersInGame[i], players[i]);

            i++;
        }
    }

    private void StartGames()
    {
        EventManager.Instance.OnNumberOfScreensChanged(screensInGame.ToArray());
        EventManager.Instance.OnNumberOfScoreScreensChanged(playerCanvases);
        EventManager.Instance.OnNumberOfMovementManagersChanged(movementManagers);
    }

    private void SetCamerasForPlayers()
    {
        Rect[] rects = SplitScreenManager.Instance.CalculateSplitRectangles(playersInGame.Count);

        for (int i = 0; i < playersInGame.Count; i++)
            playersInGame[i].SetCameraRect(rects[i]);

        if (screensInGame.Count == 2)
        {
            playerCanvases[0].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerCanvases[0].GetComponent<RectTransform>().rect.width / 2);
            playerCanvases[1].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerCanvases[1].GetComponent<RectTransform>().rect.width / 2);
        }
    }

    public NetworkRunner GetNetworkRunner() => runner;

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
