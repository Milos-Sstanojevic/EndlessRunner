using TMPro;
using UnityEngine;
public class UIManager : MonoBehaviour
{
    private const string ScoreText = "Score: ";
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject scoreScreen;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject gameOverScreen;
    private int score = 0;

    private void Start()
    {
        pointsText.text = ScoreText + score;
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
    public void SetEndScreenActive()
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
        scoreScreen.SetActive(true);
    }

    //Unity event, called when game state is Paused or GameOver
    public void SetScoreScreenInactive()
    {
        scoreScreen.SetActive(false);
    }

    public void SetGameOverScreenActive()
    {
        gameOverScreen.SetActive(true);
    }

    public void SetGameOverScreenInactive()
    {
        gameOverScreen.SetActive(false);
    }

    public void SetScoreOnScoreScreen(int score)
    {
        pointsText.text = ScoreText + score;
    }
}
