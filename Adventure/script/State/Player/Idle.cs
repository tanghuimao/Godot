using Godot;

namespace Adventure.script.State.Player;

/// <summary>
/// 空闲状态
/// </summary>
public partial class Idle : PlayerState
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
        if (!Player.AnimationPlayer.IsPlaying())
        {
            Player.AnimationPlayer.Play(AnimationName);
        }
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
        if (Player.IsJump)
        {
            StateMachine.TransitionTo("Jump");
            return;
        }
        if (Player.IsMove && Player.IsOnFloor())
        {
            StateMachine.TransitionTo("Running");
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