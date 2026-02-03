using UnityEngine;

public class EnemyCombatEntity : MonoBehaviour
{
    public float attackInterval = 1.5f; // 怪物攻速慢一点
    public float rayDistance = 1.0f;    // 攻击距离
    public LayerMask playerLayer;       // 只打玩家层

    private float timer;
    private float myAtk;
    private bool isAttacking = false;

    // 引用自己的数据（为了获取攻击力）
    private SingleEnemyData myData;

    void Start()
    {
        myData = GetComponent<SingleEnemyData>();
        myAtk = myData.atk; // 获取初始化时分配的攻击力

        // 监听玩家状态：玩家停下(Attack)时，我也停下开始攻击
        GameEventManager.OnPlayerStateChanged += HandleStateChange;
    }

    void OnDestroy()
    {
        GameEventManager.OnPlayerStateChanged -= HandleStateChange;
    }

    void HandleStateChange(PlayerState state)
    {
        // 玩家攻击 = 我也被挡住了 = 我开始攻击
        isAttacking = (state == PlayerState.Attacking);
    }

    void Update()
    {
        if (isAttacking)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                TryAttackPlayer();
                timer = attackInterval;
            }
        }
    }

    void TryAttackPlayer()
    {
        // 向左发射射线 (Vector2.left)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, rayDistance, playerLayer);
        Debug.DrawRay(transform.position, Vector3.left * rayDistance, Color.yellow, 0.5f);

        if (hit.collider != null)
        {
            // 这里我们需要一个让玩家扣血的方法
            // 我们暂时假设玩家身上有一个接收伤害的脚本，稍后在第二步写
            var player = hit.collider.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(myAtk);
            }
        }
    }
}