using System.Collections;
using UnityEngine;

public class PlayerJetHandler : MonoBehaviour
{
    private static Vector3 RotateAroundX = new Vector3(90, 0, 0);
    private const int CollectablePointsWorth = 5;
    [SerializeField] private GameObject jetPosition;
    private const float FlyingPosition = 4.65f;
    private const float OffsetFromGround = 0.65f;
    private const float AsscendingSpeed = 5;
    private const float ZeroPosition = 0;
    private JetController jetOnBack;
    private Collider playerCollider;
    private PlayerMovement playerMovement;
    private bool isInAir = false;


    private void Awake()
    {
        playerCollider = GetComponent<Collider>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (playerMovement.IsMovementEnabled())
            ContinueJetCoroutine();


        if (jetOnBack?.HasJet == false && isInAir)
        {
            StartCoroutine(SlowlyStartOrStopFlying(Vector3.zero, new Vector3(transform.position.x, ZeroPosition + OffsetFromGround, transform.position.z)));
            jetOnBack = null;
        }
    }

    public IEnumerator SlowlyStartOrStopFlying(Vector3 endRotation, Vector3 endPosition)
    {
        isInAir = !isInAir;
        playerCollider.enabled = false;

        Vector3 startPos = transform.position;
        Vector3 startRotation = transform.rotation.eulerAngles;
        Quaternion startQuaternion = Quaternion.Euler(startRotation);
        Quaternion endQuaternion = Quaternion.Euler(endRotation);
        float distance = Vector3.Distance(startPos, endPosition);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            transform.position = Vector3.Lerp(startPos, endPosition, 1 - (remainingDistance / distance));
            transform.rotation = Quaternion.Slerp(startQuaternion, endQuaternion, 1 - (remainingDistance / distance));

            remainingDistance -= AsscendingSpeed * Time.deltaTime;

            yield return null;
        }

        playerCollider.enabled = true;
        transform.position = endPosition;
        transform.eulerAngles = endRotation;
    }

    public int CollectJet(JetController collectable)
    {
        JetController jet = collectable;

        if (jetOnBack == null && !isInAir)
        {
            jetOnBack = jet;
            StartCoroutine(SlowlyStartOrStopFlying(RotateAroundX, new Vector3(transform.position.x, ZeroPosition + FlyingPosition, transform.position.z)));
            jet.MoveOnPlayerBack(this, jetPosition.transform.position);
        }
        else
            jet.ReleaseJetToPool();

        AudioManager.Instance.PlayJetCollectedSound();

        return CollectablePointsWorth * 2;
    }

    private void ContinueJetCoroutine()
    {
        if (jetOnBack != null)
            jetOnBack.UnpauseCoroutine();
    }

    public void ReleaseJetIfHaveOne()
    {
        if (jetOnBack != null)
            StartCoroutine(ReleaseJetCoroutine());
    }

    private IEnumerator ReleaseJetCoroutine()
    {
        jetOnBack.ReleaseJetToPool();
        yield return new WaitForEndOfFrame();
        jetOnBack = null;
    }

    public JetController GetJetOnBack() => jetOnBack;
    public bool HasJetOnBack() => jetOnBack != null;
    public bool IsInAir() => isInAir;
}
