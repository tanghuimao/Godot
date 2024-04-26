using Godot;

namespace Adventure.script.State.Player;
/// <summary>
/// 受伤
/// </summary>
public partial class Hurt : PlayerState
{
    public override void Enter()
    {
        base.Enter();
        // Player.AnimationPlayer.Play(AnimationName);
        //获得攻击者方向
        var direction = Player.HitBoxed.GlobalPosition.DirectionTo(Player.GlobalPosition);
        //击退距离
        Player.Velocity = direction * Player.KnockBackAmount;
        //开启无敌时间
        Player.InvincibleTimer.Start();
    }

    public override void Exit()
    {
        Player.IsHurt = false;
        Player.HitBox = null;
    }

    public override void Update(float delta)
    {
        MangeStateShift();
    }

    public override void PhysicsUpdate(float delta)
    {
        base.PhysicsUpdate(delta);
    }

    public override void HandleInput(InputEvent @event)
    {
        
    }

    public override void MangeStateShift()
    {
        if (!Player.AnimationPlayer.IsPlaying())
        {
            StateMachine.TransitionTo("Idle");
        }
    }
}