using UnityEngine;

public class TestEnemy2D : MonoBehaviour
{
    // 模拟怪物的属性
    public int myXP = 150;
    public int myLevel = 1;
    public int myGroupID = 1001;

    // 这个函数是 Unity 自带的：当鼠标点击这个物体时触发
    // 【注意】为了让这个函数生效，你的物体必须挂载 "BoxCollider2D" 组件！
    void OnMouseDown()
    {
        Debug.Log("【2D测试】点击了方形怪物，模拟死亡！");

        // 发送广播：告诉全游戏“我死了”，并交出我的经验和掉落组ID
        GameEventManager.CallEnemyDead(myXP, myLevel, myGroupID);

        // 销毁自己（从屏幕上消失）
        Destroy(gameObject);
    }
}