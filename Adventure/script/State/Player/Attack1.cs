using Godot;

namespace Adventure.script.State.Player;
/// <summary>
/// 一段攻击
/// </summary>
public partial class Attack1 : PlayerState
{
    public override void Enter()
    {
        base.Enter();
        // Player.AnimationPlayer.Play(AnimationName);
        SoundManager.PlaySFX("Attack");
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
        Player.Stand(delta, Player.DefaultGravity, Player.FloorAcceleration);
    }

    public override void HandleInput(InputEvent @event)
    {
        //玩家按下攻击键
        if (@event.IsActionPressed("attack") && Player.CanCombo)
        {
            Player.ComboTimer.Start();
        }
    }

    public override void MangeStateShift()
    {
        if (Player.AnimationPlayer.IsPlaying()) return;
        StateMachine.TransitionTo(Player.ComboTimer.TimeLeft > 0 ? "Attack2" : "Idle");
    }
}