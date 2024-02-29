using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class OneScreenController : NetworkBehaviour
{
    [SerializeField] private MovementManager movementManager;
    [SerializeField] private SpawnManager spawnManager;
    private PlayerController playerController;

    private void Start()
    {
        NetworkSpawner.Instance.RPC_SpawnedScreen(this);
    }

    private void OnDestroy()
    {
        NetworkSpawner.Instance.RPC_DespawnedScreen(this);
    }

    public void SetPlayerControllerOnScreen(PlayerController player)
    {
        playerController = player;
    }

    public MovementManager GetMovementManagerInOneScreen() => movementManager;
    public PlayerController GetPlayerControllerInOneScreen() => playerController;
    public List<EnvironmentMovementController> GetEnvironmentMovementControllersInOneScreen() => gameObject.GetComponentsInChildren<EnvironmentMovementController>().ToList();
    public SpawnManager GetSpawnManagerOfOneScreen() => spawnManager;
}
