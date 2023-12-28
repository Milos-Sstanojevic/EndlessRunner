using UnityEngine;
using static GlobalConstants;

public class PlayerController : MonoBehaviour
{

    private Rigidbody playerRb;
    private Animator Animator;
    private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float jumpForce;
    private bool canJump = true;
    public float GravityModifier;
    private float edgePosX = 4;


    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        Physics.gravity *= GravityModifier;
        canJump = true;
    }

    void Update()
    {
        Movement();
        if (GameManager.Instance.CurrentState == GameStates.Playing)
        {
            Animator.SetBool(RunAnimation, true);
        }
        else
        {
            Animator.SetBool(RunAnimation, false);
        }
    }


    void Movement()
    {
        if (GameManager.Instance.IsGameActive)
        {
            MoveLeftOrRight();

            if (canJump)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Jump();
                }
            }

            KeepPlayerOnRoad();
        }
    }

    void MoveLeftOrRight()
    {
        float horizontalInput = Input.GetAxis(HorizontalAxis);

        transform.Translate(Vector3.right * GameManager.Instance.MovingSpeed * Time.deltaTime * horizontalInput);
    }


    private void Jump()
    {
        Animator.SetBool(JumpAnimation, true);
        audioSource.PlayOneShot(jumpSound, ClipVolume);
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        canJump = false;
    }

    private void KeepPlayerOnRoad()
    {
        if (transform.position.x < -edgePosX)
        {
            transform.position = new Vector3(-edgePosX, transform.position.y, transform.position.z);
        }
        if (transform.position.x > edgePosX)
        {
            transform.position = new Vector3(edgePosX, transform.position.y, transform.position.z);
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(ObstacleTag))
        {
            GameManager.Instance.GameOver();
            if (ShouldDropPlayerOnGround())
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);

            Animator.SetBool(DeadAnimation, true);
            audioSource.PlayOneShot(deathSound, ClipVolume);
        }

        if (other.gameObject.CompareTag(GroundTag))
        {
            canJump = true;
            Animator.SetBool(JumpAnimation, false);
        }
    }

    private bool ShouldDropPlayerOnGround()
    {
        return transform.position.y != 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        ICollectible collectible = other.GetComponent<ICollectible>();
        if (collectible != null)
        {
            collectible.Collect();
        }
    }

    private void OnDestroy()
    {
        Physics.gravity /= GravityModifier;
    }
}
