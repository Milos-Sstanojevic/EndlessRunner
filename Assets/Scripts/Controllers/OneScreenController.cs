using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class OneScreenController : NetworkBehaviour
{
    [SerializeField] private MovementManager movementManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private Camera playerCamera;

    public MovementManager GetMovementManagerInOneScreen() => movementManager;
    public PlayerController GetPlayerControllerInOneScreen() => playerController;
    public List<EnvironmentMovementController> GetEnvironmentMovementControllersInOneScreen() => gameObject.GetComponentsInChildren<EnvironmentMovementController>().ToList();
    public SpawnManager GetSpawnManagerOfOneScreen() => spawnManager;
}
