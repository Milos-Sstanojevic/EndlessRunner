using UnityEngine;
using UnityEngine.UI;

public class EnemyController : EnviromentMovementBase
{
    private const int negator = -1;
    private int minimumHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private AnimationManager enemyAnimator;
    [SerializeField] private EnemyScriptableObject enemyScriptableObject;
    private int isOnEdge = -1;
    private int health;

    private void Start()
    {
        health = enemyScriptableObject.health;
        minimumHealth = enemyScriptableObject.minimumHealth;
    }

    protected override void Update()
    {
        base.Update();
        MoveLeftAndRight();

        if (healthSlider.value != health)
        {
            healthSlider.value = health;
        }

        if (health <= minimumHealth)
        {
            health = enemyScriptableObject.fullHealth;
            EventManager.Instance.OnEnviromentDestroyed(this);
            EventManager.Instance.OnEnemyKilled(enemyScriptableObject.enemyWorth);
        }
    }

    private void MoveLeftAndRight()
    {
        if (base.MovementEnabled == true)
        {
            enemyScriptableObject.MoveEnemy(this, enemyAnimator, isOnEdge);
            RotateEnemy();
        }
        else
            enemyAnimator.StopWalkAnimation();
    }

    private void RotateEnemy()
    {
        if (enemyScriptableObject.IsEnemyOnEdge(transform.position))
        {
            transform.Rotate(enemyScriptableObject.faceTheOtherWay * isOnEdge);
            isOnEdge *= negator;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }
}
