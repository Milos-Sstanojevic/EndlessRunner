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

    private void Start()
    {
        NetworkSpawner.Instance.RPC_SpawnedObject(this);
    }

    public void SetCameraRect(Rect rect)
    {
        playerCamera.rect = rect;
    }

    private void OnDestroy()
    {
        NetworkSpawner.Instance.RPC_DespawnedObject(this);
    }

    public MovementManager GetMovementManagerInOneScreen() => movementManager;
    public PlayerController GetPlayerControllerInOneScreen() => playerController;
    public List<EnvironmentMovementController> GetEnvironmentMovementControllersInOneScreen() => gameObject.GetComponentsInChildren<EnvironmentMovementController>().ToList();
    public SpawnManager GetSpawnManagerOfOneScreen() => spawnManager;
}
