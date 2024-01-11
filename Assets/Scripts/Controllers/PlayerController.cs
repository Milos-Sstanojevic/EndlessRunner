using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const string groundTag = "Ground";
    private const string obstacleTag = "Obstacle";
    private const string horizontalAxis = "Horizontal";
    private Rigidbody playerRb;
    [SerializeField] private ParticleSystem jumpingParticles;
    [SerializeField] private ParticleSystem landingParticles;
    [SerializeField] private AnimationManager characterAnimator;
    [SerializeField] private float jumpForce;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip spaceshipCollectedSound;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private float gravityModifier;
    private bool canJump = true;
    private bool movementEnabled;
    private float movementSpeed;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Physics.gravity *= gravityModifier;
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
        float horizontalInput = Input.GetAxisRaw(horizontalAxis);
        transform.Translate(Vector3.right * movementSpeed * Time.deltaTime * horizontalInput);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            jumpingParticles.Play();
            characterAnimator.PlayJumpAnimation();
            audioManager.PlayJumpSound(jumpSound);
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            canJump = false;
        }
    }

    private void KeepPlayerOnRoad()
    {
        if (transform.position.x < -GlobalConstants.EdgePosX)
        {
            transform.position = new Vector3(-GlobalConstants.EdgePosX, transform.position.y, transform.position.z);
        }
        if (transform.position.x > GlobalConstants.EdgePosX)
        {
            transform.position = new Vector3(GlobalConstants.EdgePosX, transform.position.y, transform.position.z);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(obstacleTag))
        {
            EventManager.Instance.OnPlayerDead();

            if (!IsPlayerOnGround())
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);

            characterAnimator.PlayDeadAnimation();
            audioManager.PlayDeathSound(deathSound);
        }

        if (other.gameObject.CompareTag(groundTag))
        {
            if (!IsPlayerOnGround())
            {
                landingParticles.Play();    //ne valja ovo prepravi BUG
            }
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
        Physics.gravity /= gravityModifier;
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
