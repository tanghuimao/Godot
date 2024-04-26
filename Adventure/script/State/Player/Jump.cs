using Godot;

namespace Adventure.script.State.Player;

/// <summary>
/// 跳跃状态
/// </summary>
public partial class Jump : PlayerState
{
    //当前跳跃次数
    private short _currentJumpCount;
    //可跳跃次数
    [Export]private short _canJumpCount;
    //跳跃状态进入时
    public override void Enter()
    {
        _currentJumpCount = 1;
        //开启第一帧
        Player.IsFirstTick = true;
        
        base.Enter();
        // Player.AnimationPlayer.Play(AnimationName);
        SoundManager.PlaySFX("Jump");
    }

    public override void Exit()
    {
        Player.IsJump = false;
        Player.IsWallJump = false;
        _currentJumpCount = 0;
    }

    public override void Update(float delta)
    {
        MangeStateShift();
    }

    public override void PhysicsUpdate(float delta)
    {
        base.PhysicsUpdate(delta);
        //第一帧不收重力加速度影响
        var gravity = Player.IsFirstTick ? 0.0f : Player.DefaultGravity; 
        //执行玩家移动
        Player.Move(delta, gravity, Player.AirAcceleration);
        if (Player.IsJump || (Player.JumpRequestTimer.TimeLeft > 0 && _currentJumpCount <= _canJumpCount))
        {
            //设置二段跳高度
            var jumpVelocity = _currentJumpCount == _canJumpCount ? Player.JumpVelocity / 1.5f : Player.JumpVelocity;
            //改变玩家速度
            Player.Velocity = new Vector2(Player.Velocity.X, jumpVelocity);
            //停止定时器
            Player.CoyoteTimer.Stop();
            Player.JumpRequestTimer.Stop();
            //变更跳跃状态
            Player.IsJump = false;
            //添加跳跃次数
            _currentJumpCount++;
        };
        //设置不是第一帧
        Player.IsFirstTick = false;
    }

    public override void HandleInput(InputEvent @event)
    {
        //按下 启动跳跃计时器
        if (@event.IsActionPressed("jump"))
        {
            Player.JumpRequestTimer.Start();
        }
        //松开  玩家跳跃健之前跳跃高度小于设置高度一半 重新设置
        if (@event.IsActionReleased("jump") && Player.Velocity.Y < Player.JumpVelocity / 2)
        {
            Player.Velocity = new Vector2(Player.Velocity.X, Player.JumpVelocity / 2);
        }
    }

    public override void MangeStateShift()
    {
        //当前Y方向上速度大于0 状态变更为下落状态
        if (Player.Velocity.Y >= 0)
        {
            StateMachine.TransitionTo("Fall");
        }
    }
}