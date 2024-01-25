using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuntBullet : MonoBehaviour
{
    [SerializeField] private int active;
    [SerializeField] private int inactive;
    [SerializeField] private int all;

    private void Update()
    {
        all = BulletPoolyingSystem.Instance.GetCountAll();
        active = BulletPoolyingSystem.Instance.GetCountActive();
        inactive = BulletPoolyingSystem.Instance.GetCountInactive();
    }
}
