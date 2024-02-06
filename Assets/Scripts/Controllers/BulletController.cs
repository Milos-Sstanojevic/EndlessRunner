using UnityEngine;

public class BulletController : MonoBehaviour
{
    private const float BulletSpeed = 100;
    private const int BulletDamage = 20;

    public float GetSpeed() => BulletSpeed;
    public int GetBulletDamage() => BulletDamage;
}
