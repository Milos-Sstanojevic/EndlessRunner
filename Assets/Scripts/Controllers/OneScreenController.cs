using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class OneScreenController : NetworkBehaviour
{
    [SerializeField] private MovementManager movementManager;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private PlayerController playerController;
    private bool isGameOnline;

    private void OnEnable()
    {
        EventManager.Instance.SubscribeToOnSetGameToOnlineAction(SetGameToOnline);
        EventManager.Instance.SubscribeToOnSetGameToOfflineAction(SetGameToOffline);
    }

    private void Start()
    {
        if (isGameOnline)
            NetworkSpawner.Instance.RPC_SpawnedScreen(this);
    }

    public override void Spawned()
    {
        SetGameToOnline();
    }

    public void SetGameToOnline()
    {
        isGameOnline = true;
    }

    public void SetGameToOffline()
    {
        isGameOnline = false;
    }

    public void SetPlayerControllerOnScreen(PlayerController player)
    {
        playerController = player;
    }

    public MovementManager GetMovementManagerInOneScreen() => movementManager;
    public PlayerController GetPlayerControllerInOneScreen() => playerController;
    public List<EnvironmentMovementController> GetEnvironmentMovementControllersInOneScreen() => gameObject.GetComponentsInChildren<EnvironmentMovementController>().ToList();
    public SpawnManager GetSpawnManagerOfOneScreen() => spawnManager;

    private void OnDisable()
    {
        EventManager.Instance.UnsubscribeToOnSetGameToOnlineAction(SetGameToOnline);
        EventManager.Instance.UnsubscribeToOnSetGameToOfflineAction(SetGameToOffline);
    }

    private void OnDestroy()
    {
        if (isGameOnline)
            NetworkSpawner.Instance.RPC_DespawnedScreen(this);
    }
}
