using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private const int ScoreStep = 100;
    private const int OneScorePoint = 1;
    private const float AddPointsDelay = 0.5f;
    [SerializeField] private TextMeshProUGUI playersScoreText;
    [SerializeField] private SpawnManager spawnManager;
    private int score;
    private int speedupRound = 1;
    private bool canAddPoints;

    public void Initialize()
    {
        score = 0;
    }

    private void OnEnable()
    {
        EventManager.Instance.SubscribeToOnEnemyKilledAction(AddPoints);
    }

    public void StartAddingPoints()
    {
        canAddPoints = true;
        StartCoroutine(AddPointsEachHalfSecond());
    }

    private IEnumerator AddPointsEachHalfSecond()
    {
        while (canAddPoints)
        {
            score += OneScorePoint;
            yield return new WaitForSeconds(AddPointsDelay);
            EventManager.Instance.OnChangeScoreOnScreen(score, playersScoreText);
            SpeedupGame();
        }
    }

    private void SpeedupGame()
    {
        if (score < ScoreStep * speedupRound)
            return;

        EventManager.Instance.OnDecreaseSpawningTimeOfChunk(spawnManager);
        //MovementManager.Instance.IncreaseMovementSpeed();
        EventManager.Instance.OnIncreaseSpeed(transform.parent.gameObject);
        speedupRound++;
    }

    public void StopAddingPoints()
    {
        canAddPoints = false;
    }

    public void AddPoints(int points)
    {
        score += points;
        EventManager.Instance.OnChangeScoreOnScreen(score, playersScoreText);
    }

}
