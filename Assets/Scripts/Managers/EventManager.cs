using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    public event Action<CollectableBase, int> OnCollectAction;
    public event Action<ObjectMovementBase> OnDestroyAction;
    public event Action OnPlayerDeadAction;

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

    public void OnObjectDestroyed(ObjectMovementBase destroyable)
    {
        OnDestroyAction?.Invoke(destroyable);
    }

    public void OnPlayerDead()
    {
        OnPlayerDeadAction?.Invoke();
    }
}
