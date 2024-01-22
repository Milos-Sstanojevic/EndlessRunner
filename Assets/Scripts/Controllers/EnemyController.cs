
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : EnviromentMovementBase
{
    private static Vector3 faceTheOtherWay = new Vector3(0, -180, 0);
    private const int fullHealth = 100;
    private const int minimumHealth = 0;
    private const int negator = -1;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private AnimationManager enemyAnimator;
    private int isOnLeftEdge = -1;
    private float movementSpeed = 6.5f;
    private int health = 100;

    private void OnEnable()
    {
        Debug.Log(health);
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
            health = fullHealth;
            EventManager.Instance.OnEnviromentDestroyed(this);
        }
    }

    private void MoveLeftAndRight()
    {
        if (base.MovementEnabled == true)
        {
            enemyAnimator.StartWalkAnimation();
            transform.Translate(Vector3.left * isOnLeftEdge * movementSpeed * Time.deltaTime, Space.World);
            RotateEnemy();
        }
        else
            enemyAnimator.StopWalkAnimation();
    }

    private void RotateEnemy()
    {
        if (transform.position.x < -GlobalConstants.EdgePosX)
        {
            transform.Rotate(faceTheOtherWay);
            isOnLeftEdge *= negator;
        }
        if (transform.position.x > GlobalConstants.EdgePosX)
        {
            transform.Rotate(-faceTheOtherWay);
            isOnLeftEdge *= negator;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }
}
