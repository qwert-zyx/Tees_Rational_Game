using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // 受击逻辑
    public void TakeDamage(float damage)
    {
        var db = PlayerBaseData.Instance;

        // 1. 计算伤害
        float finalDamage = Mathf.Max(0, damage);

        if (finalDamage > 0)
        {
            db.currentHP -= finalDamage;
            Debug.Log($"<color=red>玩家受到伤害: -{finalDamage}, 剩余: {db.currentHP}</color>");

            // 刷新 UI 血条
            GameEventManager.CallDataNeedUpdate();
        }

        // 2. 死亡判定
        if (db.currentHP <= 0)
        {
            // 锁死血量为0，防止UI显示负数
            db.currentHP = 0;
            // 再次强制刷新一下UI，确保血条归零
            GameEventManager.CallDataNeedUpdate();

            Die();
        }
    }

    void Die()
    {
        Debug.Log("【玩家死亡】执行死亡逻辑...");

        // ==========================================
        // 【核心修复】频道对齐！
        // ==========================================

        // 如果当前是爬塔模式，必须发送 "OnLevelFailed"
        // 因为 TowerGameManager 只听这个！
        if (PlayerBaseData.Instance.isTowerMode)
        {
            Debug.Log("【信号发送】当前是爬塔模式，通知裁判：挑战失败！");
            GameEventManager.CallLevelFailed();
        }
        else
        {
            // 如果是挂机模式，可能只是重生或者别的逻辑
            Debug.Log("【信号发送】挂机模式死亡，暂时不做处理");
        }

        // ==========================================
        // 3. 处理尸体
        // ==========================================

        // 简单粗暴：直接禁用玩家物体，防止他死后还能移动/被攻击
        // 如果你有死亡动画，可以在动画播完后再 SetActive(false)
        gameObject.SetActive(false);
    }
}