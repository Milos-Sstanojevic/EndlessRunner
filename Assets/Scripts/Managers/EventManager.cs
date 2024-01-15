using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    public event Action<CollectableBase, int> OnCollectAction;
    public event Action<EnviromentMovementBase> OnDestroyAction;
    public event Action OnPlayerDeadAction;
    public event Action OnGunCollectedAction;

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

    public void OnCollectibleCollected(CollectableBase collectible, int pointsWorth)
    {
        OnCollectAction?.Invoke(collectible, pointsWorth);
    }

    public void OnObjectDestroyed(EnviromentMovementBase destroyable)
    {
        OnDestroyAction?.Invoke(destroyable);
    }

    public void OnPlayerDead()
    {
        OnPlayerDeadAction?.Invoke();
    }

    public void OnGunCollected()
    {
        OnGunCollectedAction?.Invoke();
    }
}
