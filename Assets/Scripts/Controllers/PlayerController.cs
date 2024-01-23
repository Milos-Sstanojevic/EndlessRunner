using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const string groundTag = "Ground";
    private const string enemyTag = "Enemy";
    private const string obstacleTag = "Obstacle";
    private const string horizontalAxis = "Horizontal";
    private const string verticalAxis = "Vertical";
    private const float playerEdgePositionBackZ = -10.5f;
    private const float offsetFromGround = 0.65f;
    private const float flyingPosition = 4.65f;
    private const float playerEdgePositionFrontZ = -2;
    private const int collectablePointsWorth = 5;
    private const float asscendingSpeed = 5;
    private const float zeroPosition = 0;
    private static Vector3 rotateAroundX = new Vector3(90, 0, 0);
    private GunController gunInHands;
    private JetController jetOnBack;
    private Rigidbody playerRb;
    private Collider playerCollider;
    private AnimationManager characterAnimator;
    private ParticleSystemManager playerParticleSystem;
    [SerializeField] private float jumpForce;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip spaceshipCollectedSound;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private float gravityModifier;
    [SerializeField] private GameObject gunPosition;
    [SerializeField] private GameObject jetPosition;
    private bool canJump = true;
    private bool isInAir = false;
    private bool movementEnabled;
    private float movementSpeed;
    private Dictionary<Type, Func<CollectableBase, int>> collectableHandlers;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        playerParticleSystem = GetComponent<ParticleSystemManager>();
        characterAnimator = GetComponent<AnimationManager>();
    }

    private void Start()
    {
        Physics.gravity *= gravityModifier;
        collectableHandlers = new Dictionary<Type, Func<CollectableBase, int>>()
        {
            {typeof(GunController),CollectGun},
            {typeof(JetController),CollectJet}
        };
    }

    private void Update()
    {
        if (movementEnabled)
        {
            Shoot();
            characterAnimator.PlayRunAnimation();
            MoveLeftOrRight();
            Jump();
            KeepPlayerOnRoad();
            PlayAnimationsWithGunOrFlyingAnimations();
        }
        else
        {
            characterAnimator.StopRunAnimation();
            characterAnimator.StopRunningWithGunAnimation();
        }

        if (jetOnBack?.HasJet == false && isInAir)
        {
            StartCoroutine(SlowlyStartOrStopFlying(Vector3.zero, new Vector3(transform.position.x, zeroPosition + offsetFromGround, transform.position.z)));
            jetOnBack = null;
        }

        if (jetOnBack?.HasJet == true)
        {
            KeepPlayerInCameraField();
        }
    }

    private void PlayAnimationsWithGunOrFlyingAnimations()
    {
        if (gunInHands?.HasGun == true)
        {
            characterAnimator.StartRunningWithGunAnimation();
        }
        else
        {
            characterAnimator.StopRunningWithGunAnimation();
        }

        if (jetOnBack?.HasJet == true)
        {
            characterAnimator.StartFlyingAnimation();
        }
        else
        {
            characterAnimator.StopFlyingAnimation();
        }
    }

    private void Shoot()
    {
        if (Input.GetKey(KeyCode.Mouse0) && gunInHands?.HasGun == true)
            gunInHands.ShootFromGun();
    }

    private void KeepPlayerInCameraField()
    {
        if (transform.position.z > -5f)
            transform.position = new Vector3(transform.position.x, transform.position.y, -5f);
        if (transform.position.y <= 4.65f)
            transform.position = new Vector3(transform.position.x, 4.65f, transform.position.z);
    }

    private void MoveLeftOrRight()
    {
        float horizontalInput = Input.GetAxisRaw(horizontalAxis);
        float verticalInput = Input.GetAxisRaw(verticalAxis);
        transform.Translate(Vector3.right * movementSpeed * Time.deltaTime * horizontalInput, Space.World);
        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime * verticalInput, Space.World);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump && !isInAir)
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
        if (other.gameObject.CompareTag(obstacleTag) /*|| other.gameObject.CompareTag(enemyTag)*/)
        {
            EventManager.Instance.OnPlayerDead();

            if (!IsPlayerOnGround())
                transform.position = new Vector3(transform.position.x, zeroPosition, transform.position.z);

            characterAnimator.PlayDeadAnimation();
            audioManager.PlayDeathSound(deathSound);

            ReleaseGunAndJetIfHaveOne();
        }

        if (other.gameObject.CompareTag(groundTag))
        {
            canJump = true;
            playerParticleSystem.PlayLandingParticleEffect();
            characterAnimator.StopJumpAnimation();
        }
    }

    private void ReleaseGunAndJetIfHaveOne()
    {
        if (gunInHands != null)
        {
            gunInHands.ReleaseGunInPool();
        }
        if (jetOnBack != null)
        {
            jetOnBack.ReleaseJetToPool();
        }
    }

    private bool IsPlayerOnGround() => (int)transform.position.y == zeroPosition;

    private void OnTriggerEnter(Collider other)
    {
        CollectableBase collectable = other.GetComponent<CollectableBase>();

        if (collectable != null)
        {
            int pointsWorth = collectablePointsWorth;
            if (collectableHandlers.TryGetValue(collectable.GetType(), out var collectFunction))
            {
                pointsWorth = collectFunction(collectable);
            }
            else
            {
                EventManager.Instance.OnEnviromentDestroyed(collectable);
            }

            audioManager.PlaySpaceshipCollectedSound(spaceshipCollectedSound);
            EventManager.Instance.OnCollectibleCollected(collectable, pointsWorth);
        }
    }

    private int CollectJet(CollectableBase collectable)
    {
        JetController jet = (JetController)collectable;

        if (jetOnBack == null && !isInAir)
        {
            jetOnBack = jet;
            StartCoroutine(SlowlyStartOrStopFlying(rotateAroundX, new Vector3(transform.position.x, zeroPosition + flyingPosition, transform.position.z)));
            jet.MoveOnPlayerBack(this, jetPosition.transform.position);
        }
        else
        {
            jet.ReleaseJetToPool();
        }

        return collectablePointsWorth * 2;
    }

    public IEnumerator SlowlyStartOrStopFlying(Vector3 endRotation, Vector3 endPosition)
    {
        isInAir = !isInAir;
        playerCollider.enabled = false;

        Vector3 startPos = transform.position;
        Vector3 startRotation = transform.rotation.eulerAngles;
        float distance = Vector3.Distance(startPos, endPosition);
        float remainingDistance = distance;


        while (remainingDistance > 0)
        {
            transform.position = Vector3.Lerp(startPos, endPosition, 1 - (remainingDistance / distance));
            transform.eulerAngles = Vector3.Lerp(startRotation, endRotation, 1 - (remainingDistance / distance));
            remainingDistance -= asscendingSpeed * Time.deltaTime;

            yield return null;
        }

        playerCollider.enabled = true;
        transform.position = endPosition;
        transform.eulerAngles = endRotation;
    }

    private int CollectGun(CollectableBase collectable)
    {
        GunController gun = (GunController)collectable;
        gunInHands = gun;
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
