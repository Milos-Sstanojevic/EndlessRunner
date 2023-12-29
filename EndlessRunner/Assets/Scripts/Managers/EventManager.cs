using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    public event Action<ICollectible> OnCollectAction;
    public event Action<IDestroyable> OnDestroyAction;
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

    public void Collect(ICollectible collectible)
    {
        if (OnCollectAction != null)
        {
            OnCollectAction?.Invoke(collectible);
        }
    }

    // public void Destroy(IDestroyable destroyable)
    // {
    //     if (OnDestroyAction != null)
    //     {
    //         OnDestroyAction?.Invoke(destroyable);
    //     }
    // }
}
