using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Addons.Physics;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class NetworkSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkSpawner Instance { get; set; }

    private const int Seed = 22012002;
    private const int TwoPlayers = 2;
    public Dictionary<PlayerRef, NetworkObject> SpawnedScreens = new Dictionary<PlayerRef, NetworkObject>();
    public Dictionary<PlayerRef, NetworkObject> SpawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();
    private NetworkRunner runner;
    public PlayerRef playerRef;
    private List<OneScreenController> screensInGame = new List<OneScreenController>();
    private List<Canvas> playerCanvases = new List<Canvas>();
    private List<MovementManager> movementManagers = new List<MovementManager>();
    private List<PlayerController> playersInGame = new List<PlayerController>();
    public static NetworkInputData BufferedInput = new NetworkInputData();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
    }

    private void OnEnable()
    {
        // EventManager.Instance.SubscribeToOnPlayerReadyAction(StartGames);
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

        gameObject.AddComponent<RunnerSimulatePhysics3D>();
    }

    //Called when player presses Host button 
    public void EnterAsHost()
    {
        if (runner == null)
        {
            StartGame(GameMode.Host);
        }
    }

    //Called when player presses Join button 
    public void EnterAsClient()
    {
        if (runner == null)
        {
            StartGame(GameMode.Client);

            //ovde mozes da proveravas dal neko poksava da se ukljuci tako da mora njegov ready da se ceka
            //isto ovde treba da aktiviras kanvas sa ready dugmetom
        }
    }


    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            NetworkObject screen = SplitScreenManager.Instance.SpawnOnlineScreen(runner, player);
            NetworkObject playerOnScreen = SplitScreenManager.Instance.SpawnOnlinePlayer(runner, player);
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
            playerCanvases.Remove(playerOnScreen.GetComponentInChildren<Canvas>());
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

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
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StartGames(int numberOfPlayersReady)
    {
        foreach (PlayerController player in playersInGame)
        {
            player.IncreaseNumberOfPlayersReady();
        }

        numberOfPlayersReady++;

        if (numberOfPlayersReady != playersInGame.Count + 1)
            return;

        WaitForPlayersToBeReady();
    }

    private void WaitForPlayersToBeReady()
    {
        EventManager.Instance.OnNumberOfScreensChanged(screensInGame.ToArray());
        EventManager.Instance.OnNumberOfScoreScreensChanged(playerCanvases);
        EventManager.Instance.OnNumberOfMovementManagersChanged(movementManagers);
        GameManager.Instance.StartGame();
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

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_PlayerDied(int id, PlayerController playerController, GameObject gameOverScreen)
    {
        foreach (PlayerController player in playersInGame)
            if (player.PlayerId == playerController.PlayerId)
                EventManager.Instance.OnPlayerDead(id, playerController, gameOverScreen);
    }

    private void SetupListsForEvents()
    {
        int i = 0;
        foreach (OneScreenController screen in screensInGame)
        {
            if (!movementManagers.Contains(screen.GetComponentInChildren<MovementManager>()))
                movementManagers.Add(screen.GetComponentInChildren<MovementManager>());

            PlayerRef[] players = runner.ActivePlayers.ToArray();
            screen.GetComponentInChildren<SpawnManager>(true).SetOffsetToRespectedStage(new Vector3(playersInGame[i].PlayerId * SplitScreenManager.SpaceBetweenStages, 0, 0));

            SplitScreenManager.Instance.InitializeNetworkedScreen(screen, playersInGame[i]);

            i++;
        }

        foreach (PlayerController player in playersInGame)
        {
            if (!playerCanvases.Contains(player.GetComponentInChildren<Canvas>()))
                playerCanvases.Add(player.GetComponentInChildren<Canvas>());
        }
    }

    private void SetCamerasForPlayers()
    {
        Rect[] rects = SplitScreenManager.Instance.CalculateSplitRectangles(playersInGame.Count);

        for (int i = 0; i < playersInGame.Count; i++)
        {
            playersInGame[i].SetCameraRect(rects[i]);
        }

        if (screensInGame.Count == TwoPlayers)
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
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
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
