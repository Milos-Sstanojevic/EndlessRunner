using UnityEngine;
using static GlobalConstants;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip spaceshipCollectedSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayJumpSound() => audioSource.PlayOneShot(jumpSound, ClipVolume);
    public void PlayDeathSound() => audioSource.PlayOneShot(deathSound, ClipVolume);
    public void PlaySpaceshipCollectedSound() => audioSource.PlayOneShot(spaceshipCollectedSound, ClipVolume);
}
