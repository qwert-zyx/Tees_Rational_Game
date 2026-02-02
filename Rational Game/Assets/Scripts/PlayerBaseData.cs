using UnityEngine;

public class PlayerBaseData : MonoBehaviour
{
    public static PlayerBaseData Instance; // 单例，方便别人找

    [Header("存档数据")]
    public int level;
    public int currentXP;
    public int currentWeaponID;

    [Header("运行时计算数据 (只读)")]
    public int nextLevelXP;
    public float finalMaxHP;
    public float finalATK;
    public float finalDEF;

    // 临时存武器属性
    public float weaponATK;
    public float weaponHP;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); // 切换场景不销毁
    }
}