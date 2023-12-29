using System;
using UnityEngine;
using static GlobalConstants;

public class PlayerController : MonoBehaviour
{
    public event Action OnPlayerDead;

    private const float edgePosX = 4;
    private Rigidbody playerRb;
    private Animator Animator;
    [SerializeField] private float jumpForce;
    [SerializeField] private AudioManager audioManager;
    private bool canJump = true;
    [SerializeField] private float GravityModifier;
    private bool movementEnabled = false;
    private float movementSpeed;


    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();
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
            audioManager.PlayJumpSound();
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            canJump = false;
        }
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
            OnPlayerDead?.Invoke();

            if (IsPlayerOnGround())
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);

            Animator.SetBool(DeadAnimation, true);
            audioManager.PlayDeathSound();
        }

        if (other.gameObject.CompareTag(GroundTag))
        {
            canJump = true;
            Animator.SetBool(JumpAnimation, false);
        }
    }

    private bool IsPlayerOnGround() => transform.position.y != 0;

    private void OnTriggerEnter(Collider other)
    {
        ICollectible collectible = other.GetComponent<ICollectible>();
        if (collectible != null)
        {
            audioManager.PlaySpaceshipCollectedSound();
            EventManager.Instance.Collect(collectible);
        }
    }

    private void OnDestroy()
    {
        Physics.gravity /= GravityModifier;
    }

    public void SetMovementEnabled() => movementEnabled = true;
    public void SetMovementDisabled() => movementEnabled = false;
    public void SetSpeedOfCharacter(float speed) => movementSpeed = speed;
}
