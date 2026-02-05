using System;

// 这个脚本不需要挂在任何物体上，它是静态的工具
public static class GameEventManager
{
    // 1. 游戏启动信号
    public static event Action OnGameStart;
    public static void CallGameStart() => OnGameStart?.Invoke();

    // 2. 怪物死亡信号 
    // 参数：xp(经验), level(怪物等级), groupID(掉落组ID)
    // 注意：哪怕爬塔模式不需要经验，为了兼容性，我们还是传这三个参数
    public static event Action<int, int, int> OnEnemyDead;
    public static void CallEnemyDead(int xp, int level, int groupID) => OnEnemyDead?.Invoke(xp, level, groupID);

    // 3. 数据需要更新信号 (通知存档和查表)
    public static event Action OnDataNeedUpdate;
    public static void CallDataNeedUpdate() => OnDataNeedUpdate?.Invoke();

    // 4. 属性计算完毕信号 (通知UI和战斗实体)
    public static event Action OnStatsChanged;
    public static void CallStatsChanged() => OnStatsChanged?.Invoke();

    // 5. 换武器信号 (通知动画)
    public static event Action<int> OnWeaponSwapped;
    public static void CallWeaponSwapped(int weaponID) => OnWeaponSwapped?.Invoke(weaponID);

    // 6. 玩家状态改变信号 (是移动还是攻击？)
    public static event Action<PlayerState> OnPlayerStateChanged;
    public static void CallPlayerStateChanged(PlayerState state) => OnPlayerStateChanged?.Invoke(state);

    // ==========================================
    // 【新增】爬塔专用信号
    // ==========================================

    // 7. 关卡胜利 (爬塔)
    public static event Action OnLevelVictory;
    public static void CallLevelVictory() => OnLevelVictory?.Invoke();

    // 8. 关卡失败 (爬塔 - 时间到或玩家死)
    public static event Action OnLevelFailed;
    public static void CallLevelFailed() => OnLevelFailed?.Invoke();
}