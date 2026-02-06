using UnityEngine;

public class EnemyCombatEntity : MonoBehaviour
{
    [Header("调试开关")]
    public bool showDebugLogs = true; // 可以在Inspector里关掉防止刷屏

    public float attackInterval = 1.5f;
    public float rayDistance = 1.0f;
    public LayerMask playerLayer;

    private float timer;
    private float myAtk;
    private bool isAttacking = false;

    private SingleEnemyData myData;

    void Start()
    {
        myData = GetComponent<SingleEnemyData>();

        // 【侦探 0】检查数据初始化顺序
        // 如果这里打印出来是 0，说明 Spawner 还没来得及给它赋值，这个脚本就先跑了
        if (showDebugLogs) Debug.Log($"【侦探 0】怪物出生，当前数据攻击力: {myData.atk}");

        // 建议：不要在这里缓存 myAtk，因为可能还没初始化。
        // 最好在攻击的那一刻去取 myData.atk，或者在 Init 方法里赋值。
        myAtk = myData.atk;

        GameEventManager.OnPlayerStateChanged += HandleStateChange;
    }

    void OnDestroy()
    {
        GameEventManager.OnPlayerStateChanged -= HandleStateChange;
    }

    void HandleStateChange(PlayerState state)
    {
        isAttacking = (state == PlayerState.Attacking);

        // 【侦探 1】检查状态同步
        if (showDebugLogs)
            Debug.Log($"【侦探 1】收到玩家状态: {state} | 怪物是否决定攻击: {isAttacking}");
    }

    void Update()
    {
        if (isAttacking)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                // 【侦探 2】计时器到点
                if (showDebugLogs) Debug.Log("【侦探 2】攻击冷却结束，准备挥刀！");

                TryAttackPlayer();
                timer = attackInterval;
            }
        }
    }

    void TryAttackPlayer()
    {
        // 重新获取一下最新的攻击力，防止Start时没取到
        if (myData != null) myAtk = myData.atk;

        // 【视觉辅助】画出红色的线，方便你看有没有够得着
        Debug.DrawRay(transform.position, Vector3.left * rayDistance, Color.red, 1.0f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, rayDistance, playerLayer);

        if (hit.collider != null)
        {
            // 【侦探 3】射线打中东西了
            if (showDebugLogs)
                Debug.Log($"【侦探 3】射线击中了物体: {hit.collider.name} (Layer: {hit.collider.gameObject.layer})");

            var player = hit.collider.GetComponent<PlayerHealth>();
            if (player != null)
            {
                // 【侦探 4】确认是玩家，并且有血条脚本
                if (showDebugLogs)
                    Debug.Log($"【侦探 4】找到 PlayerHealth 组件！执行扣血: {myAtk}");

                player.TakeDamage(myAtk);
            }
            else
            {
                // 【凶手可能是这里】打中了Player层的东西，但没找到 PlayerHealth 脚本
                Debug.LogError($"【侦探 4 失败】打中了 {hit.collider.name}，但是它身上没有 'PlayerHealth' 脚本！请检查玩家预制体！");
            }
        }
        else
        {
            // 【侦探 3 失败】射线挥空了
            // 如果你看见玩家在面前但他提示挥空，说明距离(rayDistance)太短，或者 LayerMask 设置错了
            if (showDebugLogs)
                Debug.Log($"【侦探 3 失败】攻击挥空！前方 {rayDistance} 米内没有检测到 PlayerLayer 的物体。");
        }
    }
}