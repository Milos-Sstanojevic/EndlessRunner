using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    private const float ZeroPosition = 0;
    private const string GroundTag = "Ground";
    private const string EnemyTag = "Enemy";
    private AnimationManager characterAnimator;
    private PlayerController playerController;
    private ParticleSystemManager playerParticleSystem;
    private PlayerGunHandler gunHandler;
    private PlayerJetHandler jetHandler;
    private PlayerCollectingHandler playerCollectingHandler;
    private PlayerInputController playerInputController;

    private void Start()
    {
        playerInputController = GetComponent<PlayerInputController>();
        playerCollectingHandler = GetComponent<PlayerCollectingHandler>();
        jetHandler = GetComponent<PlayerJetHandler>();
        gunHandler = GetComponent<PlayerGunHandler>();
        characterAnimator = GetComponent<AnimationManager>();
        playerController = GetComponent<PlayerController>();
        playerParticleSystem = GetComponent<ParticleSystemManager>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (/*other.gameObject.CompareTag(obstacleTag) ||*/other.gameObject.CompareTag(EnemyTag))
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
        EventManager.Instance.OnPlayerDead();

        if (IsPlayerOnGround())
        {
            playerController.transform.position = new Vector3(playerController.transform.position.x, ZeroPosition + 0.5f, playerController.transform.position.z);
        }

        characterAnimator.PlayDeadAnimation();
        AudioManager.Instance.PlayDeathSound();

        gunHandler.ReleaseGunIfHaveOne();
        jetHandler.ReleaseJetIfHaveOne();
    }

    private void HandleGroundCollision()
    {
        playerInputController.SetCanJumpToTrue();
        playerParticleSystem.PlayLandingParticleEffect();
        characterAnimator.StopJumpAnimation();
    }

    private void OnTriggerEnter(Collider other)
    {
        CollectableController collectable = other.GetComponent<CollectableController>();

        if (collectable != null)
        {
            HandleCollectableCollision(collectable);
        }
    }

    private void HandleCollectableCollision(CollectableController collectable)
    {
        int points = playerCollectingHandler.Collect(collectable);
        playerController.ScoreManager.AddPoints(points);
    }

    public bool IsPlayerOnGround() => (int)transform.position.y == ZeroPosition;
}