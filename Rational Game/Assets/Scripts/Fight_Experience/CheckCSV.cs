using UnityEngine;
using UnityEngine.SceneManagement; // 【必须引用】用于识别场景名称

public class CheckCSV : MonoBehaviour
{
    void Awake()
    {
        // 这里的 Awake 只负责监听，不读数据，防止报错
        GameEventManager.OnGameStart += UpdateAllStats;
        GameEventManager.OnDataNeedUpdate += UpdateAllStats;
    }

    void Start() // ✅ 安全入口
    {
        // 强制执行一次，确保开局数据载入
        UpdateAllStats();
    }

    void OnDestroy()
    {
        GameEventManager.OnGameStart -= UpdateAllStats;
        GameEventManager.OnDataNeedUpdate -= UpdateAllStats;
    }

    // ==========================================
    // 核心查表逻辑
    // ==========================================
    public void UpdateAllStats()
    {
        // 0. 防空检查
        if (PlayerBaseData.Instance == null)
        {
            Debug.LogError("严重错误：PlayerBaseData 缺失！");
            return;
        }

        var db = PlayerBaseData.Instance;

        // ==========================================
        // 【关键逻辑】根据场景名称自动切换模式
        // ==========================================
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Fight_Jail")
        {
            db.isTowerMode = true;
            Debug.Log("【环境检测】当前是爬塔场景 (Fight_Jail)，将读取塔数据。");
        }
        else if (sceneName == "Fight_Experience")
        {
            db.isTowerMode = false;
            Debug.Log("【环境检测】当前是刷经验场景 (Fight_Experience)，将读取挂机数据。");
        }
        // 如果在主菜单或其他场景，保持原状或默认处理

        // ==========================================
        // 第一步：查 PlayerData (始终根据玩家等级)
        // ==========================================
        TextAsset pData = Resources.Load<TextAsset>("CSV_Data/PlayerData");
        if (pData != null)
        {
            string[] pLines = pData.text.Split('\n');
            for (int i = 1; i < pLines.Length; i++)
            {
                string[] row = pLines[i].Split(',');
                if (row.Length < 5) continue;

                if (int.Parse(row[0]) == db.level)
                {
                    db.nextLevelXP = int.Parse(row[1]);
                    db.finalATK = float.Parse(row[2]);
                    // row[3] 可能是防御，你代码里跳过了
                    db.finalMaxHP = float.Parse(row[4]);
                    break;
                }
            }
        }

        // ==========================================
        // 第二步：查 WeaponData (根据武器ID)
        // ==========================================
        TextAsset wData = Resources.Load<TextAsset>("CSV_Data/WeaponData");
        if (wData != null)
        {
            string[] wLines = wData.text.Split('\n');
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
        }

        // ==========================================
        // 第三步：查 EnemyData (【核心修改】动态文件名与索引)
        // ==========================================

        // A. 决定读哪个文件
        string enemyFileName = db.isTowerMode ? "TowerEnemyData" : "EnemyData";

        // B. 决定查哪一行 (爬塔查 targetLevelID，挂机查 playerLevel)
        int searchTargetID = db.isTowerMode ? db.targetLevelID : db.level;

        TextAsset eData = Resources.Load<TextAsset>("CSV_Data/" + enemyFileName);

        if (eData != null)
        {
            string[] eLines = eData.text.Split('\n');
            bool foundEnemy = false;

            for (int i = 1; i < eLines.Length; i++)
            {
                string[] row = eLines[i].Split(',');
                if (row.Length < 6) continue;

                // 第一列通常都是 Level (不管是玩家等级还是塔层数)
                int csvLevel = int.Parse(row[0]);

                if (csvLevel == searchTargetID)
                {
                    db.enemyTemplateATK = float.Parse(row[1]);
                    // row[2] 是防御 E_DEF
                    db.enemyTemplateHP = float.Parse(row[3]);
                    db.enemyTemplateXP = int.Parse(row[4]);
                    db.enemyTemplateGroupID = int.Parse(row[5]);

                    foundEnemy = true;
                    Debug.Log($"<color=cyan>怪物数据已加载 -> 文件:{enemyFileName} | 层级:{searchTargetID} | 攻:{db.enemyTemplateATK} | 血:{db.enemyTemplateHP}</color>");
                    break;
                }
            }

            if (!foundEnemy)
            {
                Debug.LogWarning($"在表 {enemyFileName} 中未找到 ID 为 {searchTargetID} 的怪物数据！");
            }
        }
        else
        {
            Debug.LogError($"找不到CSV文件: CSV_Data/{enemyFileName}");
        }

        // ==========================================
        // 第四步：汇总计算
        // ==========================================
        db.finalATK += db.weaponATK;
        db.finalMaxHP += db.weaponHP;

        // 回血补丁
        if (db.currentHP <= 0 || db.currentHP > db.finalMaxHP)
        {
            db.currentHP = db.finalMaxHP;
        }

        // ==========================================
        // 第五步：通知刷新
        // ==========================================
        GameEventManager.CallStatsChanged();
    }
}