using UnityEngine;

public class DeathSettlement : MonoBehaviour
{
    void Start()
    {
        // 只需要监听怪物死亡
        GameEventManager.OnEnemyDead += HandleEnemyDeath;
    }

    void OnDestroy()
    {
        GameEventManager.OnEnemyDead -= HandleEnemyDeath;
    }

    // 当收到怪物死亡广播时
    void HandleEnemyDeath(int xp, int level, int groupID)
    {
        Debug.Log($"收到怪物死亡: XP+{xp}, GroupID:{groupID}");
        
        // === 1. 经验结算 ===
        var db = PlayerBaseData.Instance;
        db.currentXP += xp;

        // 升级判定 (用 while 防止连升两级)
        while (db.currentXP >= db.nextLevelXP)
        {
            db.currentXP -= db.nextLevelXP;
            db.level++;
            Debug.Log("<color=yellow>升级了！</color>");
        }

        // === 2. 掉落结算 (Weapon Settlement) ===
        CheckDrop(groupID);

        // === 3. 统一通知更新 ===
        // 这会触发 CheckCSV 重算属性，也会触发 CommitJson 保存
        GameEventManager.CallDataNeedUpdate();
    }

    void CheckDrop(int groupID)
    {
        // 读取掉落组表
        string[] lines = Resources.Load<TextAsset>("CSV_Data/DropGroup").text.Split('\n');
        
        foreach (var line in lines)
        {
            string[] row = line.Split(','); // 注意这里先用逗号分大块
            if (row.Length < 2 || row[0] == "GroupID") continue;

            if (int.Parse(row[0]) == groupID)
            {
                // 解析后面那一大串：110001|1|80;110002|1|20
                string allDrops = row[1]; 
                string[] dropItems = allDrops.Split(';'); // 用分号拆分每个物品

                foreach (var item in dropItems)
                {
                    string[] details = item.Split('|'); // 用竖线拆分细节
                    if (details.Length < 3) continue;

                    int weaponID = int.Parse(details[0]);
                    int probability = int.Parse(details[2]); // 简单概率

                    // 摇骰子 (0-100)
                    if (Random.Range(0, 100) < probability)
                    {
                        Debug.Log($"<color=cyan>掉落判定成功！获得武器: {weaponID}</color>");
                        CompareAndEquip(weaponID);
                        return; // 这里的逻辑是掉了一个就不掉了，防止一次掉一堆
                    }
                }
            }
        }
    }

    // 简单的比对换装逻辑
    void CompareAndEquip(int newWeaponID)
    {
        // 这里应该再去查 WeaponData 获取新武器攻击力，为了代码简洁，
        // 我这里直接简单粗暴判定：ID越大越强 (你可以后续完善去查表)
        if (newWeaponID > PlayerBaseData.Instance.currentWeaponID)
        {
            Debug.Log("新武器更好！自动装备。");
            PlayerBaseData.Instance.currentWeaponID = newWeaponID;
            // 广播换装信号
            GameEventManager.CallWeaponSwapped(newWeaponID);
        }
    }
}