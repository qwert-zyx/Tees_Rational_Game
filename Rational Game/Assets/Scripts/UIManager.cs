using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // 必须引用：用于切换场景

public class UIManager : MonoBehaviour
{
    [Header("UI 组件绑定")]
    public Slider hpSlider;
    public Slider xpSlider;
    public TMP_Text infoText;

    [Header("游戏结束面板")]
    public GameObject gameOverPanel;
    public Button backMenuButton;    // 改名：返回菜单按钮

    [Header("场景设置")]
    public string mainMenuSceneName = "MainMenu"; // 你的主菜单场景名字

    void Start()
    {
        GameEventManager.OnStatsChanged += RefreshUI;
        GameEventManager.OnDataNeedUpdate += RefreshUI;

        // 绑定按钮事件：点击由于我们要返回主菜单
        backMenuButton.onClick.AddListener(ReturnToMenu);

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    void OnDestroy()
    {
        GameEventManager.OnStatsChanged -= RefreshUI;
        GameEventManager.OnDataNeedUpdate -= RefreshUI;
    }

    void RefreshUI()
    {
        var db = PlayerBaseData.Instance;

        // 1. 刷新文本
        infoText.text = $"Lv.{db.level} ATK:{db.finalATK}";

        // 2. 刷新血条
        if (db.finalMaxHP > 0)
        {
            hpSlider.value = db.currentHP / db.finalMaxHP;
        }

        // 3. 刷新经验条
        if (db.nextLevelXP > 0)
        {
            xpSlider.value = (float)db.currentXP / db.nextLevelXP;
        }

        // 4. 核心：检测是否死亡
        if (db.currentHP <= 0)
        {
            ShowGameOver();
        }
    }

    void ShowGameOver()
    {
        if (gameOverPanel != null && !gameOverPanel.activeSelf)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0; // 暂停游戏，防止怪物继续移动
            Debug.Log("显示死亡界面，等待玩家返回主菜单");
        }
    }

    // 【修改核心】：返回主菜单逻辑
    void ReturnToMenu()
    {
        // 1. 恢复时间 (如果不恢复，进了主菜单也是暂停的)
        Time.timeScale = 1;

        // 2. 处理单例数据 (非常重要！)
        // 因为 PlayerBaseData 是 DontDestroyOnLoad 的，如果不处理，
        // 你回到菜单再重新进游戏，会有两个 PlayerBaseData，或者数据没重置。
        // 建议：在这里把当前的 PlayerBaseData 销毁，让新游戏重新生成；
        // 或者手动重置它的数据。

        // 方案A：简单粗暴，销毁单例，下次进游戏重新 Init
        if (PlayerBaseData.Instance != null)
        {
            Destroy(PlayerBaseData.Instance.gameObject);
        }

        // 3. 切换场景
        SceneManager.LoadScene(mainMenuSceneName);
    }
}