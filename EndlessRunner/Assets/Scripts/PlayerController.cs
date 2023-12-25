using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip deathSound;
    [SerializeField] private float jumpForce;
    private bool canJump = true;
    public float gravityModifier;
    private float edgePosX = 4;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Physics.gravity *= gravityModifier;
        canJump = true;
        audioSource = GetComponent<AudioSource>();
    }


    // Update is called once per frame
    void Update()
    {
        Movement();
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
        float horizontalInput = Input.GetAxis("Horizontal");

        transform.Translate(Vector3.right * GameManager.Instance.MovingSpeed * Time.deltaTime * horizontalInput);
    }


    private void Jump()
    {
        animator.SetBool("Jump", true);
        audioSource.PlayOneShot(jumpSound, 1.0f);
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
        if (other.gameObject.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver();
            if (ShouldDropPlayerOnGround())
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);

            animator.SetBool("Dead", true);
            audioSource.PlayOneShot(deathSound, 1.0f);
        }

        if (other.gameObject.CompareTag("Ground"))
        {
            canJump = true;
            animator.SetBool("Jump", false);
        }
    }

    private bool ShouldDropPlayerOnGround()
    {
        return (transform.position.y != 0);
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
        Physics.gravity /= gravityModifier; //ovo treba da izmenis
    }
}
