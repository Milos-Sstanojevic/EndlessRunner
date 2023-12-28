using System;
using UnityEngine;
using static GlobalConstants;

public class SpaceshipController : MonoBehaviour, ICollectible, IDestroyable
{
    public static event Action OnSpaceshipCollected;
    public static event Action<SpaceshipController> OnDestroySpaceship;
    public AudioClip SpaceshipCollectedSound;
    public float RotationSpeed;

    void Update()
    {
        if (GameManager.Instance.IsGameActive)
        {
            RotateSpaceship();
            MoveSpaceship();

            if (transform.position.z < PositionBehindPlayerAxisZ)//pokreni akciju koja ce da uradi Release u pool za brod ako prodje iza player-a
            {
                Destroy();
            }
        }
    }

    void RotateSpaceship()
    {
        transform.Rotate(Time.deltaTime * RotationSpeed * Vector3.forward);
    }

    void MoveSpaceship()
    {
        transform.Translate(Vector3.back * GameManager.Instance.MovingSpeed * Time.deltaTime, Space.World);
    }

    public void Collect()
    {
        AudioSource.PlayClipAtPoint(SpaceshipCollectedSound, transform.position);
        OnDestroySpaceship?.Invoke(this);
        OnSpaceshipCollected?.Invoke();
    }

    public void Destroy()
    {
        OnDestroySpaceship?.Invoke(this);
    }
}
