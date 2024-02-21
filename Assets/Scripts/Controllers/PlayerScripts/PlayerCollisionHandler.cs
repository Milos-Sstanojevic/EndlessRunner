using System.Collections;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    private const float ZeroPosition = 0;
    private const float OffsetForPrefab = 0.5f;
    private const string GroundTag = "Ground";
    private const string EnemyTag = "Enemy";
    private const string ObstacleTag = "Obstacle";
    [SerializeField] private GameObject gameOverScreen;
    private AnimationManager characterAnimator;
    private PlayerController playerController;
    private ParticleSystemManager playerParticleSystem;
    private PlayerGunHandler gunHandler;
    private PlayerJetHandler jetHandler;
    private PlayerCollectingHandler playerCollectingHandler;
    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerCollectingHandler = GetComponent<PlayerCollectingHandler>();
        jetHandler = GetComponent<PlayerJetHandler>();
        gunHandler = GetComponent<PlayerGunHandler>();
        characterAnimator = GetComponent<AnimationManager>();
        playerController = GetComponent<PlayerController>();
        playerParticleSystem = GetComponent<ParticleSystemManager>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (/*other.gameObject.CompareTag(ObstacleTag) ||*/ other.gameObject.CompareTag(EnemyTag))
        {
            HandleEnemyOrObstacleCollision();
        }

        if (other.gameObject.CompareTag(GroundTag))
        {
            HandleGroundCollision();
        }
    }

    private void HandleEnemyOrObstacleCollision()
    {
        EventManager.Instance.OnPlayerDead(playerController, gameOverScreen);
        playerController.PlayerDied();

        if (!IsPlayerOnGround())
        {
            playerController.transform.position = new Vector3(playerController.transform.position.x, ZeroPosition + OffsetForPrefab, playerController.transform.position.z);
            playerController.transform.eulerAngles = Vector3.zero;
        }

        characterAnimator.PlayDeadAnimation();
        AudioManager.Instance.PlayDeathSound();

        gunHandler.ReleaseGunIfHaveOne();
        jetHandler.ReleaseJetIfHaveOne();
    }

    private void HandleGroundCollision()
    {
        StartCoroutine(WaitToLoad());
    }

    private IEnumerator WaitToLoad()
    {
        yield return new WaitForEndOfFrame();
        playerMovement.SetCanJumpToTrue();
        playerParticleSystem.PlayLandingParticleEffect();
        characterAnimator.StopJumpAnimation();
    }

    private void OnTriggerEnter(Collider other)
    {
        CollectableController collectable = other.GetComponent<CollectableController>();

        if (collectable != null)
            HandleCollectableCollision(collectable);
    }

    private void HandleCollectableCollision(CollectableController collectable)
    {
        int points = playerCollectingHandler.Collect(collectable);
        playerController.ScoreManager.AddPoints(points);
    }

    public bool IsPlayerOnGround() => (int)transform.position.y == ZeroPosition;
}
