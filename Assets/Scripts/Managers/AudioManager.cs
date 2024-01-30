using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private const float clipVolume = 0.5f;
    private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip spaceshipCollectedSound;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayJumpSound()
    {
        audioSource.PlayOneShot(jumpSound, clipVolume);
    }

    public void PlayDeathSound()
    {
        audioSource.PlayOneShot(deathSound, clipVolume);
    }

    public void PlaySpaceshipCollectedSound()
    {
        audioSource.PlayOneShot(spaceshipCollectedSound, clipVolume);
    }
}
