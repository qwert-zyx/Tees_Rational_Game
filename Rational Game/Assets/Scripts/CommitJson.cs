using UnityEngine;

public class CommitJson : MonoBehaviour
{
    void Start()
    {
        GameEventManager.OnDataNeedUpdate += SaveData;
    }
    
    void OnDestroy()
    {
        GameEventManager.OnDataNeedUpdate -= SaveData;
    }

    void SaveData()
    {
        var db = PlayerBaseData.Instance;
        SaveData save = new SaveData
        {
            level = db.level,
            currentXP = db.currentXP,
            weaponID = db.currentWeaponID
        };
        JsonIO.Save(save);
    }
}