using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const string groundTag = "Ground";
    private const string obstacleTag = "Obstacle";
    private const string horizontalAxis = "Horizontal";
    private const string verticalAxis = "Vertical";
    private const float playerEdgePositionBackZ = -10.5f;
    private const float playerEdgePositionFrontZ = 0;
    private const int collectablePointsWorth = 20;
    private const float zeroPosition = 0;
    private GunController gunInHands;
    private Rigidbody playerRb;
    private AnimationManager characterAnimator;
    private ParticleSystemManager playerParticleSystem;
    [SerializeField] private float jumpForce;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip spaceshipCollectedSound;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private float gravityModifier;
    [SerializeField] private GameObject gunPosition;
    private bool canJump = true;
    private bool movementEnabled;
    private float movementSpeed;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        playerParticleSystem = GetComponent<ParticleSystemManager>();
        characterAnimator = GetComponent<AnimationManager>();
    }

    private void Start()
    {
        Physics.gravity *= gravityModifier;
    }

    private void Update()
    {
        if (movementEnabled)
        {
            PlayAnimationsWithGun();
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
    private void PlayAnimationsWithGun()
    {
        if (gunInHands != null && gunInHands.HasGun == true)
        {
            characterAnimator.StartRunningWithGunAnimation();
        }
        else
        {
            characterAnimator.StopRunningWithGunAnimation();
            gunInHands = null;
        }
    }

    private void MoveLeftOrRight()
    {
        float horizontalInput = Input.GetAxisRaw(horizontalAxis);
        float verticalInput = Input.GetAxisRaw(verticalAxis);
        transform.Translate(Vector3.right * movementSpeed * Time.deltaTime * horizontalInput);
        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime * verticalInput);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            playerParticleSystem.PlayJumpingParticleEffect();
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
        if (transform.position.z < playerEdgePositionBackZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, playerEdgePositionBackZ);
        }
        if (transform.position.z > playerEdgePositionFrontZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, playerEdgePositionFrontZ);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(obstacleTag))
        {
            EventManager.Instance.OnPlayerDead();

            if (!IsPlayerOnGround())
                transform.position = new Vector3(transform.position.x, zeroPosition, transform.position.z);

            characterAnimator.PlayDeadAnimation();
            audioManager.PlayDeathSound(deathSound);

            ReleaseGunIfITsInHands();
        }

        if (other.gameObject.CompareTag(groundTag))
        {
            canJump = true;
            playerParticleSystem.PlayLandingParticleEffect();
            characterAnimator.StopJumpAnimation();
        }
    }
    
    private void ReleaseGunIfITsInHands()
    {
        if (gunInHands != null)
        {
            gunInHands.ReleaseGunInPool();
        }
    }

    private bool IsPlayerOnGround() => (int)transform.position.y == zeroPosition;

    private void OnTriggerEnter(Collider other)
    {
        CollectableBase collectable = other.GetComponent<CollectableBase>();
        if (collectable != null)
        {
            int pointsWorth = collectablePointsWorth;
            if (collectable is GunController gun)
            {
                gunInHands = gun;
                pointsWorth = CollectGun(gun);
            }
            else
            {
                EventManager.Instance.OnObjectDestroyed(collectable);
            }
            audioManager.PlaySpaceshipCollectedSound(spaceshipCollectedSound);
            EventManager.Instance.OnCollectibleCollected(collectable, pointsWorth);

        }
    }

    private int CollectGun(GunController gun)
    {
        gun.MoveToPlayerHand(this, gunPosition.transform.position);
        return collectablePointsWorth * 2;
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
