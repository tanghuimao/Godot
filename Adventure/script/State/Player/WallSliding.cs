using Godot;

namespace Adventure.script.State.Player;

/// <summary>
/// 滑墙状态
/// </summary>
public partial class WallSliding : PlayerState
{
    public override void Enter()
    {
        base.Enter();
        // Player.AnimationPlayer.Play(AnimationName);
    }

    public override void Exit()
    {
        
    }

    public override void Update(float delta)
    {
        MangeStateShift();
    }

    public override void PhysicsUpdate(float delta)
    {
        base.PhysicsUpdate(delta);
        //执行玩家移动 滑墙状态下重力是平时 1/4
        Player.Move(delta, Player.DefaultGravity / 4, Player.FloorAcceleration);
        //设置扶墙翻转玩家 GetWallNormal
        Player.Graphics.Scale = new Vector2(Player.GetWallNormal().X , 1);
    }

    public override void HandleInput(InputEvent @event)
    {
        //玩家在墙上并且按下跳跃键才能蹬墙跳
        if (@event.IsActionPressed("jump") && Player.IsOnWall())
        {
            Player.IsWallJump = true;
        }
    }

    public override void MangeStateShift()
    {
        //跳跃
        if (Player.IsWallJump)
        {
            StateMachine.TransitionTo("WallJump");
            return;
        }
        //落到地面
        if (Player.IsOnFloor())
        {
            StateMachine.TransitionTo("Idle");
            return;
        }
        //主动离开墙面
        if (!Player.IsOnWall())
        {
            StateMachine.TransitionTo("Fall");
        }
    }
}