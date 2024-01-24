using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ScriptableEnemy", menuName = "EndlessRunner/Scriptable Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    public Vector3 faceTheOtherWay = new Vector3(0, 180, 0);
    private static float edgePositionUpY = 4f;
    private static float edgePositionDownY = 0f;
    public string enemyName;
    public int health = 100;
    public int minimumHealth = 0;
    public float movementSpeed = 6.5f;
    public bool isGroundEnemy;
    public int enemyWorth = 25;
    public int fullHealth = 100;


    public void MoveEnemy(EnemyController enemy, AnimationManager enemyAnimator, int isOnEdge)
    {
        if (!isGroundEnemy)
        {
            enemy.transform.Translate(Vector3.up * isOnEdge * movementSpeed * Time.deltaTime, Space.World);

        }
        if (isGroundEnemy)
        {
            enemyAnimator.StartWalkAnimation();
            enemy.transform.Translate(Vector3.left * isOnEdge * movementSpeed * Time.deltaTime, Space.World);
        }
    }

    public void DisplayAndUpdateHealth(EnemyController enemy, Slider healthSlider)
    {

        if (healthSlider.value != health)
        {
            healthSlider.value = health;
        }

        if (health <= minimumHealth)
        {
            health = fullHealth;
            EventManager.Instance.OnEnviromentDestroyed(enemy);
            EventManager.Instance.OnEnemyKilled(enemyWorth);
        }
    }

    public bool IsEnemyOnEdge(Vector3 position)
    {
        if (position.x <= -GlobalConstants.EdgePosX)
            return true;
        if (position.x >= GlobalConstants.EdgePosX)
            return true;
        if (position.y >= edgePositionUpY)
            return true;
        if (position.y <= edgePositionDownY)
            return true;

        return false;
    }
}
