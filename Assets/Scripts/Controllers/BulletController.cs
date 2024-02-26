using Fusion;
using UnityEngine;

public class BulletController : NetworkBehaviour
{
    private const float BulletSpeed = 100;
    private const int BulletDamage = 20;

    public float GetSpeed() => BulletSpeed;
    public int GetBulletDamage() => BulletDamage;
}
