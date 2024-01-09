using UnityEngine;
using static GlobalConstants;

public class PlayerController : MonoBehaviour
{
    private const float edgePosX = 4;
    private Rigidbody playerRb;
    private Animator Animator;
    [SerializeField] private float jumpForce;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip deathSound;
    private AudioSource audioSource;
    private bool canJump = true;
    [SerializeField] private float GravityModifier;
    private bool movementEnabled = false;
    private float movementSpeed;

    public bool ShouldPlaySpaceshipCollectedSound { get; set; }


    public void SetMovementEnabled() => movementEnabled = true;
    public void SetMovementDisabled() => movementEnabled = false;
    public void SetSpeedOfCharacter(float speed) => movementSpeed = speed;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        Physics.gravity *= GravityModifier;
    }

    private void Update()
    {
        if (movementEnabled)
        {
            Animator.SetBool(RunAnimation, true);

            MoveLeftOrRight();
            Jump();
            KeepPlayerOnRoad();
        }
        else
        {
            Animator.SetBool(RunAnimation, false);
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
            Animator.SetBool(JumpAnimation, true);
            PlayJumpSound();
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            canJump = false;
        }
    }

    public void PlayJumpSound()
    {
        audioSource.PlayOneShot(jumpSound, ClipVolume);
    }

    private void KeepPlayerOnRoad()
    {
        if (transform.position.x < -edgePosX)
        {
            transform.position = new Vector3(-edgePosX, transform.position.y, transform.position.z);
        }
        if (transform.position.x > edgePosX)
        {
            transform.position = new Vector3(edgePosX, transform.position.y, transform.position.z);
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(ObstacleTag))
        {
            EventManager.Instance.OnPlayerDead();

            if (!IsPlayerOnGround())
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);

            Animator.SetBool(DeadAnimation, true);
            PlayDeathSound();
        }

        if (other.gameObject.CompareTag(GroundTag))
        {
            canJump = true;
            Animator.SetBool(JumpAnimation, false);
        }
    }

    public void PlayDeathSound()
    {
        audioSource.PlayOneShot(deathSound, ClipVolume);
    }

    private bool IsPlayerOnGround() => transform.position.y == 0;

    private void OnTriggerEnter(Collider other)
    {
        ICollectible collectible = other.GetComponent<ICollectible>();
        if (collectible != null)
        {
            ShouldPlaySpaceshipCollectedSound = true;
            EventManager.Instance.Collect(collectible);
        }
    }



    private void OnDestroy()
    {
        Physics.gravity /= GravityModifier;
    }
}
