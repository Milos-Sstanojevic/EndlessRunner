using System.Collections;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private const int OneScorePoint = 1;
    private const float AddPointsDelay = 0.5f;
    private int score;
    private bool canAddPoints;

    public void Initialize()
    {
        score = 0;
    }

    public void StartAddingPoints()
    {
        canAddPoints = true;
        StartCoroutine(AddPointsEachHalfSecond());
    }

    public void StopAddingPoints()
    {
        canAddPoints = false;
    }

    public void AddPoints(int points)
    {
        score += points;
        EventManager.Instance.OnChangeScoreOnScreen(score);
    }

    private IEnumerator AddPointsEachHalfSecond()
    {
        while (canAddPoints)
        {
            score += OneScorePoint;
            yield return new WaitForSeconds(AddPointsDelay);
            EventManager.Instance.OnChangeScoreOnScreen(score);
        }
    }
}
