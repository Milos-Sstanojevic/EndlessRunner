using System.Collections;
using UnityEngine;

public class JetController : CollectableBase
{
    private const int jetTimeToLive = 5;
    private static Vector3 defaultJetOrientation = new Vector3(-90, 0, 0);
    [SerializeField] private GameObject engineParticleEffect;
    public bool HasJet { get; private set; }

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
        HasJet = true;
        transform.position = jetPosition;
        StartCoroutine(JetExpiration());
    }

    private IEnumerator JetExpiration()
    {
        yield return new WaitForSeconds(jetTimeToLive);
        //engineParticleEffect.SetActive(false);
        HasJet = false;
        transform.eulerAngles = defaultJetOrientation;
        EventManager.Instance.OnEnviromentDestroyed(this);
    }

    public void ReleaseJetToPool()
    {
        EventManager.Instance.OnEnviromentDestroyed(this);
    }
}
