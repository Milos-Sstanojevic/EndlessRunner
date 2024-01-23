using System.Collections;
using UnityEngine;

public class JetController : CollectableBase
{
    private const int jetTimeToLive = 5;
    private static Vector3 defaultJetOrientation = new Vector3(-90, 0, 0);
    [SerializeField] private GameObject engineParticleEffect;
    private ParticleSystemManager particleSystemManager;
    public bool HasJet { get; private set; }

    private void Start()
    {
        particleSystemManager = GetComponent<ParticleSystemManager>();
    }

    protected override void Update()
    {
        if (!HasJet)
        {
            base.Update();
        }
    }

    public void MoveOnPlayerBack(PlayerController player, Vector3 jetPosition)
    {
        transform.SetParent(player.transform);
        transform.eulerAngles = defaultJetOrientation;
        engineParticleEffect.SetActive(true);
        particleSystemManager.PlayJetEngineParticleEffect();
        HasJet = true;
        transform.position = jetPosition;
        StartCoroutine(JetExpiration());
    }

    private IEnumerator JetExpiration()
    {
        yield return new WaitForSeconds(jetTimeToLive);
        engineParticleEffect.SetActive(false);
        HasJet = false;
        transform.eulerAngles = defaultJetOrientation;
        EventManager.Instance.OnEnviromentDestroyed(this);
    }

    public void ReleaseJetToPool()
    {
        EventManager.Instance.OnEnviromentDestroyed(this);
    }

    public void PauseCoroutine()
    {
        // Time.timeScale = 0f;
    }

    public void UnpauseCoroutine()
    {
        //Time.timeScale = 1f;
    }
}
