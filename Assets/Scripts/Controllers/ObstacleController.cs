using UnityEngine;
public class ObstacleController : MovementManager, IDestroyable
{
    protected override void Update()
    {
        base.Update();
        Destroy();
    }


    public void Destroy()
    {
        if (transform.position.z < GlobalConstants.PositionBehindPlayerAxisZ)
            EventManager.Instance.OnObjectDestroyed(this);
    }
}
