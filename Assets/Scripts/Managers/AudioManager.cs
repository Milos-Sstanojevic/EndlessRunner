using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private const float ClipVolume = 0.5f;
    private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip spaceshipCollectedSound;
    [SerializeField] private AudioClip jetCollectedSound;
    [SerializeField] private AudioClip gunCollectedSound;

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

    public void PlayJetCollectedSound()
    {
        PlaySound(jetCollectedSound);
    }

    public void PlayGunCollectedSound()
    {
        PlaySound(gunCollectedSound);
    }

    public void PlayJumpSound()
    {
        PlaySound(jumpSound);
    }

    public void PlayDeathSound()
    {
        PlaySound(deathSound);
    }

    public void PlaySpaceshipCollectedSound()
    {
        PlaySound(spaceshipCollectedSound);
    }

    private void PlaySound(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip, ClipVolume);
    }
    
    public void SetAudioSourceVolume(float value)
    {
        audioSource.volume = value;
    }
}
