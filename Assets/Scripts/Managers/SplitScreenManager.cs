using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplitScreenManager : MonoBehaviour
{
    public static SplitScreenManager Instance { get; private set; }
    private const float SpaceBetweenStages = 94.77f;
    private const float StagePositionZ = 0;
    private const float StagePositionY = 0;
    [SerializeField] private Transform parentOfScreens;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject playerAndLevelPrefab;
    [SerializeField] private Canvas startMenuCanvas;
    [SerializeField] private GameObject level;
    private GameObject[] screenInstances;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
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
        AdjustUI(numberOfPlayers);
        EventManager.Instance.OnNumberOfScreensChanged(screenInstances);
    }

    private void CleanupScene()
    {
        if (screenInstances != null)
        {
            foreach (GameObject instance in screenInstances)
            {
                Destroy(instance);
            }
        }
    }

    private void EnableCamerasAndLevels(int numberOfPlayers)
    {
        Rect[] splitRects = CalculateSplitRectangles(numberOfPlayers);
        mainCamera.rect = splitRects[0];

        screenInstances = new GameObject[numberOfPlayers - 1];

        for (int i = 0; i < numberOfPlayers - 1; i++)
        {
            screenInstances[i] = Instantiate(playerAndLevelPrefab, new Vector3(-(i + 1) * SpaceBetweenStages, StagePositionY, StagePositionZ), playerAndLevelPrefab.transform.rotation);
            screenInstances[i].transform.SetParent(parentOfScreens);
            Camera playerCamera = screenInstances[i].GetComponentInChildren<Camera>();

            if (playerCamera != null)
            {
                playerCamera.rect = splitRects[i + 1];
                playerCamera.gameObject.SetActive(true);
            }
        }
    }

    private void AdjustUI(int numberOfPlayers)
    {
        CanvasScaler canvasScaler = startMenuCanvas.GetComponent<CanvasScaler>();
        Rect[] splitRects = CalculateSplitRectangles(numberOfPlayers);

        for (int i = 0; i < numberOfPlayers; i++)
        {
            Rect normalizedRect = splitRects[i];
            Vector2 referenceResolution = new Vector2(Screen.width * normalizedRect.width, Screen.height * normalizedRect.height);

            canvasScaler.referenceResolution = referenceResolution;
            startMenuCanvas.renderMode = RenderMode.WorldSpace;

            RectTransform canvasRect = startMenuCanvas.GetComponent<RectTransform>();
            canvasRect.anchorMin = new Vector2(normalizedRect.x, normalizedRect.y);
            canvasRect.anchorMax = new Vector2(normalizedRect.x + normalizedRect.width, normalizedRect.y + normalizedRect.height);
            canvasRect.offsetMin = Vector2.zero;
            canvasRect.offsetMax = Vector2.zero;

            startMenuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }

    private Rect[] CalculateSplitRectangles(int numberOfPlayers)
    {
        Rect[] splitRects = new Rect[numberOfPlayers];

        if (numberOfPlayers == 2)
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
