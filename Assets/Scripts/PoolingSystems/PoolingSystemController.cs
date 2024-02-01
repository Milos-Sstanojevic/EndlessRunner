using UnityEngine;

public class PoolingSystemController : MonoBehaviour
{
    public static PoolingSystemController Instance { get; private set; }

    [SerializeField] private EnvironmentMovementController leftAndRightObstacle;
    [SerializeField] private EnvironmentMovementController leftObstacle;
    [SerializeField] private EnvironmentMovementController rightObstacle;
    [SerializeField] private EnvironmentMovementController roadblockObstacle;
    [SerializeField] private EnemyController flyingEnemy;
    [SerializeField] private EnemyController groundEnemy;
    [SerializeField] private JetPoolingSystem jetPoolingSystem;
    [SerializeField] private BulletPoolingSystem bulletPoolingSystem;
    [SerializeField] private ChunkPoolingSystem chunkPoolingSystem;
    [SerializeField] private EnemyPoolingSystem enemyPoolingSystem;
    [SerializeField] private GunPoolingSystem gunPoolingSystem;
    [SerializeField] private ObstaclesPoolingSystem obstaclesPoolingSystem;
    [SerializeField] private SpaceshipPoolingSystem spaceshipPoolingSystem;
    [SerializeField] private ChunkController twoEnemiesChunk;
    [SerializeField] private ChunkController flyingEnemyChunk;
    [SerializeField] private ChunkController completeChunk;
    [SerializeField] private ChunkController randomObstaclesChunk;

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
    }

    public JetPoolingSystem GetJetPoolingSystem() => jetPoolingSystem;
    public BulletPoolingSystem GetBulletPoolingSystem() => bulletPoolingSystem;
    public ChunkPoolingSystem GetChunkPoolingSystem() => chunkPoolingSystem;
    public EnemyPoolingSystem GetEnemyPoolingSystem() => enemyPoolingSystem;
    public GunPoolingSystem GetGunPoolingSystem() => gunPoolingSystem;
    public ObstaclesPoolingSystem GetObstaclesPoolingSystem() => obstaclesPoolingSystem;
    public SpaceshipPoolingSystem GetSpaceshipPoolingSystem() => spaceshipPoolingSystem;
    public EnvironmentMovementController GetRoadblockObstaclePrefab() => roadblockObstacle;
    public EnvironmentMovementController GetLeftObstaclePrefab() => leftObstacle;
    public EnvironmentMovementController GetRightObstaclePrefab() => rightObstacle;
    public EnvironmentMovementController GetLeftAndRightObstaclePrefab() => leftAndRightObstacle;
    public EnemyController GetFlyingEnemyPrefab() => flyingEnemy;
    public EnemyController GetGroundEnemyPrefab() => groundEnemy;
    public ChunkController GetChunkWithTwoEnemies() => twoEnemiesChunk;
    public ChunkController GetChunkWithFlyingEnemy() => flyingEnemyChunk;
    public ChunkController GetCompleteChunk() => completeChunk;
    public ChunkController GetChunkWithRandomObstacles() => randomObstaclesChunk;
}
