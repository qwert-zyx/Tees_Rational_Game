using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // 受击逻辑
    public void TakeDamage(float damage)
    {
        var db = PlayerBaseData.Instance;

        // 1. 计算伤害
        // 可以在这里加上防御力减伤逻辑: float finalDamage = damage - db.finalDEF;
        // 既然你通过配表控制攻击力为0，这里直接减就行
        float finalDamage = Mathf.Max(0, damage); // 确保伤害不会变成负数（加血）

        if (finalDamage > 0)
        {
            db.currentHP -= finalDamage;
            Debug.Log($"<color=red>玩家受到伤害: -{finalDamage}, 剩余: {db.currentHP}</color>");

            // 只有掉血了才需要通知 UI 刷新血条
            GameEventManager.CallDataNeedUpdate();
        }

        // 2. 死亡判定
        if (db.currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // 防止重复死亡逻辑（比如一瞬间被多颗子弹打中）
        if (PlayerBaseData.Instance.currentHP > 0) return;

        Debug.Log("【玩家死亡】发送广播...");

        // 我们不在这里暂停游戏，也不在这里切场景
        // 我们只发广播，让 UIManager 去处理界面和流程
        // 这里我们可以复用 CallStatsChanged 或者定义一个新的 OnPlayerDead 信号
        // 为了省事，直接用 CallDataNeedUpdate 让 UI 检测血量，或者在 UIManager 里直接检测

        // 实际上，为了严谨，我们应该在 UIManager 里每帧检测或者监听更新
        // 这里我们把血量强制归零（防止负数）
        PlayerBaseData.Instance.currentHP = 0;
        GameEventManager.CallDataNeedUpdate();
    }
}