using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableEnemy", menuName = "EndlessRunner/Scriptable Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    private const float EdgePositionUpY = 4f;
    private const float EdgePositionDownY = 0f;
    public Vector3 faceTheOtherWay = new Vector3(0, 180, 0);
    public string enemyName;
    public int health = 100;
    public int minimumHealth = 0;
    public float movementSpeed = 6.5f;
    public bool isGroundEnemy;
    public int enemyWorth = 25;
    public int fullHealth = 100;

    public bool IsEnemyOnEdge(Vector3 position)
    {
        if (position.x <= -MapEdgeConstants.EdgePosX)
            return true;

        if (position.x >= MapEdgeConstants.EdgePosX)
            return true;

        if (position.y >= EdgePositionUpY)
            return true;

        if (position.y <= EdgePositionDownY)
            return true;

        return false;
    }
}
