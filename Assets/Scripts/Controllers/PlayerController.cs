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
    private const float edgePositionYWithJet = 4.65f;
    private const float edgePositionZWithJet = -5f;
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
    [SerializeField] private float gravityModifier;
    [SerializeField] private GameObject gunPosition;
    [SerializeField] private GameObject jetPosition;
    private bool canJump = true;
    private bool isInAir = false;
    private bool movementEnabled;
    private float movementSpeed;

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
    }

    private void Update()
    {
        if (movementEnabled)
        {
            ContinueGunAndJetCoroutine();
            Shoot();
            characterAnimator.PlayRunAnimation();
            MovePlayer();
            Jump();
            KeepPlayerOnRoad();
            PlayAnimationsWithGunOrFlyingAnimations();
        }
        else
        {
            StopGunAndJetCoroutines();
            characterAnimator.StopRunAnimation();
            characterAnimator.StopRunningWithGunAnimation();
        }

        if (jetOnBack?.HasJet == false && isInAir)
        {
            StartCoroutine(SlowlyStartOrStopFlying(Vector3.zero, new Vector3(transform.position.x, zeroPosition + offsetFromGround, transform.position.z)));
            jetOnBack = null;
        }

        KeepPlayerInCameraFieldIfHasJet();
    }

    private void ContinueGunAndJetCoroutine()
    {
        if (gunInHands != null)
        {
            gunInHands.UnpauseCoroutine();
        }
        if (jetOnBack != null)
        {
            jetOnBack.UnpauseCoroutine();
        }
    }

    private void StopGunAndJetCoroutines()
    {
        if (gunInHands != null)
        {
            gunInHands.PauseCoroutine();
        }
        if (jetOnBack != null)
        {
            jetOnBack.PauseCoroutine();
        }
    }

    private void Shoot()
    {
        if (Input.GetKey(KeyCode.Mouse0) && gunInHands?.HasGun == true)
            gunInHands.ShootFromGun();
    }

    private void MovePlayer()
    {
        float horizontalInput = Input.GetAxisRaw(horizontalAxis);
        float verticalInput = Input.GetAxisRaw(verticalAxis);
        transform.Translate(Vector3.right * movementSpeed * Time.deltaTime * horizontalInput, Space.World);
        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime * verticalInput, Space.World);

        SpinWhileFlying(horizontalInput);
    }

    private void SpinWhileFlying(float horizontalInput)
    {
        if (jetOnBack?.HasJet == true)
        {
            transform.Rotate(0, 0, horizontalInput * -5, Space.World);
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump && !isInAir)
        {
            playerParticleSystem.PlayJumpingParticleEffect();
            characterAnimator.PlayJumpAnimation();
            AudioManager.Instance.PlayJumpSound();
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

    private void KeepPlayerInCameraFieldIfHasJet()
    {
        if (jetOnBack?.HasJet == true)
        {
            if (transform.position.z > edgePositionZWithJet)
                transform.position = new Vector3(transform.position.x, transform.position.y, edgePositionZWithJet);
            if (transform.position.y <= edgePositionYWithJet)
                transform.position = new Vector3(transform.position.x, edgePositionYWithJet, transform.position.z);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (/*other.gameObject.CompareTag(obstacleTag) ||*/  other.gameObject.CompareTag(enemyTag))
        {
            EventManager.Instance.OnPlayerDead();

            if (!IsPlayerOnGround())
                transform.position = new Vector3(transform.position.x, zeroPosition + 0.5f, transform.position.z);

            characterAnimator.PlayDeadAnimation();
            AudioManager.Instance.PlayDeathSound();

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
            gunInHands = null;
        }
        if (jetOnBack != null)
        {
            jetOnBack.ReleaseJetToPool();
            jetOnBack = null;
        }
    }

    private bool IsPlayerOnGround() => (int)transform.position.y == zeroPosition;

    private void OnTriggerEnter(Collider other)
    {
        CollectableController collectable = other.GetComponent<CollectableController>();

        if (collectable != null)
        {
            int pointsWorth = Collect(collectable);

            AudioManager.Instance.PlaySpaceshipCollectedSound();
            EventManager.Instance.OnCollectibleCollected(collectable, pointsWorth);
        }
    }

    private int Collect(CollectableController collectable)
    {
        JetController jet = collectable.GetComponent<JetController>();
        GunController gun = collectable.GetComponent<GunController>();

        if (jet != null)
        {
            if (gunInHands != null && gunInHands.HasGun)
            {
                gunInHands.ReleaseGunInPool();
                gunInHands = null;
            }

            return CollectJet(jet);
        }
        else if (gun != null)
        {
            return CollectGun(gun);
        }
        else
        {
            EventManager.Instance.OnSpaceshipDestroyed(collectable);
            return collectablePointsWorth;
        }
    }

    private int CollectGun(GunController collectable)
    {
        GunController gun = collectable;
        if ((gunInHands == null || gunInHands.HasGun == false) && jetOnBack == null)
        {
            gunInHands = gun;
            gun.MoveToPlayerHand(this, gunPosition.transform.position);
        }
        else
        {
            gun.ReleaseGunInPool();
        }
        return collectablePointsWorth * 2;
    }

    private int CollectJet(JetController collectable)
    {
        JetController jet = collectable;

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
