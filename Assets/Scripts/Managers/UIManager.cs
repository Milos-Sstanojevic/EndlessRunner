using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private const string ScoreText = "Score: ";
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private TextMeshProUGUI numberOfPlayersText;
    [SerializeField] private GameObject numberOfPlayersScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private List<Canvas> playerCanvases;

    private void OnEnable()
    {
        EventManager.Instance.SubscribeToOnChangeNumberOfPlayersAction(ChangeNumberOfPlayers);
        EventManager.Instance.SubscribeToOnNumberOfPlayersSavedAction(SetNumberOfPlayersScreenInactive);
        EventManager.Instance.SubscribeToNumberOfScoreScreensChangedAction(SetScoreScreens);
    }

    private void SetScoreScreens(List<Canvas> screens)
    {
        if (screens != null)
            playerCanvases.Clear();
        foreach (Canvas screen in screens)
            playerCanvases.Add(screen);
    }

    private void ChangeNumberOfPlayers(int number)
    {
        numberOfPlayersText.text = number.ToString();
    }

    //Called when Cancel button is clicked in menu fro choosing number of players
    public void CancelSettingNumberOfPlayers()
    {
        numberOfPlayersScreen.SetActive(false);
        startScreen.SetActive(true);
    }

    public void SetNumberOfPlayersScreenInactive()
    {
        numberOfPlayersScreen.SetActive(false);
        startScreen.SetActive(true);
    }

    public void SetNumberOfPlayersScreenActive()
    {
        numberOfPlayersScreen.SetActive(true);
    }

    public void SetSettingsScreenActive()
    {
        settingsScreen.SetActive(true);
    }

    public void SetSettingsScreenInactive()
    {
        settingsScreen.SetActive(false);
    }

    //Unity event, called when play button is pressed
    public void SetStartScreenInactive()
    {
        startScreen.SetActive(false);
    }

    public void SetStartScreenActive()
    {
        startScreen.SetActive(true);
    }

    //Unity event, called when player is dead
    public void SetEndScreenActive(GameObject endScreen)
    {
        endScreen.SetActive(true);
    }

    //Unity event, called when player presses ESC on keyboard
    public void SetPauseScreenActive()
    {
        pauseScreen.SetActive(true);
    }

    //Unity event, called when player presses continue button
    public void SetPauseScreenInactive()
    {
        pauseScreen.SetActive(false);
    }

    //Unity event, called when game state is Playing
    public void SetScoreScreenActive()
    {
        if (playerCanvases != null)
            foreach (Canvas playerScreen in playerCanvases)
            {
                TextMeshProUGUI points = playerScreen.GetComponentInChildren<TextMeshProUGUI>(true);
                if (points != null)
                    points.gameObject.SetActive(true);
            }
    }

    //Unity event, called when game state is Paused or GameOver
    public void SetScoreScreenInactive(PlayerController player)
    {
        if (playerCanvases != null)
            foreach (Canvas playerScreen in playerCanvases)
            {
                if (playerScreen.GetComponentInParent<PlayerController>() == player)
                {
                    TextMeshProUGUI points = playerScreen.GetComponentInChildren<TextMeshProUGUI>(true);
                    if (points != null)
                        points.gameObject.SetActive(false);
                }
            }
    }

    public void SetGameOverScreenActive()
    {
        gameOverScreen.SetActive(true);
    }

    public void SetGameOverScreenInactive()
    {
        gameOverScreen.SetActive(false);
    }

    public void SetScoreOnScoreScreen(int score, TextMeshProUGUI playerTextScore)
    {
        playerTextScore.text = ScoreText + score;
    }

    private void OnDisable()
    {
        EventManager.Instance.UnsubscribeFromOnChangeNumberOfPlayersAction(ChangeNumberOfPlayers);
        EventManager.Instance.UnsubscribeFromOnNumberOfPlayersSavedAction(SetNumberOfPlayersScreenInactive);
    }
}