using System.IO;
using UnityEngine;

// 定义要存入硬盘的数据结构
[System.Serializable]
public class SaveData
{
    public int level;
    public int currentXP;
    public int weaponID;
}

public static class JsonIO
{
    // 存档路径
    private static string Path => Application.persistentDataPath + "/SaveData.json";

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Path, json);
        Debug.Log("存档成功: " + Path);
    }

    public static SaveData Load()
    {
        if (!File.Exists(Path))
        {
            // 如果没有存档，返回默认新号数据
            return new SaveData { level = 1, currentXP = 0, weaponID = 110001 };
        }
        string json = File.ReadAllText(Path);
        return JsonUtility.FromJson<SaveData>(json);
    }
}