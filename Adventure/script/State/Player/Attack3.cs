using Godot;

namespace Adventure.script.State.Player;
/// <summary>
/// 三段攻击
/// </summary>
public partial class Attack3 : PlayerState
{
    public override void Enter()
    {
        base.Enter();
        // Player.AnimationPlayer.Play(AnimationName);
        SoundManager.PlaySFX("Attack");
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
        Player.Stand(delta, Player.DefaultGravity, Player.FloorAcceleration);
    }

    public override void HandleInput(InputEvent @event)
    {
        
    }

    public override void MangeStateShift()
    {
        //动画播放完成
        if (!Player.AnimationPlayer.IsPlaying())
        {
            StateMachine.TransitionTo("Idle");
        }
    }
}