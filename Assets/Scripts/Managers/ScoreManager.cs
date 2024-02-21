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
        EventManager.Instance.SubscribeToOnPlayerDeadAction(StopAddingPointsForThisPlayer);
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
        EventManager.Instance.OnIncreaseSpeed(transform.GetComponentInParent<OneScreenController>());
        speedupRound++;
    }

    public void StopAddingPointsForThisPlayer(PlayerController player, GameObject endScreen)
    {
        if (player == GetComponent<PlayerController>())
            canAddPoints = false;
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
