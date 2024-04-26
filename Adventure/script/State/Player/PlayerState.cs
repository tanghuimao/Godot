using System;
using Godot;

namespace Adventure.script.State.Player;

/// <summary>
/// 玩家状态 父类
/// </summary>
public partial class PlayerState : State
{
    public script.Player Player;
    //动画名称
    [Export] protected String AnimationName;
    
    public override void Enter()
    {
        Player?.AnimationPlayer.Play(AnimationName);
    }
    
    public override void PhysicsUpdate(float delta)
    {
        //检测玩家交换图标是否显示
        if (Player.Interactables.Count > 0 && Player.HealthComponent.GetCurrentHealth() > 0)
        {
            Player.InteractionIcon.Visible = true;
        }
        else
        {
            //设置交换图标不显示
            Player.InteractionIcon.Visible = false; 
        }
        //设置玩家无敌闪烁
        if (Player.InvincibleTimer.TimeLeft > 0)
        {   
            var modulate = Player.Graphics.Modulate;
            // Mathf.Sin(Time.GetTicksMsec() / 20F) * 0.5F + 0.5F    -1  1
            modulate.A = Mathf.Sin(Time.GetTicksMsec() / 20F) * 0.5F + 0.5F;
            Player.Graphics.Modulate = modulate;
        }
        else
        {
            var modulate = Player.Graphics.Modulate;
            modulate.A = 1f;
            Player.Graphics.Modulate = modulate;
        }
    }
    
}