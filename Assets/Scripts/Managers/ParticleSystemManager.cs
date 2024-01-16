using UnityEngine;

public class ParticleSystemManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem jumpingParticles;
    [SerializeField] private ParticleSystem landingParticles;
    [SerializeField] private ParticleSystem muzzleParticle;
    private bool canPlayLangingParticles;
    public void PlayLandingParticleEffect()
    {
        if (canPlayLangingParticles == true)
        {
            canPlayLangingParticles = false;
            landingParticles.Play();
        }
    }

    public void PlayJumpingParticleEffect()
    {
        canPlayLangingParticles = true;
        jumpingParticles.Play();
    }

    public void PlayMuzzleParticleEffect()
    {
        muzzleParticle.Play();
    }

    public void StopMuzzleParticleEffect()
    {
        muzzleParticle.Stop();
    }
}
