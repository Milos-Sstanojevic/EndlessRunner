using UnityEngine;

public class EnvironmentMovementController : MonoBehaviour
{
    public bool MovementEnabled { get; private set; }
    public float MovementSpeed { get; private set; }
    [SerializeField] private bool isStage;
    private GunController gun;
    private JetController jet;
    private CollectableController spaceship;
    private EnemyController enemy;

    private void Awake()
    {
        gun = GetComponent<GunController>();
        jet = GetComponent<JetController>();
        spaceship = GetComponent<CollectableController>();
        enemy = GetComponent<EnemyController>();
    }

    private void Update()
    {
        MoveObject();
        if (!isStage)
            Destroy();
    }

    public void MoveObject()
    {

        if (MovementEnabled)
        {
            transform.Translate(Vector3.back * MovementSpeed * Time.deltaTime, Space.World);
        }
    }

    public void EnableMovement()
    {
        MovementEnabled = true;
    }

    public void DisableMovement()
    {
        MovementEnabled = false;
    }

    public void SetMovementSpeed(float speed)
    {
        MovementSpeed = speed;
    }

    private void Destroy()
    {
        if (transform.position.z < GlobalConstants.PositionBehindPlayerAxisZ)
        {
            if (gun == null && jet == null && spaceship == null && enemy == null)
            {
                EventManager.Instance.OnEnviromentDestroyed(this);
            }
        }
    }
}
