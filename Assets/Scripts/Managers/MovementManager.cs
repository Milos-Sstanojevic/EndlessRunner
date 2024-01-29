using System;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public static MovementManager Instance { get; private set; }
    private const float speedIncrease = 1f;
    private const float playerSpeedBalancer = 1;
    [SerializeField] private PlayerController player;
    [SerializeField] private List<StageController> stagesInGame;
    private List<EnvironmentMovementBase> obstaclesInGame;
    private List<GunController> gunsInGame;
    private List<JetController> jetsInGame;
    private List<CollectableBase> spaceshipsInGame;
    private List<EnemyController> enemiesInGame;
    private float speed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        obstaclesInGame = new List<EnvironmentMovementBase>();
        gunsInGame = new List<GunController>();
        jetsInGame = new List<JetController>();
        spaceshipsInGame = new List<CollectableBase>();
        enemiesInGame = new List<EnemyController>();

        speed = 8;
    }

    private void Update()
    {
        GetCollectablesAndObstaclesInGame();
    }

    public void GetCollectablesAndObstaclesInGame()
    {
        obstaclesInGame = ObstaclesPoolingSystem.Instance.GetInstantiatedObjects();
        gunsInGame = GunPoolingSystem.Instance.GetInstantiatedObjects();
        jetsInGame = JetPoolingSystem.Instance.GetInstantiatedObjects();
        spaceshipsInGame = SpaceshipPoolingSystem.Instance.GetInstantiatedObjects();
        enemiesInGame = EnemyPoolingSystem.Instance.GetInstantiatedObjects();
    }

    private void PerformActionOnObjects<T>(List<T> objects, Action<T> action) where T : MonoBehaviour
    {
        foreach (T obj in objects)
        {
            EnvironmentMovementBase envMovement = obj.GetComponent<EnvironmentMovementBase>();
            if (envMovement != null)
            {
                action.Invoke(obj);
            }
        }
    }

    public void EnableMovement()
    {
        // foreach (EnvironmentMovementBase obs in obstaclesInGame)
        // {
        //     obs.EnableMovement();
        // }

        // foreach (GunController gun in gunsInGame)
        // {
        //     gun.GetComponent<EnvironmentMovementBase>().EnableMovement();
        //     gun.UnpauseCoroutine();
        // }

        // foreach (StageController stage in stagesInGame)
        // {
        //     stage.GetComponent<EnvironmentMovementBase>().EnableMovement();
        // }

        // foreach (JetController jet in jetsInGame)
        // {
        //     jet.GetComponent<EnvironmentMovementBase>().EnableMovement();
        //     jet.UnpauseCoroutine();
        // }

        // foreach (CollectableBase spaceship in spaceshipsInGame)
        // {
        //     spaceship.GetComponent<EnvironmentMovementBase>().EnableMovement();
        // }

        // foreach (EnemyController enemy in enemiesInGame)
        // {
        //     enemy.GetComponent<EnvironmentMovementBase>().EnableMovement();
        // }

        PerformActionOnObjects(obstaclesInGame, obs => obs.EnableMovement());
        PerformActionOnObjects(gunsInGame, gun =>
        {
            gun.GetComponent<EnvironmentMovementBase>().EnableMovement();
            gun.UnpauseCoroutine();
        });
        PerformActionOnObjects(stagesInGame, stage => stage.GetComponent<EnvironmentMovementBase>().EnableMovement());
        PerformActionOnObjects(jetsInGame, jet =>
        {
            jet.GetComponent<EnvironmentMovementBase>().EnableMovement();
            jet.UnpauseCoroutine();
        });
        PerformActionOnObjects(spaceshipsInGame, spaceship => spaceship.GetComponent<EnvironmentMovementBase>().EnableMovement());
        PerformActionOnObjects(enemiesInGame, enemy => enemy.GetComponent<EnvironmentMovementBase>().EnableMovement());

        player.EnableMovement();
    }

    public void SetMovementSpeed()
    {
        // foreach (EnvironmentMovementBase movable in obstaclesInGame)
        // {
        //     movable.SetMovementSpeed(speed);
        // }

        // foreach (StageController stage in stagesInGame)
        // {
        //     stage.GetComponent<EnvironmentMovementBase>().SetMovementSpeed(speed);
        // }

        // foreach (GunController gun in gunsInGame)
        // {
        //     gun.GetComponent<EnvironmentMovementBase>().SetMovementSpeed(speed);
        // }

        // foreach (JetController jet in jetsInGame)
        // {
        //     jet.GetComponent<EnvironmentMovementBase>().SetMovementSpeed(speed);
        // }

        // foreach (CollectableBase spaceship in spaceshipsInGame)
        // {
        //     spaceship.GetComponent<EnvironmentMovementBase>().SetMovementSpeed(speed);
        // }

        // foreach (EnemyController enemy in enemiesInGame)
        // {
        //     enemy.GetComponent<EnvironmentMovementBase>().SetMovementSpeed(speed);
        // }
        PerformActionOnObjects(obstaclesInGame, movable => movable.SetMovementSpeed(speed));
        PerformActionOnObjects(stagesInGame, stage => stage.GetComponent<EnvironmentMovementBase>().SetMovementSpeed(speed));
        PerformActionOnObjects(gunsInGame, gun => gun.GetComponent<EnvironmentMovementBase>().SetMovementSpeed(speed));
        PerformActionOnObjects(jetsInGame, jet => jet.GetComponent<EnvironmentMovementBase>().SetMovementSpeed(speed));
        PerformActionOnObjects(spaceshipsInGame, spaceship => spaceship.GetComponent<EnvironmentMovementBase>().SetMovementSpeed(speed));
        PerformActionOnObjects(enemiesInGame, enemy => enemy.GetComponent<EnvironmentMovementBase>().SetMovementSpeed(speed));

        player.SetMovementSpeed(speed + playerSpeedBalancer);
    }

    public void DisableMovementOfMovableObjects()
    {
        // foreach (EnvironmentMovementBase movable in obstaclesInGame)
        // {
        //     movable.DisableMovement();
        // }

        // foreach (JetController jet in jetsInGame)
        // {
        //     jet.GetComponent<EnvironmentMovementBase>().DisableMovement();
        //     jet.PauseCoroutine();
        // }

        // foreach (GunController gun in gunsInGame)
        // {
        //     gun.GetComponent<EnvironmentMovementBase>().DisableMovement();
        //     gun.PauseCoroutine();
        // }

        // foreach (StageController stage in stagesInGame)
        // {
        //     stage.GetComponent<EnvironmentMovementBase>().DisableMovement();
        // }

        // foreach (CollectableBase spaceship in spaceshipsInGame)
        // {
        //     spaceship.GetComponent<EnvironmentMovementBase>().DisableMovement();
        // }

        // foreach (EnemyController enemy in enemiesInGame)
        // {
        //     enemy.GetComponent<EnvironmentMovementBase>().DisableMovement();
        // }
        PerformActionOnObjects(obstaclesInGame, movable => movable.DisableMovement());
        PerformActionOnObjects(jetsInGame, jet =>
        {
            jet.GetComponent<EnvironmentMovementBase>().DisableMovement();
            jet.PauseCoroutine();
        });
        PerformActionOnObjects(gunsInGame, gun =>
        {
            gun.GetComponent<EnvironmentMovementBase>().DisableMovement();
            gun.PauseCoroutine();
        });
        PerformActionOnObjects(stagesInGame, stage => stage.GetComponent<EnvironmentMovementBase>().DisableMovement());
        PerformActionOnObjects(spaceshipsInGame, spaceship => spaceship.GetComponent<EnvironmentMovementBase>().DisableMovement());
        PerformActionOnObjects(enemiesInGame, enemy => enemy.GetComponent<EnvironmentMovementBase>().DisableMovement());


        player.DisableMovement();
    }


    public void IncreaseMovementSpeed()
    {
        speed += speedIncrease;
        SetMovementSpeed();
    }

}
