using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float gravityModifier;
    public ScoreManager ScoreManager { get; private set; }


    private void Awake()
    {
        ScoreManager = GetComponent<ScoreManager>();
        ScoreManager.Initialize();
    }

    private void Start()
    {
        Physics.gravity *= gravityModifier;
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        EventManager.Instance.SubscribeToStartAddingPointsAction(ScoreManager.StartAddingPoints);
        EventManager.Instance.SubscribeToStopAddingPointsAction(ScoreManager.StopAddingPoints);
    }

    private void OnDestroy()
    {
        Physics.gravity /= gravityModifier;
    }

    private void OnDisable()
    {
        EventManager.Instance.UnsubscribeFromStartAddingPointsAction(ScoreManager.StartAddingPoints);
        EventManager.Instance.UnsubscribeFromStopAddingPointsAction(ScoreManager.StopAddingPoints);
    }
}
