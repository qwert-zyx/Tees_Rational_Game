using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("基础设置")]
    public GameObject enemyPrefab;
    public Transform spawnPoint;

    [Header("挂机模式设置")]
    public float spawnInterval = 3f; // 几秒刷一波
    public int[] spawnCountPool = { 1, 1, 2 }; // 每次随机刷几只

    [Header("爬塔模式设置")]
    public int towerSpawnCount = 3; // 爬塔模式固定刷 3 只
    public float enemySpacing = 1.5f; // 怪物间距

    private float timer;
    private bool canSpawn = true; // 控制挂机模式的开关

    void Awake()
    {
        GameEventManager.OnPlayerStateChanged += HandleStateChange;
    }

    void OnDestroy()
    {
        GameEventManager.OnPlayerStateChanged -= HandleStateChange;
    }

    void Start()
    {
        // ==========================================
        // 【核心修改】如果是爬塔模式，开局直接刷完收工
        // ==========================================
        if (PlayerBaseData.Instance.isTowerMode)
        {
            SpawnTowerWave();
        }
    }

    void HandleStateChange(PlayerState state)
    {
        // 挂机模式下，只有玩家跑起来才刷怪
        // 爬塔模式下，这个开关不影响（因为已经在 Start 里生成完了）
        canSpawn = (state == PlayerState.Moving);
    }

    void Update()
    {
        // ==========================================
        // 【核心修改】爬塔模式不需要计时器，直接 return
        // ==========================================
        if (PlayerBaseData.Instance.isTowerMode) return;

        // --- 下面是原有的挂机刷怪逻辑 ---
        if (!canSpawn) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnGrindWave();
            timer = 0;
        }
    }

    // --- 逻辑 A：挂机模式刷一波 (随机数量) ---
    void SpawnGrindWave()
    {
        if (enemyPrefab == null || spawnCountPool.Length == 0) return;

        int randomIndex = Random.Range(0, spawnCountPool.Length);
        int count = spawnCountPool[randomIndex];

        GenerateEnemies(count);
    }

    // --- 逻辑 B：爬塔模式刷一波 (固定数量) ---
    void SpawnTowerWave()
    {
        if (enemyPrefab == null) return;

        Debug.Log($"【爬塔模式】开始生成 {towerSpawnCount} 个敌人...");
        GenerateEnemies(towerSpawnCount);
    }

    // --- 公用的生成方法 (不重复造轮子) ---
    void GenerateEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // 计算排列位置，横向排开
            Vector3 offset = Vector3.right * (i * enemySpacing);
            Vector3 finalPos = (spawnPoint != null) ? spawnPoint.position + offset : transform.position + offset;

            CreateEnemy(finalPos);
        }
    }

    // --- 创建单体并注入数据 ---
    void CreateEnemy(Vector3 pos)
    {
        GameObject newEnemy = Instantiate(enemyPrefab, pos, Quaternion.identity);

        // 注入数据
        var db = PlayerBaseData.Instance;
        var enemyData = newEnemy.GetComponent<SingleEnemyData>();

        if (enemyData != null)
        {
            // 注意：这里读取的 template 属性，已经在 CheckCSV 里根据模式自动切换好了
            // 所以这里不需要再写 if else，直接用就行
            enemyData.Init(db.enemyTemplateHP, db.enemyTemplateATK, db.enemyTemplateXP, db.enemyTemplateGroupID);
        }
    }
}