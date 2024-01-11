using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const string GroundTag = "Ground";
    private const string ObstacleTag = "Obstacle";
    private const string HorizontalAxis = "Horizontal";
    private Rigidbody playerRb;
    [SerializeField] private AnimationManager characterAnimator;
    [SerializeField] private float jumpForce;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip spaceshipCollectedSound;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private float GravityModifier;
    private bool canJump = true;
    private bool movementEnabled;
    private float movementSpeed;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Physics.gravity *= GravityModifier;
    }

    private void Update()
    {
        if (movementEnabled)
        {
            characterAnimator.PlayRunAnimation();

            MoveLeftOrRight();
            Jump();
            KeepPlayerOnRoad();
        }
        else
        {
            characterAnimator.StopRunAnimation();
        }
    }

    private void MoveLeftOrRight()
    {
        float horizontalInput = Input.GetAxisRaw(HorizontalAxis);
        transform.Translate(Vector3.right * movementSpeed * Time.deltaTime * horizontalInput);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            characterAnimator.PlayJumpAnimation();
            audioManager.PlayJumpSound(jumpSound);
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            canJump = false;
        }
    }

    private void KeepPlayerOnRoad()
    {
        if (transform.position.x < -GlobalConstants.edgePosX)
        {
            transform.position = new Vector3(-GlobalConstants.edgePosX, transform.position.y, transform.position.z);
        }
        if (transform.position.x > GlobalConstants.edgePosX)
        {
            transform.position = new Vector3(GlobalConstants.edgePosX, transform.position.y, transform.position.z);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(ObstacleTag))
        {
            EventManager.Instance.OnPlayerDead();

            if (!IsPlayerOnGround())
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);

            characterAnimator.PlayDeadAnimation();
            audioManager.PlayDeathSound(deathSound);
        }

        if (other.gameObject.CompareTag(GroundTag))
        {
            canJump = true;
            characterAnimator.StopJumpAnimation();
        }
    }


    private bool IsPlayerOnGround() => transform.position.y == 0;

    private void OnTriggerEnter(Collider other)
    {
        ICollectible collectible = other.GetComponent<ICollectible>();
        if (collectible != null)
        {
            audioManager.PlaySpaceshipCollectedSound(spaceshipCollectedSound);
            EventManager.Instance.OnCollectibleCollected(collectible);
        }
    }


    private void OnDestroy()
    {
        Physics.gravity /= GravityModifier;
    }

    public void EnableMovement()
    {
        movementEnabled = true;
    }

    public void DisableMovement()
    {
        movementEnabled = false;
    }

    public void SetMovementSpeed(float speed)
    {
        movementSpeed = speed;
    }
}
