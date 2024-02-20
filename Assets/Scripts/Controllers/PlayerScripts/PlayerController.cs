using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public ScoreManager ScoreManager { get; private set; }
    private bool isPlayerDead;

    private void Awake()
    {
        ScoreManager = GetComponent<ScoreManager>();
        ScoreManager.Initialize();
    }

    private void Start()
    {
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        EventManager.Instance.SubscribeToStartAddingPointsAction(ScoreManager.StartAddingPoints);
        EventManager.Instance.SubscribeToStopAddingPointsAction(ScoreManager.StopAddingPoints);
    }

    public void PlayerDied()
    {
        isPlayerDead = true;
    }

    public bool IsPlayerDead() => isPlayerDead;


    private void OnDisable()
    {
        EventManager.Instance.UnsubscribeFromStartAddingPointsAction(ScoreManager.StartAddingPoints);
        EventManager.Instance.UnsubscribeFromStopAddingPointsAction(ScoreManager.StopAddingPoints);
    }
}
