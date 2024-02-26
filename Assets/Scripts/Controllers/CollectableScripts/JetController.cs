using System.Collections;
using Fusion;
using UnityEngine;

public class JetController : NetworkBehaviour, IDestroyable
{
    private const int JetTimeToLive = 5;
    private static Vector3 DefaultJetOrientation = new Vector3(-90, 0, 0);
    [SerializeField] private GameObject engineParticleEffect;
    private EnvironmentMovementController environmentComponent;
    private CollectableController collectableComponent;
    private ParticleSystemManager particleSystemManager;
    public bool HasJet { get; private set; }

    private void Awake()
    {
        particleSystemManager = GetComponent<ParticleSystemManager>();
        environmentComponent = GetComponent<EnvironmentMovementController>();
        collectableComponent = GetComponent<CollectableController>();
    }

    private void Update()
    {
        if (!HasJet)
        {
            environmentComponent.enabled = true;
            collectableComponent.enabled = true;
        }


        if (transform.position.z < MapEdgeConstants.PositionBehindPlayerAxisZ)
            Destroy();
    }

    public void Destroy()
    {
        EventManager.Instance.OnJetDestroyed(this);
    }

    public void MoveOnPlayerBack(PlayerJetHandler player, Vector3 jetPosition)
    {
        transform.SetParent(player.transform);
        transform.eulerAngles = DefaultJetOrientation;
        engineParticleEffect.SetActive(true);
        particleSystemManager.PlayJetEngineParticleEffect();
        HasJet = true;
        environmentComponent.enabled = false;
        collectableComponent.enabled = false;
        transform.position = jetPosition;
        StartCoroutine(JetExpiration());
    }

    private IEnumerator JetExpiration()
    {
        yield return new WaitForSeconds(JetTimeToLive);
        engineParticleEffect.SetActive(false);
        HasJet = false;
        transform.eulerAngles = DefaultJetOrientation;
        EventManager.Instance.OnJetDestroyed(this);
    }

    public void ReleaseJetToPool()
    {
        EventManager.Instance.OnJetDestroyed(this);
    }

    public void PauseCoroutine()
    {
        Time.timeScale = 0f;
    }

    public void UnpauseCoroutine()
    {
        Time.timeScale = 1f;
    }
}
