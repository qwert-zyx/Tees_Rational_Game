using UnityEngine;

public class InitializatePlayer : MonoBehaviour
{
    void Start()
    {
        Debug.Log("游戏初始化...");

        // 1. 加载存档到内存
        SaveData save = JsonIO.Load();
        var db = PlayerBaseData.Instance;
        
        db.level = save.level;
        db.currentXP = save.currentXP;
        db.currentWeaponID = save.weaponID;

        // 2. 发出启动信号
        // 这会让 CheckCSV 立即去算一次属性
        GameEventManager.CallGameStart();
    }
}