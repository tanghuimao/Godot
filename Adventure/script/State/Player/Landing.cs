using Godot;

namespace Adventure.script.State.Player;

/// <summary>
/// 着陆状态
/// </summary>
public partial class Landing : PlayerState
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
        //执行玩家移动
        Player.Stand(delta, Player.DefaultGravity, Player.FloorAcceleration);
    }

    public override void HandleInput(InputEvent @event)
    {
        
    }

    public override void MangeStateShift()
    {
        //动画播放完毕  状态转换
        if (!Player.AnimationPlayer.IsPlaying())
        {
            StateMachine.TransitionTo(Player.IsMove ? "Running" : "Idle");
        }
    }
}