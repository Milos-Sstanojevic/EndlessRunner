using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    private const int negator = -1;
    private int minimumHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private AnimationManager enemyAnimator;
    [SerializeField] private EnemyScriptableObject enemyScriptableObject;
    private int isOnEdge = -1;
    private int health;
    private bool hasRotated;
    private EnvironmentMovementController environmentComponent;

    private void Awake()
    {
        environmentComponent = GetComponent<EnvironmentMovementController>();
    }

    private void OnEnable()
    {
        health = enemyScriptableObject.health;
    }

    private void Start()
    {
        minimumHealth = enemyScriptableObject.minimumHealth;
    }

    private void Update()
    {
        MoveLeftAndRight();

        if (healthSlider.value != health)
        {
            healthSlider.value = health;
        }

        if (health <= minimumHealth)
        {
            health = enemyScriptableObject.fullHealth;
            EventManager.Instance.OnEnemyDestroyed(this);
            EventManager.Instance.OnEnemyKilled(enemyScriptableObject.enemyWorth);
        }

        Destroy();
    }

    private void MoveLeftAndRight()
    {
        if (environmentComponent.MovementEnabled == true)
        {
            enemyScriptableObject.MoveEnemy(this, enemyAnimator, isOnEdge);
            RotateEnemy();
        }
        else if (enemyAnimator != null)
            enemyAnimator.StopWalkAnimation();
    }

    private void RotateEnemy()
    {
        if (!hasRotated && enemyScriptableObject.IsEnemyOnEdge(transform.position))
        {
            transform.Rotate(enemyScriptableObject.faceTheOtherWay * isOnEdge);
            isOnEdge *= negator;
            hasRotated = true;
        }
        else if (!enemyScriptableObject.IsEnemyOnEdge(transform.position))
        {
            hasRotated = false;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    private void Destroy()
    {
        if (transform.position.z < GlobalConstants.PositionBehindPlayerAxisZ)
            EventManager.Instance.OnEnemyDestroyed(this);
    }
}
