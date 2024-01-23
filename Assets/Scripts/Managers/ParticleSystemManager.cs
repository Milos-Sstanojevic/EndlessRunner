using System.Collections;
using UnityEngine;

public class ParticleSystemManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem jumpingParticles;
    [SerializeField] private ParticleSystem landingParticles;
    [SerializeField] private ParticleSystem[] jetEngineParticles;
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

    public void PlayJetEngineParticleEffect()
    {
        for (int i = 0; i < jetEngineParticles.Length; i++)
            jetEngineParticles[i].Play();
    }
}
