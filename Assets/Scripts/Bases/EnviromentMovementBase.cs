using UnityEngine;

public class EnvironmentMovementBase : MonoBehaviour
{
    public bool MovementEnabled { get; private set; }
    public float MovementSpeed { get; private set; }
    [SerializeField] private bool isStage;
    private GunController gun;
    private JetController jet;
    private CollectableBase spaceship;
    private EnemyController enemy;

    private void Awake()
    {
        gun = GetComponent<GunController>();
        jet = GetComponent<JetController>();
        spaceship = GetComponent<CollectableBase>();
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
            //svaki od ovih eventa bi mogao da ide u svoju skriptu a ovde samo da se pita da li su svi gun jet i spaceship i enemy null i ako jesu pozovese defaultni enviroment action
            // if (gun != null)
            // {
            //     EventManager.Instance.OnGunDestroyed(gun);
            // }
            // else if (jet != null)
            // {
            //     EventManager.Instance.OnJetDestroyed(jet);
            // }
            // else if (spaceship != null)
            // {
            //     EventManager.Instance.OnSpaceshipDestroyed(spaceship);
            // }
            // else if (enemy != null)
            // {
            //     EventManager.Instance.OnEnemyDestroyed(enemy);
            // }

            if (gun == null && jet == null && spaceship == null && enemy == null)
            {
                EventManager.Instance.OnEnviromentDestroyed(this);
            }
        }
    }
}
