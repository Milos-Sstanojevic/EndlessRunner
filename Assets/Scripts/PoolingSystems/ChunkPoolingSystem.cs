using System.Collections.Generic;
using UnityEngine;

public class ChunkPoolingSystem : BasePoolingSystem<ChunkController>
{
    private void OnEnable()
    {
        EventManager.Instance.SubscribeToOnChunkDestroyAction(DestroyObjects);
    }

    private void OnDisable()
    {
        EventManager.Instance.UnsubscribeFromOnChunkDestroyAction(DestroyObjects);
    }
}
