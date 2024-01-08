using UnityEngine;
using static GlobalConstants;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private PlayerController player;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip spaceshipCollectedSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        PlayJumpSound();
        PlayDeathSound();
        PlaySpaceshipCollectedSound();
    }

    public void PlayJumpSound()
    {
        if (player.ShouldPlayJumpSound == true)
        {
            audioSource.PlayOneShot(jumpSound, ClipVolume);
            player.ShouldPlayJumpSound = false;
        }
    }
    public void PlayDeathSound()
    {
        if (player.ShouldPlayDeathSound == true)
        {
            audioSource.PlayOneShot(deathSound, ClipVolume);
            player.ShouldPlayDeathSound = false;
        }
    }
    public void PlaySpaceshipCollectedSound()
    {
        if (player.ShouldPlaySpaceshipCollectedSound == true)
        {
            audioSource.PlayOneShot(spaceshipCollectedSound, ClipVolume);
            player.ShouldPlaySpaceshipCollectedSound = false;
        }
    }
}
