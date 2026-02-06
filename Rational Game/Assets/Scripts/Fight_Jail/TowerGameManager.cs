using UnityEngine;
using TMPro; // 如果你用 TextMeshPro，没用就注释掉或者换成 UnityEngine.UI

public class TowerGameManager : MonoBehaviour
{
    [Header("挑战规则")]
    public float timeLimit = 60f; // 限时60秒
    public int targetKillCount = 3; // 必须杀够3只

    [Header("UI 面板绑定")]
    public GameObject panelWin;   // 拖入你的 UI_Win
    public GameObject panelFail;  // 拖入你的 UI_Fail
    public TMP_Text timerText;    // 拖入显示倒计时的文本 (可选)

    // 运行时数据
    private float currentTimer;
    private int currentKillCount = 0;
    private bool isGameEnded = false;

    void Start()
    {
        // 1. 初始化数据
        currentTimer = timeLimit;
        currentKillCount = 0;
        isGameEnded = false;

        // 2. 隐藏结算界面
        if (panelWin != null) panelWin.SetActive(false);
        if (panelFail != null) panelFail.SetActive(false);

        // 3. 监听事件
        // 注意：这里订阅的是那个带3个参数的事件
        GameEventManager.OnEnemyDead += HandleEnemyDead;

        // 监听关卡失败（由倒计时触发，或者玩家死的时候触发）
        GameEventManager.OnLevelFailed += ShowFailUI;
        // 监听关卡胜利
        GameEventManager.OnLevelVictory += ShowWinUI;
    }

    void OnDestroy()
    {
        // 记得取消订阅，防止报错
        GameEventManager.OnEnemyDead -= HandleEnemyDead;
        GameEventManager.OnLevelFailed -= ShowFailUI;
        GameEventManager.OnLevelVictory -= ShowWinUI;
    }

    void Update()
    {
        if (isGameEnded) return;

        // --- 倒计时逻辑 ---
        currentTimer -= Time.deltaTime;

        // 更新 UI (显示整数)
        if (timerText != null)
            timerText.text = $"{Mathf.Ceil(currentTimer)}s";

        // --- 超时判负 ---
        if (currentTimer <= 0)
        {
            Debug.Log("【裁判】时间耗尽！判定失败。");
            GameEventManager.CallLevelFailed(); // 广播失败信号
        }



        // ==========================================
        // 【新增测试代码】按下键盘 "L" (Lose) 强制失败
        // ==========================================
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("【强制测试】手动触发失败流程");
            GameEventManager.CallLevelFailed();
        }



    }

    // ==========================================
    // 事件回调区
    // ==========================================

    // 重点：这里的参数必须和 EventManager 里定义的 Action<int, int, int> 一致
    // 即使我们在爬塔裁判里不需要 xp，也得把参数位置留着
    void HandleEnemyDead(int xp, int level, int groupID)
    {
        if (isGameEnded) return;

        currentKillCount++;
        Debug.Log($"【裁判】检测到击杀！当前进度: {currentKillCount} / {targetKillCount}");

        if (currentKillCount >= targetKillCount)
        {
            Debug.Log("【裁判】击杀目标达成！判定胜利。");
            GameEventManager.CallLevelVictory(); // 广播胜利信号
        }
    }

    void ShowWinUI()
    {
        if (isGameEnded) return; // 防止重复触发
        isGameEnded = true;

        // 可以在这里暂停游戏
        // Time.timeScale = 0; 

        if (panelWin != null) panelWin.SetActive(true);
    }

    void ShowFailUI()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        // Time.timeScale = 0;

        if (panelFail != null) panelFail.SetActive(true);
    }
}