using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PlayerController : MonoBehaviour
{
    private const string GroundTag = "Ground";
    private const string EnemyTag = "Enemy";
    private const string ObstacleTag = "Obstacle";
    private const string HorizontalAxis = "Horizontal";
    private const string VerticalAxis = "Vertical";
    private const int OneScorePoint = 1;
    private const float AddPointsDelay = 0.5f;
    private const float PlayerEdgePositionBackZ = -10.5f;
    private const float EdgePositionYWithJet = 4.65f;
    private const float EdgePositionZWithJet = -5f;
    private const float OffsetFromGround = 0.65f;
    private const float FlyingPosition = 4.65f;
    private const float PlayerEdgePositionFrontZ = -2;
    private const int CollectablePointsWorth = 5;
    private const float AsscendingSpeed = 5;
    private const float ZeroPosition = 0;
    private static Vector3 RotateAroundX = new Vector3(90, 0, 0);
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
    private int score;
    private bool canAddPoints;

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
        Time.timeScale = 1f;
        score = 0;
    }

    private void OnEnable()
    {
        EventManager.Instance.SubscribeToStartAddingPointsAction(StartAddingPoints);
        EventManager.Instance.SubscribeToStopAddingPointsAction(StopAddingPoints);
    }

    private void StartAddingPoints()
    {
        canAddPoints = true;
        StartCoroutine(AddPointsEachHalfSecond());
    }

    private IEnumerator AddPointsEachHalfSecond()
    {
        while (canAddPoints)
        {
            score += OneScorePoint;
            yield return new WaitForSeconds(AddPointsDelay);
            EventManager.Instance.OnChangeScoreOnScreen(score);
        }
    }

    private void StopAddingPoints()
    {
        canAddPoints = false;
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
            StartCoroutine(SlowlyStartOrStopFlying(Vector3.zero, new Vector3(transform.position.x, ZeroPosition + OffsetFromGround, transform.position.z)));
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
        float horizontalInput = Input.GetAxisRaw(HorizontalAxis);
        float verticalInput = Input.GetAxisRaw(VerticalAxis);
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
        if (transform.position.x < -MapEdgeConstants.EdgePosX)
        {
            transform.position = new Vector3(-MapEdgeConstants.EdgePosX, transform.position.y, transform.position.z);
        }
        if (transform.position.x > MapEdgeConstants.EdgePosX)
        {
            transform.position = new Vector3(MapEdgeConstants.EdgePosX, transform.position.y, transform.position.z);
        }
        if (transform.position.z < PlayerEdgePositionBackZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, PlayerEdgePositionBackZ);
        }
        if (transform.position.z > PlayerEdgePositionFrontZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, PlayerEdgePositionFrontZ);
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
            if (transform.position.z > EdgePositionZWithJet)
                transform.position = new Vector3(transform.position.x, transform.position.y, EdgePositionZWithJet);
            if (transform.position.y <= EdgePositionYWithJet)
                transform.position = new Vector3(transform.position.x, EdgePositionYWithJet, transform.position.z);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (/*other.gameObject.CompareTag(obstacleTag) ||*/  other.gameObject.CompareTag(EnemyTag))
        {
            EventManager.Instance.OnPlayerDead();

            if (!IsPlayerOnGround())
                transform.position = new Vector3(transform.position.x, ZeroPosition + 0.5f, transform.position.z);

            characterAnimator.PlayDeadAnimation();
            AudioManager.Instance.PlayDeathSound();

            ReleaseGunAndJetIfHaveOne();
        }

        if (other.gameObject.CompareTag(GroundTag))
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
            StartCoroutine(ReleaseGunCoroutine());
        }
        if (jetOnBack != null)
        {
            StartCoroutine(ReleaseJetCoroutine());
        }
    }

    private IEnumerator ReleaseGunCoroutine()
    {
        gunInHands.ReleaseGunInPool();
        yield return null;
        gunInHands = null;
    }

    private IEnumerator ReleaseJetCoroutine()
    {
        jetOnBack.ReleaseJetToPool();
        yield return null;
        jetOnBack = null;
    }

    private bool IsPlayerOnGround() => (int)transform.position.y == ZeroPosition;

    private void OnTriggerEnter(Collider other)
    {
        CollectableController collectable = other.GetComponent<CollectableController>();

        if (collectable != null)
        {
            score += Collect(collectable);
            EventManager.Instance.OnChangeScoreOnScreen(score);
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
            AudioManager.Instance.PlaySpaceshipCollectedSound();
            EventManager.Instance.OnSpaceshipDestroyed(collectable);
            return CollectablePointsWorth;
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

        AudioManager.Instance.PlayGunCollectedSound();

        return CollectablePointsWorth * 2;
    }

    private int CollectJet(JetController collectable)
    {
        JetController jet = collectable;

        if (jetOnBack == null && !isInAir)
        {
            jetOnBack = jet;
            StartCoroutine(SlowlyStartOrStopFlying(RotateAroundX, new Vector3(transform.position.x, ZeroPosition + FlyingPosition, transform.position.z)));
            jet.MoveOnPlayerBack(this, jetPosition.transform.position);
        }
        else
        {
            jet.ReleaseJetToPool();
        }

        AudioManager.Instance.PlayJetCollectedSound();

        return CollectablePointsWorth * 2;
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
            remainingDistance -= AsscendingSpeed * Time.deltaTime;

            yield return null;
        }

        playerCollider.enabled = true;
        transform.position = endPosition;
        transform.eulerAngles = endRotation;
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

    private void OnDestroy()
    {
        Physics.gravity /= gravityModifier;
    }

    private void OnDisable()
    {
        EventManager.Instance.UnsubscribeFromStartAddingPointsAction(StartAddingPoints);
        EventManager.Instance.UnsubscribeFromStopAddingPointsAction(StopAddingPoints);
    }
}
