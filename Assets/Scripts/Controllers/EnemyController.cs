using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour, IDestroyable
{
    private const int Negator = -1;
    private const string EffectsVolume = "effectsVolume";
    private int minimumHealth;
    [SerializeField] private int chanceForThisEnemy;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private AnimationManager enemyAnimator;
    [SerializeField] private EnemyScriptableObject enemyScriptableObject;
    [SerializeField] private GameObject deathExplosion;
    private int isOnEdge = -1;
    private int health;
    private bool hasRotated;
    private EnvironmentMovementController environmentComponent;
    private ParticleSystemManager particleSystemManager;
    private AudioSource audioSource;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        environmentComponent = GetComponent<EnvironmentMovementController>();
        particleSystemManager = GetComponent<ParticleSystemManager>();
    }

    private void OnEnable()
    {
        health = enemyScriptableObject.health;

        UpdateHealthSlider();
    }

    private void UpdateHealthSlider()
    {
        if (healthSlider.value != health)
            healthSlider.value = health;
    }

    private void Start()
    {
        minimumHealth = enemyScriptableObject.minimumHealth;
    }

    private void Update()
    {
        MoveLeftAndRight();

        //ovo treba preko audioMixera
        if (audioSource.volume != PlayerPrefs.GetFloat(EffectsVolume))
            audioSource.volume = PlayerPrefs.GetFloat(EffectsVolume);

        if (transform.position.z < MapEdgeConstants.PositionBehindPlayerAxisZ)
            Destroy();
    }

    public void Destroy()
    {
        audioSource.Stop();
        EventManager.Instance.OnEnemyDestroyed(this);
    }

    private void MoveLeftAndRight()
    {
        if (environmentComponent.MovementEnabled)
        {
            MoveEnemy(enemyAnimator, isOnEdge);
            RotateEnemy();
        }
        else if (enemyAnimator != null)
        {
            enemyAnimator.StopWalkAnimation();
        }
    }

    public void MoveEnemy(AnimationManager enemyAnimator, int isOnEdge)
    {
        if (!enemyScriptableObject.isGroundEnemy)
            transform.Translate(Vector3.up * isOnEdge * enemyScriptableObject.movementSpeed * Time.deltaTime, Space.World);

        if (enemyScriptableObject.isGroundEnemy)
        {
            enemyAnimator.StartWalkAnimation();
            transform.Translate(Vector3.left * isOnEdge * enemyScriptableObject.movementSpeed * Time.deltaTime, Space.World);
        }
    }

    private void RotateEnemy()
    {
        if (!hasRotated && enemyScriptableObject.IsEnemyOnEdge(transform.position))
        {
            transform.Rotate(enemyScriptableObject.faceTheOtherWay * isOnEdge);
            isOnEdge *= Negator;
            hasRotated = true;
        }
        else if (!enemyScriptableObject.IsEnemyOnEdge(transform.position))
        {
            hasRotated = false;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        UpdateHealthSlider();

        if (health <= minimumHealth)
        {
            health = enemyScriptableObject.fullHealth;
            PlayDeathParticles();
            EventManager.Instance.OnEnemyDestroyed(this);
            EventManager.Instance.OnEnemyKilled(enemyScriptableObject.enemyWorth);
        }
    }

    public void PlayDeathParticles()
    {
        if (!enemyScriptableObject.isGroundEnemy)
        {
            Instantiate(deathExplosion, transform.position, Quaternion.identity);
            deathExplosion.GetComponent<ParticleSystem>().Play();
        }
    }

    public void PlayBloodParticles()
    {
        if (enemyScriptableObject.isGroundEnemy)
        {
            particleSystemManager.PlayBloodParticleEffect();
        }
    }

    public int GetChanceForThisEnemy() => chanceForThisEnemy;
}
