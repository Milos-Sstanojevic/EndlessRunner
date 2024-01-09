using UnityEngine;
using static GlobalConstants;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private PlayerController player;
    [SerializeField] private AudioClip spaceshipCollectedSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        PlaySpaceshipCollectedSound();
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
