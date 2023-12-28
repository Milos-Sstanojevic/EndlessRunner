using System;
using UnityEngine;
using static GlobalConstants;

public class SpaceshipController : MonoBehaviour, ICollectible, IDestroyable
{
    public static event Action OnSpaceshipCollected;
    public static event Action<SpaceshipController> OnDestroySpaceship;
    [SerializeField] private AudioManager audioManager;
    public float RotationSpeed;

    private void Update()
    {
        if (GameManager.Instance.IsGameActive)
        {
            RotateSpaceship();
            MoveSpaceship();

            if (transform.position.z < PositionBehindPlayerAxisZ)
            {
                Destroy();
            }
        }
    }

    private void RotateSpaceship()
    {
        transform.Rotate(Time.deltaTime * RotationSpeed * Vector3.forward);
    }

    private void MoveSpaceship()
    {
        transform.Translate(Vector3.back * GameManager.Instance.MovingSpeed * Time.deltaTime, Space.World);
    }

    public void Collect()
    {
        OnDestroySpaceship?.Invoke(this);
        OnSpaceshipCollected?.Invoke();
    }

    public void Destroy()
    {
        OnDestroySpaceship?.Invoke(this);
    }
}
