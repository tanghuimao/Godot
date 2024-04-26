using Godot;

namespace Adventure.script.State.Player;

/// <summary>
/// 跑步状态
/// </summary>
public partial class Running : PlayerState
{
    public override void Enter()
    {
        base.Enter();
        // Player.AnimationPlayer.Play(AnimationName);
    }

    public override void Exit()
    {
        Player.ComboTimer.Stop();
    }

    public override void Update(float delta)
    {
        MangeStateShift();
    }

    public override void PhysicsUpdate(float delta)
    {
        base.PhysicsUpdate(delta);
        //执行玩家移动
        Player.Move(delta, Player.DefaultGravity, Player.FloorAcceleration);
    }

    public override void HandleInput(InputEvent @event)
    {
        //玩家在地面并且按下跳跃键才能跳跃 
        if (@event.IsActionPressed("jump") && Player.IsOnFloor())
        {
            Player.IsJump = true;
        }
        //玩家按下攻击键
        if (@event.IsActionPressed("attack"))
        {
            Player.ComboTimer.Start();
        }
        //玩家按下滑铲
        if (@event.IsActionPressed("slide"))
        {
            Player.SlideRequestTimer.Start();
        }
    }

    public override void MangeStateShift()
    {
        //不在地面上
        if (!Player.IsOnFloor())
        {
            Player.CoyoteTimer.Start();
            StateMachine.TransitionTo("Fall");
            return;
        }
        //跳跃
        if (Player.IsJump)
        {
            StateMachine.TransitionTo("Jump");
            return;
        }
        //没有移动状态转换为空闲
        if (!Player.IsMove)
        {
            StateMachine.TransitionTo("Idle");
            return;
        }
        if (Player.ComboTimer.TimeLeft > 0)
        {
            StateMachine.TransitionTo("Attack1");
            return;
        }
        //受伤
        if (Player.IsHurt)
        {
            StateMachine.TransitionTo("Hurt");
            return;
        }
        //死亡
        if (Player.IsDie)
        {
            StateMachine.TransitionTo("Die");
            return;
        }
        //滑铲
        if (Player.CanSliding())
        {
            StateMachine.TransitionTo("SlidingStart");
        }
    }
}