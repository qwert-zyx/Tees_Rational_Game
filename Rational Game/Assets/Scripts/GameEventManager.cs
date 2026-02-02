using System;

// 这个脚本不需要挂在任何物体上，它是静态的工具
public static class GameEventManager
{
    // 1. 游戏启动信号
    public static event Action OnGameStart;
    public static void CallGameStart() => OnGameStart?.Invoke();

    // 2. 怪物死亡信号 (传递参数：经验，怪物等级，掉落组ID)
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
}