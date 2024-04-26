using Godot;

namespace Adventure.script.State.Player;

/// <summary>
/// 落下状态
/// </summary>
public partial class Fall : PlayerState
{
    public override void Enter()
    {
        base.Enter();
        //记录玩家下落时高度
        Player.FallingHeight = Player.GlobalPosition.Y;
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
        //执行玩家移动
        Player.Move(delta, Player.DefaultGravity, Player.AirAcceleration);
    }

    public override void HandleInput(InputEvent @event)
    {
        //玩家下落过程中按下跳跃 并且 CoyoteTimer超时时间大于0
        if (@event.IsActionPressed("jump") && Player.CoyoteTimer.TimeLeft > 0)
        {
            Player.IsJump = true;
        }
        //玩家按下滑铲
        if (@event.IsActionPressed("slide"))
        {
            Player.SlideRequestTimer.Start();
        }
    }

    public override void MangeStateShift()
    {
        //跳跃
        if (Player.IsJump)
        {
            StateMachine.TransitionTo("Jump");
            return;
        }
        //玩家在地面时
        if (Player.IsOnFloor())
        {
            //计算下落高度
            Player.FallingHeight = Player.GlobalPosition.Y - Player.FallingHeight;
            //否则是着陆状态
            StateMachine.TransitionTo(Player.FallingHeight >= Player.LandingHeight ? "Landing" : "Running");
            return;
        }
        //玩家贴墙 并且 手脚检测点都贴墙
        if (Player.CanWallSliding())
        {
            StateMachine.TransitionTo("WallSliding");
            return;
        }
        
        //滑铲
        if (Player.CanSliding())
        {
            StateMachine.TransitionTo("SlidingStart");
        }
    }
}