using UnityEngine;
using UnityEngine.UI; // 【新增】引用 UI
using UnityEngine.SceneManagement;

public class TowerUIController : MonoBehaviour
{
    [Header("请把按钮拖进来")]
    public Button btnNextLevel;   // 对应 UI_Win 里的“下一关”
    public Button btnBackToMenu;  // 对应 UI_Win 和 UI_Fail 里的“返回”

    // 如果你有两个返回按钮（比如胜利界面一个，失败界面一个）
    // 你可以把它们都拖给同一个变量（虽然不推荐），或者再加一个变量：
    public Button btnBackToMenuFail; // 失败界面的返回按钮

    void Start()
    {
        // 1. 自动绑定事件 (防止你忘了在编辑器里配 OnClick)
        if (btnNextLevel != null)
            btnNextLevel.onClick.AddListener(OnClickNextLevel);

        if (btnBackToMenu != null)
            btnBackToMenu.onClick.AddListener(OnClickBackToMenu);

        if (btnBackToMenuFail != null)
            btnBackToMenuFail.onClick.AddListener(OnClickBackToMenu);
    }

    // --- 按钮逻辑 ---

    void OnClickNextLevel()
    {
        // 目标层数 + 1
        if (PlayerBaseData.Instance != null)
        {
            PlayerBaseData.Instance.targetLevelID++;
            Debug.Log($"【系统】前往第 {PlayerBaseData.Instance.targetLevelID} 层");
        }

        // 重新加载当前场景，刷新数据
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);

        // 恢复时间
        Time.timeScale = 1;
    }

    void OnClickBackToMenu()
    {
        // 恢复时间
        Time.timeScale = 1;

        // 返回主菜单
        SceneManager.LoadScene("UI_MainMenu");
    }
}