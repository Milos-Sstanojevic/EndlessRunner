using UnityEngine;

public class ParticleSystemManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem jumpingParticles;
    [SerializeField] private ParticleSystem landingParticles;
    [SerializeField] private ParticleSystem enemyBloodParticles;
    [SerializeField] private ParticleSystem[] jetEngineParticles;
    [SerializeField] private ParticleSystem flyingEnemyExplosion;
    private bool canPlayLandingParticles;

    public void PlayLandingParticleEffect()
    {
        if (!canPlayLandingParticles)
            return;

        canPlayLandingParticles = false;
        landingParticles.Play();
    }

    public void PlayJumpingParticleEffect()
    {
        canPlayLandingParticles = true;
        jumpingParticles.Play();
    }

    public void PlayJetEngineParticleEffect()
    {
        foreach (ParticleSystem particle in jetEngineParticles)
            particle.Play();
    }

    public void PlayBloodParticleEffect()
    {
        enemyBloodParticles.Play();
    }

    public void PlayDeathParticles()
    {
        flyingEnemyExplosion.Play();
    }
}
