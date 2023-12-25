using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour, ICollectible
{
    public static event Action OnSpaceshipCollected;
    private static event Action<SpaceshipController> destroySpaceship;


    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsGameActive)
        {
            RotateSpaceship();
            MoveSpaceship();

            if (transform.position.z < -12)//pokreni akciju koja ce da uradi Release u pool za brod ako prodje iza player-a
            {
                destroySpaceship(this);
            }
        }
    }

    void RotateSpaceship()
    {
        transform.Rotate(Time.deltaTime * rotationSpeed * Vector3.forward);
    }

    void MoveSpaceship()
    {
        transform.Translate(Vector3.back * GameManager.Instance.MovingSpeed * Time.deltaTime, Space.World);
    }

    public void Init(Action<SpaceshipController> destroyS)
    {
        destroySpaceship = destroyS;
    }

    public void Collect()
    {
        destroySpaceship(this);
        OnSpaceshipCollected?.Invoke();
    }
}
