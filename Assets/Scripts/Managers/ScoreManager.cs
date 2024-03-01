using System.Collections;
using Fusion;
using TMPro;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    private const int ScoreStep = 100;
    private const int OneScorePoint = 1;
    private const float AddPointsDelay = 0.5f;
    [SerializeField] private TextMeshProUGUI playersScoreText;
    [SerializeField] private SpawnManager spawnManager;
    private int score;
    private int speedupRound = 1;
    private bool canAddPoints;
    private OneScreenController screenOfThisScore;

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
        EventManager.Instance.OnIncreaseSpeed(screenOfThisScore);
        speedupRound++;
    }

    public void StopAddingPointsForThisPlayer(int id, PlayerController player, GameObject endScreen)
    {
        if (player == GetComponent<PlayerController>() && GetComponent<PlayerController>().GetPlayerId() == id)
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

    public void SetSpawnManager(SpawnManager spawn)
    {
        spawnManager = spawn;
    }

    public void SetScreenController(OneScreenController screen)
    {
        screenOfThisScore = screen;
    }

    private void OnDestroy()
    {
        EventManager.Instance.UnsubscribeFromOnEnemyKilledAction(AddPoints);
        EventManager.Instance.UnsubscribeFromOnPlayerDeadAction(StopAddingPointsForThisPlayer);
    }
}
