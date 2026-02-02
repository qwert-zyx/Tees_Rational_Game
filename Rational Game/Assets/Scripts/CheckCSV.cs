using UnityEngine;
using System.Collections.Generic;

public class CheckCSV : MonoBehaviour
{
    // 简单的CSV加载器，这里只演示最基础的读取逻辑
    // 实际项目中建议用更高级的CSV解析插件
    
    void Awake()
    {
        // 监听：游戏开始时，或数据需要更新时，都执行查表
        GameEventManager.OnGameStart += UpdateAllStats;
        GameEventManager.OnDataNeedUpdate += UpdateAllStats;
    }

    void OnDestroy()
    {
        // 养成好习惯，销毁时取消监听
        GameEventManager.OnGameStart -= UpdateAllStats;
        GameEventManager.OnDataNeedUpdate -= UpdateAllStats;
    }

    // 核心查表逻辑
    public void UpdateAllStats()
    {
        var db = PlayerBaseData.Instance;

        // --- 1. 查 PlayerData (根据等级查基础属性) ---
        // 读取 CSV 文件
        string[] pLines = Resources.Load<TextAsset>("CSV_Data/PlayerData").text.Split('\n');
        // 简单粗暴的遍历查找 (注意：第一行是标题，从 i=1 开始)
        for (int i = 1; i < pLines.Length; i++)
        {
            string[] row = pLines[i].Split(',');
            if (row.Length < 5) continue; 
            
            if (int.Parse(row[0]) == db.level) // 找到对应等级
            {
                db.nextLevelXP = int.Parse(row[1]);
                // 这里简单处理，假设基础HP就是表里的HP
                db.finalMaxHP = float.Parse(row[4]); 
                db.finalATK = float.Parse(row[2]);
                break;
            }
        }

        // --- 2. 查 WeaponData (根据武器ID查加成) ---
        string[] wLines = Resources.Load<TextAsset>("CSV_Data/WeaponData").text.Split('\n');
        for (int i = 1; i < wLines.Length; i++)
        {
            string[] row = wLines[i].Split(',');
            if (row.Length < 5) continue;

            if (int.Parse(row[0]) == db.currentWeaponID)
            {
                db.weaponATK = float.Parse(row[2]);
                db.weaponHP = float.Parse(row[4]);
                break;
            }
        }

        // --- 3. 汇总计算 ---
        db.finalATK += db.weaponATK;
        db.finalMaxHP += db.weaponHP;

        Debug.Log($"<color=green>属性更新完毕: Lv.{db.level}, ATK:{db.finalATK}</color>");

        // --- 4. 广播完成信号 ---
        GameEventManager.CallStatsChanged();
    }
}