using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
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
        playerStartPositionX = transform.position.x;
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
            if (transform.position.z > EdgePositionZWithJet)
                transform.position = new Vector3(transform.position.x, transform.position.y, EdgePositionZWithJet);
            if (transform.position.y <= EdgePositionYWithJet)
                transform.position = new Vector3(transform.position.x, EdgePositionYWithJet, transform.position.z);
        }
    }

    private void KeepPlayerOnRoad()
    {
        if (transform.position.x < -MapEdgeConstants.EdgePosX + playerStartPositionX)
            transform.position = new Vector3(-MapEdgeConstants.EdgePosX + playerStartPositionX, transform.position.y, transform.position.z);

        if (transform.position.x > playerStartPositionX + MapEdgeConstants.EdgePosX)
            transform.position = new Vector3(MapEdgeConstants.EdgePosX + playerStartPositionX, transform.position.y, transform.position.z);

        if (transform.position.z < PlayerEdgePositionBackZ)
            transform.position = new Vector3(transform.position.x, transform.position.y, PlayerEdgePositionBackZ);

        if (transform.position.z > PlayerEdgePositionFrontZ)
            transform.position = new Vector3(transform.position.x, transform.position.y, PlayerEdgePositionFrontZ);
    }

    public void MovePlayer(float horizontalInput, float verticalInput)
    {
        transform.Translate(Vector3.right * movementSpeed * Time.deltaTime * horizontalInput, Space.World);
        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime * verticalInput, Space.World);
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
