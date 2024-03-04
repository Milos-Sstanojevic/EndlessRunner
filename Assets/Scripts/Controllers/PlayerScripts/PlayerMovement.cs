using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
    private float movementSpeed;
    private PlayerJetHandler jetHandler;
    private Rigidbody playerRb;
    private bool movementEnabled;
    private Vector2 movementInput = Vector2.zero;
    private bool jumped;
    private bool canJump = true;
    private bool isGameOnline;

    private void Awake()
    {
        playerParticleSystem = GetComponent<ParticleSystemManager>();
        playerAnimationHandler = GetComponent<PlayerAnimationHandler>();
        jetHandler = GetComponent<PlayerJetHandler>();
        playerRb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (!isGameOnline)
            playerStartPositionX = transform.position.x;
    }

    public override void Spawned()
    {
        SetGameToOnline();
    }

    private void OnEnable()
    {
        EventManager.Instance.SubscribeToOnSetGameToOnlineAction(SetGameToOnline);
        EventManager.Instance.SubscribeToOnSetGameToOfflineAction(SetGameToOfflline);
    }

    private void SetGameToOnline()
    {
        isGameOnline = true;
    }

    private void SetGameToOfflline()
    {
        isGameOnline = false;
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
        if (isGameOnline)
            return;

        MovementFunctionsCall();
    }

    public override void FixedUpdateNetwork()
    {
        if (!isGameOnline)
            return;

        MovementFunctionsCall();
    }

    private void MovementFunctionsCall()
    {
        KeepPlayerOnRoad();
        KeepPlayerInCameraFieldIfHasJet();

        if (!movementEnabled)
            return;

        MovePlayer();
        HandlePlayerJumping();
    }


    private void HandlePlayerJumping()
    {
        if (!isGameOnline && jumped && canJump && !jetHandler.IsInAir())
            Jump();

        if (jumped && canJump && !jetHandler.IsInAir() && GetInput(out NetworkInputData data))
            RPC_HandlePlayerJumping(this, data);
    }

    private void Jump()
    {
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        playerParticleSystem.PlayJumpingParticleEffect();
        playerAnimationHandler.PlayJumpAnimation();
        AudioManager.Instance.PlayJumpSound();
        canJump = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_HandlePlayerJumping(PlayerMovement playerMovement, NetworkInputData data)
    {
        playerMovement.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        data.Jumped = jumped;
        playerParticleSystem.PlayJumpingParticleEffect();
        playerAnimationHandler.PlayJumpAnimation();
        AudioManager.Instance.PlayJumpSound();
        canJump = false;
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

    public void MovePlayer()
    {
        if (!isGameOnline)
        {
            Move();
        }
        else if (GetInput(out NetworkInputData data))
        {
            data.MovementInput = movementInput;
            data.Jumped = jumped;

            RPC_MovePlayer(this, data);

            NetworkSpawner.BufferedInput = data;
        }

        SpinWhileFlying(movementInput.x);
    }

    private void Move()
    {
        transform.Translate(movementInput.x * movementSpeed * Time.deltaTime * Vector3.right, Space.World);
        transform.Translate(movementSpeed * Time.deltaTime * movementInput.y * Vector3.forward, Space.World);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_MovePlayer(PlayerMovement player, NetworkInputData data)
    {
        player.transform.Translate(data.MovementInput.x * movementSpeed * Runner.DeltaTime * Vector3.right, Space.World);
        player.transform.Translate(movementSpeed * Runner.DeltaTime * data.MovementInput.y * Vector3.forward, Space.World);
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

    public void SetStartPositionOfOfPlayer(float posX)
    {
        playerStartPositionX = posX;
    }
}
