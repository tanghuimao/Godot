using Godot;

namespace Adventure.script.State.Player;
/// <summary>
/// 蹬强跳
/// </summary>
public partial class WallJump : PlayerState
{
    public override void Enter()
    {
        base.Enter();
        // Player.AnimationPlayer.Play(AnimationName);
        //设置时间流速
        // Engine.TimeScale = 0.3;
        Player.IsFirstTick = true;
        SoundManager.PlaySFX("Jump");
    }

    public override void Exit()
    {
        // Engine.TimeScale = 1.0;
    }

    public override void Update(float delta)
    {
        MangeStateShift();
    }

    public override void PhysicsUpdate(float delta)
    {   
        base.PhysicsUpdate(delta);
        //玩家状态时间小于 0.1S
        if (Player.StateMachine.StateTime < 0.2D)
        {
            //执行玩家站立
            Player.Stand(delta, Player.IsFirstTick ? 0f : Player.DefaultGravity, Player.FloorAcceleration);
            //设置玩家方向
            Player.Graphics.Scale = new Vector2(Player.GetWallNormal().X, 1);
        }
        else
        {
            //执行玩家移动 
            Player.Move(delta, Player.DefaultGravity, Player.FloorAcceleration);
        }
        if (Player.IsWallJump)
        {
            var velocity = Player.WallJumpVelocity;
            //设置跳跃方向
            velocity.X *= Player.GetWallNormal().X;
            //设置蹬墙跳
            Player.Velocity = velocity;
            //设置状态
            Player.IsWallJump = false;
        }

        Player.IsFirstTick = false;
    }

    public override void HandleInput(InputEvent @event)
    {
        //玩家在地面并且按下跳跃键才能跳跃 
        if (@event.IsActionPressed("jump") && Player.IsOnWall())
        {
            Player.IsJump = true;
        }
    }

    public override void MangeStateShift()
    {
        //玩家进入滑墙 并且 不是第一帧
        if (Player.CanWallSliding() && !Player.IsFirstTick)
        {
            StateMachine.TransitionTo("WallSliding");
            return;
        }
        //当前Y方向上速度大于0 状态变更为下落状态
        if (Player.Velocity.Y >= 0)
        {
            StateMachine.TransitionTo("Fall");
        }
    }
}