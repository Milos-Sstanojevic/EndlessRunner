
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class SplitScreenManager : NetworkBehaviour
{
    public static SplitScreenManager Instance { get; private set; }
    private static readonly List<string> controlSchemes = new List<string> { "WASDControlls", "ArrowsControlls", "IJKLControlls", "NumControlls" };
    public const float SpaceBetweenStages = 94.77f;
    private const float StagePositionZ = 0;
    private const float StagePositionY = 0;

    [SerializeField] private Transform parentOfScreens;
    [SerializeField] private NetworkPrefabRef oneScreenInstancePrefabNetwork;
    [SerializeField] private NetworkPrefabRef playerInstancePrefabNetwork;
    [SerializeField] private OneScreenController oneScreenInstance;
    [SerializeField] private Canvas startMenuCanvas;
    [SerializeField] private List<Canvas> playerCanvases;
    [SerializeField] private GameObject cameraSplitController;

    private List<MovementManager> movementManagers = new List<MovementManager>();

    [SerializeField] private OneScreenController[] screenInstances;

    private bool shouldReset;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }


    public NetworkObject SpawnOnlineScreen(NetworkRunner runner, PlayerRef player)
    {
        return runner.Spawn(oneScreenInstancePrefabNetwork, new Vector3(player.PlayerId * SpaceBetweenStages, StagePositionY, StagePositionZ), Quaternion.identity, player);
    }

    public NetworkObject SpawnOnlinePlayer(NetworkRunner runner, PlayerRef player)
    {
        NetworkObject playerOnScreen = runner.Spawn(playerInstancePrefabNetwork, new Vector3(player.PlayerId * SpaceBetweenStages, 0.5f, -10.5f), Quaternion.identity, player);
        playerOnScreen.GetComponent<PlayerController>().SetPlayerId(player.PlayerId);

        return playerOnScreen;
    }


    public void InitializeNetworkedScreen(OneScreenController screen, PlayerController playerOnScreen)
    {
        playerOnScreen.name = $"Player: {playerOnScreen.PlayerId}";
        playerOnScreen.GetComponent<PlayerMovement>().SetStartPositionOfOfPlayer(playerOnScreen.PlayerId * SpaceBetweenStages);
        screen.name = $"Screen: {playerOnScreen.PlayerId}";
        screen.transform.SetParent(parentOfScreens);
        screen.GetComponent<OneScreenController>().SetPlayerControllerOnScreen(playerOnScreen.GetComponent<PlayerController>());
        screen.GetComponentInChildren<MovementManager>().SetPlayerMovementComponentOnScreen(playerOnScreen.GetComponent<PlayerMovement>());

        playerOnScreen.SetScreenOfPlayer(screen);
        playerOnScreen.GetComponent<PlayerController>().SetScreenOfPlayer(screen.GetComponent<OneScreenController>());
        playerOnScreen.GetComponent<ScoreManager>().SetSpawnManager(screen.GetComponentInChildren<SpawnManager>(true));
        playerOnScreen.GetComponent<ScoreManager>().SetScreenController(screen);

        screen.GetComponentInChildren<SpawnManager>(true).SetOffsetToRespectedStage(new Vector3(playerOnScreen.PlayerId * SpaceBetweenStages, 0, 0));
    }

    public void Split(int numberOfPlayers)
    {
        CleanupScene();
        EnableCamerasAndLevels(numberOfPlayers);
        EventManager.Instance.OnNumberOfScreensChanged(screenInstances);
        EventManager.Instance.OnNumberOfScoreScreensChanged(playerCanvases);
        EventManager.Instance.OnNumberOfMovementManagersChanged(movementManagers);
    }

    private void CleanupScene()
    {
        if (screenInstances != null)
        {
            foreach (OneScreenController instance in screenInstances)
            {
                Destroy(instance.gameObject);
            }
        }

        if (shouldReset)
        {
            playerCanvases[0].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerCanvases[0].GetComponent<RectTransform>().rect.width * 2);
            shouldReset = false;
        }

        playerCanvases.Clear();
        movementManagers.Clear();
    }

    private void EnableCamerasAndLevels(int numberOfPlayers)
    {
        Rect[] splitRects = CalculateSplitRectangles(numberOfPlayers);

        screenInstances = new OneScreenController[numberOfPlayers];

        for (int i = 0; i < numberOfPlayers; i++)
        {
            screenInstances[i] = Instantiate(oneScreenInstance, new Vector3((i + 1) * SpaceBetweenStages, StagePositionY, StagePositionZ), oneScreenInstance.transform.rotation);
            screenInstances[i].transform.SetParent(parentOfScreens);

            screenInstances[i].GetComponentInChildren<SpawnManager>(true).SetOffsetToRespectedStage(new Vector3((i + 1) * SpaceBetweenStages, 0, 0));

            Camera playerCamera = screenInstances[i].GetComponentInChildren<Camera>();

            playerCanvases.Add(playerCamera.GetComponentInChildren<Canvas>(true));

            if (i == 0)
                screenInstances[i].GetComponentInChildren<PlayerInput>(true).SwitchCurrentControlScheme(controlSchemes[i], new InputDevice[] { Keyboard.current, Mouse.current });

            else
                screenInstances[i].GetComponentInChildren<PlayerInput>(true).SwitchCurrentControlScheme(controlSchemes[i], Keyboard.current);

            movementManagers.Add(screenInstances[i].GetComponentInChildren<MovementManager>());

            playerCamera.rect = splitRects[i];
            playerCamera.gameObject.SetActive(true);
        }

        if (numberOfPlayers == 2)
        {
            shouldReset = true;
            playerCanvases[0].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerCanvases[0].GetComponent<RectTransform>().rect.width / 2);
            playerCanvases[1].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerCanvases[1].GetComponent<RectTransform>().rect.width / 2);
        }
    }


    public Rect[] CalculateSplitRectangles(int numberOfPlayers)
    {
        Rect[] splitRects = new Rect[numberOfPlayers];

        if (numberOfPlayers == 1)
        {
            splitRects[0] = new Rect(0, 0, 1, 1);
        }
        else if (numberOfPlayers == 2)
        {
            splitRects[0] = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
            splitRects[1] = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
        }
        else if (numberOfPlayers == 3)
        {
            splitRects[0] = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
            splitRects[1] = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            splitRects[2] = new Rect(0.0f, 0.0f, 1.0f, 0.5f);
        }
        else if (numberOfPlayers == 4)
        {
            splitRects[0] = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
            splitRects[1] = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            splitRects[2] = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
            splitRects[3] = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
        }

        return splitRects;
    }
}
