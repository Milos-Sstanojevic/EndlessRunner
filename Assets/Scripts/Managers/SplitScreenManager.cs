using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SplitScreenManager : MonoBehaviour
{
    public static SplitScreenManager Instance { get; private set; }
    private static readonly List<string> controlSchemes = new List<string> { "ArrowsControlls", "IJKLControlls", "NumControlls" };
    private const float SpaceBetweenStages = 94.77f;
    private const float StagePositionZ = 0;
    private const float StagePositionY = 0;

    [SerializeField] private Transform parentOfScreens;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private OneScreenController oneScreenInstance;
    [SerializeField] private Canvas startMenuCanvas;
    [SerializeField] private List<Canvas> playerCanvases;

    private List<MovementManager> movementManagers = new List<MovementManager>();
    private OneScreenController[] screenInstances;
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

    public void Split(int numberOfPlayers)
    {
        mainCamera.gameObject.SetActive(true);

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
                Destroy(instance);
            }
        }

        if (shouldReset)
        {
            playerCanvases[0].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerCanvases[0].GetComponent<RectTransform>().rect.width * 2);
            shouldReset = false;
        }

        playerCanvases.RemoveRange(1, playerCanvases.Count - 1);
        movementManagers.Clear();
    }

    private void EnableCamerasAndLevels(int numberOfPlayers)
    {
        Rect[] splitRects = CalculateSplitRectangles(numberOfPlayers);
        mainCamera.rect = splitRects[0];

        screenInstances = new OneScreenController[numberOfPlayers - 1];

        for (int i = 0; i < numberOfPlayers - 1; i++)
        {
            screenInstances[i] = Instantiate(oneScreenInstance, new Vector3((i + 1) * SpaceBetweenStages, StagePositionY, StagePositionZ), oneScreenInstance.transform.rotation);
            screenInstances[i].transform.SetParent(parentOfScreens);

            screenInstances[i].GetComponentInChildren<SpawnManager>(true).SetOffsetToRespectedStage(new Vector3((i + 1) * SpaceBetweenStages, 0, 0));

            Camera playerCamera = screenInstances[i].GetComponentInChildren<Camera>();

            playerCanvases.Add(playerCamera.GetComponentInChildren<Canvas>(true));

            screenInstances[i].GetComponentInChildren<PlayerInput>(true).SwitchCurrentControlScheme(controlSchemes[i], Keyboard.current);

            movementManagers.Add(screenInstances[i].GetComponentInChildren<MovementManager>());

            playerCamera.rect = splitRects[i + 1];
            playerCamera.gameObject.SetActive(true);
        }

        if (numberOfPlayers == 2)
        {
            shouldReset = true;
            playerCanvases[0].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerCanvases[0].GetComponent<RectTransform>().rect.width / 2);
            playerCanvases[1].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerCanvases[1].GetComponent<RectTransform>().rect.width / 2);
        }

    }


    private Rect[] CalculateSplitRectangles(int numberOfPlayers)
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
        // for (int i = 0; i < numberOfPlayers; i++)
        // {
        //     float columns = Mathf.Ceil(Mathf.Sqrt(numberOfPlayers));
        //     float rows = Mathf.Ceil((float)numberOfPlayers / columns);

        //     float width = 1.0f / columns;
        //     float height = 1.0f / rows;

        //     float xPos = (i % columns) * width;
        //     float yPos = Mathf.Floor(i / columns) * height;

        //     splitRects[i] = new Rect(xPos, yPos, width, height);
        // }

        return splitRects;
    }
}
