using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    private const float EdgePositionYWithJet = 4.65f;
    private const float EdgePositionZWithJet = -5f;
    private const float PlayerEdgePositionBackZ = -10.5f;
    private const float PlayerEdgePositionFrontZ = -2;

    [SerializeField] private float jumpForce;

    private PlayerAnimationHandler playerAnimationHandler;
    private ParticleSystemManager playerParticleSystem;
    private float playerStartPositionX;
    private Rigidbody playerRb;
    private float movementSpeed;
    private PlayerJetHandler jetHandler;
    private bool movementEnabled;
    private Vector2 movementInput = Vector2.zero;
    private bool jumped;
    private bool canJump = true;


    private void Awake()
    {
        playerParticleSystem = GetComponent<ParticleSystemManager>();
        playerAnimationHandler = GetComponent<PlayerAnimationHandler>();
        jetHandler = GetComponent<PlayerJetHandler>();
        playerRb = GetComponent<Rigidbody>();
        playerStartPositionX = transform.localPosition.x;
        gameObject.AddComponent<RunnerSimulatePhysics3D>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jumped = context.action.triggered;
    }

    public override void FixedUpdateNetwork()
    {

        KeepPlayerOnRoad();
        KeepPlayerInCameraFieldIfHasJet();

        if (!movementEnabled)
            return;

        HandleMovementInput();
        HandleJumpInput();
    }


    private void HandleMovementInput()
    {
        MovePlayer(movementInput.x, movementInput.y);
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

    public void SetCanJumpToTrue()
    {
        canJump = true;
    }

    private void KeepPlayerInCameraFieldIfHasJet()
    {
        if (jetHandler.GetJetOnBack()?.HasJet == true)
        {
            if (transform.localPosition.z > EdgePositionZWithJet)
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, EdgePositionZWithJet);
            if (transform.localPosition.y <= EdgePositionYWithJet)
                transform.localPosition = new Vector3(transform.localPosition.x, EdgePositionYWithJet, transform.localPosition.z);
        }
    }

    private void KeepPlayerOnRoad()
    {
        if (transform.localPosition.x < -MapEdgeConstants.EdgePosX + playerStartPositionX)
            transform.localPosition = new Vector3(-MapEdgeConstants.EdgePosX + playerStartPositionX, transform.localPosition.y, transform.localPosition.z);

        if (transform.localPosition.x > playerStartPositionX + MapEdgeConstants.EdgePosX)
            transform.localPosition = new Vector3(MapEdgeConstants.EdgePosX + playerStartPositionX, transform.localPosition.y, transform.localPosition.z);

        if (transform.localPosition.z < PlayerEdgePositionBackZ)
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, PlayerEdgePositionBackZ);

        if (transform.localPosition.z > PlayerEdgePositionFrontZ)
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, PlayerEdgePositionFrontZ);
    }

    public void MovePlayer(float horizontalInput, float verticalInput)
    {
        transform.Translate(horizontalInput * movementSpeed * Runner.DeltaTime * Vector3.right, Space.World);
        transform.Translate(movementSpeed * Runner.DeltaTime * verticalInput * Vector3.forward, Space.World);
        SpinWhileFlying(horizontalInput);
    }

    private void SpinWhileFlying(float horizontalInput)
    {
        if (jetHandler.GetJetOnBack()?.HasJet == true)
            transform.Rotate(0, 0, horizontalInput * -5, Space.World);
        else
            transform.eulerAngles = Vector3.zero;
    }

    public void SetMovementSpeed(float speed)
    {
        movementSpeed = speed;
    }

    public void EnableMovement()
    {
        movementEnabled = true;
    }
    public void DisableMovement()
    {
        movementEnabled = false;
    }

    public bool IsMovementEnabled() => movementEnabled;
}
