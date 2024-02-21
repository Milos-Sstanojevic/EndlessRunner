using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OneScreenController : MonoBehaviour
{
    [SerializeField] private MovementManager movementManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private SpawnManager spawnManager;

    public MovementManager GetMovementManagerInOneScreen() => movementManager;
    public PlayerController GetPlayerControllerInOneScreen() => playerController;
    public List<EnvironmentMovementController> GetEnvironmentMovementControllersInOneScreen() => gameObject.GetComponentsInChildren<EnvironmentMovementController>().ToList();
    public SpawnManager GetSpawnManagerOfOneScreen() => spawnManager;
}
