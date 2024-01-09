using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static GlobalConstants;
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject scoreScreen;
    [SerializeField] private TextMeshProUGUI pointsText;

    private int score = 0;

    private void Start()
    {
        pointsText.text = ScoreText + score;
    }

    public void SetStartScreenActive() => startScreen.SetActive(true);

    public void SetStartScreenInactive() => startScreen.SetActive(false);

    public void SetEndScreenActive() => endScreen.SetActive(true);

    public void SetEndScreenInactive() => endScreen.SetActive(false);

    public void SetPauseScreenActive() => pauseScreen.SetActive(true);

    public void SetPauseScreenInactive() => pauseScreen.SetActive(false);

    public void SetScoreScreenActive() => scoreScreen.SetActive(true);

    public void SetScoreScreenInactive() => scoreScreen.SetActive(false);

    public int GetScore() => score;

    public void SetScoreOnScoreScreen(int points)
    {
        score += points;
        pointsText.text = ScoreText + score;
    }
}
