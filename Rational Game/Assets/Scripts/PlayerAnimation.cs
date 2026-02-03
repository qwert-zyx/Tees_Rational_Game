using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    // 有些素材朝向可能和你的游戏相反，用这个变量控制翻转
    private SpriteRenderer sr;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        // 监听核心信号
        GameEventManager.OnPlayerStateChanged += HandleStateChange;

        // 如果你之前还写了换武器的广播，这里也可以监听，用来切换持剑/持斧动画
        // GameEventManager.OnWeaponSwapped += HandleWeaponSwap;
    }

    void OnDestroy()
    {
        GameEventManager.OnPlayerStateChanged -= HandleStateChange;
    }

    void HandleStateChange(PlayerState state)
    {
        if (anim == null) return;

        // 根据状态切换动画参数
        if (state == PlayerState.Moving)
        {
            anim.SetBool("IsMoving", true);
        }
        else if (state == PlayerState.Attacking)
        {
            // 停止跑步
            anim.SetBool("IsMoving", false);
            // 触发攻击（Trigger触发一次就行）
            anim.SetTrigger("Attack");
        }
    }
}