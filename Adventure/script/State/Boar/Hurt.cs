using Godot;

namespace Adventure.script.State.Boar;

public partial class Hurt : BoarState
{
    public override void Enter()
    {
        base.Enter();
        
        //获得攻击者方向
        var direction = Boar.HitBox.GlobalPosition.DirectionTo(Boar.GlobalPosition);
        //击退距离
        Boar.Velocity = direction * Boar.KnockBackAmount;
        //受到伤害转身
        Boar.SetDirection(direction.X > 0 ? Direction.Left : Direction.Right);
    }

    public override void Exit()
    {
        Boar.IsHurt = false;
        Boar.HitBox = null;
    }

    public override void Update(float delta)
    {
        MangeStateShift();
    }

    public override void PhysicsUpdate(float delta)
    {
        // Boar.Move(delta, 0f);
    }

    

    public override void MangeStateShift()
    {
        
        //动画播放完毕 
        if (!Boar.AnimationPlayer.IsPlaying())
        {
            StateMachine.TransitionTo("Running");
            return;
        }
        //受伤
        if (Boar.IsHurt)
        {
            StateMachine.TransitionTo("Hurt");
            return;
        }
        //死亡
        if (Boar.IsDie)
        {
            StateMachine.TransitionTo("Die");
        }
    }
}