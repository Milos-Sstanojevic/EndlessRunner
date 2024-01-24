using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float bulletSpeed = 100;
    private int bulletDamage = 20;

    public float GetSpeed() => bulletSpeed;
    public int GetBulletDamage() => bulletDamage;
}
