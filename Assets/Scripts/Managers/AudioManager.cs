using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private const float ClipVolume = 0.5f;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayJumpSound(AudioClip jumpSound)
    {
        audioSource.PlayOneShot(jumpSound, ClipVolume);
    }
    public void PlayDeathSound(AudioClip deathSound)
    {
        audioSource.PlayOneShot(deathSound, ClipVolume);
    }

    public void PlaySpaceshipCollectedSound(AudioClip spaceshipCollectedSound)
    {
        audioSource.PlayOneShot(spaceshipCollectedSound, ClipVolume);
    }
}
