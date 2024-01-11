using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private const float clipVolume = 0.5f;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayJumpSound(AudioClip jumpSound)
    {
        audioSource.PlayOneShot(jumpSound, clipVolume);
    }
    public void PlayDeathSound(AudioClip deathSound)
    {
        audioSource.PlayOneShot(deathSound, clipVolume);
    }

    public void PlaySpaceshipCollectedSound(AudioClip spaceshipCollectedSound)
    {
        audioSource.PlayOneShot(spaceshipCollectedSound, clipVolume);
    }
}
