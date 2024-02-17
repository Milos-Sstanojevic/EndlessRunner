using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private const string HorizontalAxis = "Horizontal";
    private const string VerticalAxis = "Vertical";
    private bool canJump = true;
    private PlayerMovement playerMovement;
    [SerializeField] private float jumpForce;
    private PlayerGunHandler gunHandler;
    private PlayerAnimationHandler playerAnimationHandler;
    private ParticleSystemManager playerParticleSystem;
    private PlayerJetHandler jetHandler;
    private Rigidbody playerRb;
    private Vector2 movementInput = Vector2.zero;
    private bool jumped;

    private void Awake()
    {
        playerParticleSystem = GetComponent<ParticleSystemManager>();
        playerAnimationHandler = GetComponent<PlayerAnimationHandler>();
        playerRb = GetComponent<Rigidbody>();
        jetHandler = GetComponent<PlayerJetHandler>();
        gunHandler = GetComponent<PlayerGunHandler>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jumped = context.action.triggered;
    }

    private void Update()
    {
        if (playerMovement.IsMovementEnabled())
        {
            HandleMovementInput();
            HandleJumpInput();
            HandleShootInput();
        }
    }

    private void HandleMovementInput()
    {
        playerMovement.MovePlayer(movementInput.x, movementInput.y);
    }

    private void HandleJumpInput()
    {
        if (jumped && canJump && !jetHandler.IsInAir())
            HandlePlayerJumping();
    }

    private void HandlePlayerJumping()
    {
        playerParticleSystem.PlayJumpingParticleEffect();
        playerAnimationHandler.PlayJumpAnimation();
        AudioManager.Instance.PlayJumpSound();
        canJump = false;
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void HandleShootInput()
    {
        if (Input.GetKey(KeyCode.Mouse0) && gunHandler.HasGun())
        {
            gunHandler.GetGunInHands().ShootFromGun();
        }
    }

    public void SetCanJumpToTrue()
    {
        canJump = true;
    }
}
