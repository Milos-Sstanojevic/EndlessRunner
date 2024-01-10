using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    public event Action<ICollectible> OnCollectAction;
    public event Action<IDestroyable> OnDestroyAction;
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

    public void OnCollectibleCollected(ICollectible collectible)
    {
        OnCollectAction?.Invoke(collectible);
    }

    public void OnObjectDestroyed(IDestroyable destroyable)
    {
        OnDestroyAction?.Invoke(destroyable);
    }

    public void OnPlayerDead()
    {
        OnPlayerDeadAction?.Invoke();
    }
}
