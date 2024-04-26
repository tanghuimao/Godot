using Godot;

namespace Adventure.script.State.Boar;
/// <summary>
/// 空闲
/// </summary>
public partial class Idle : BoarState
{
    public override void Enter()
    {
        base.Enter();
        //如果面向墙
        if (Boar != null && Boar.WallChecker.IsColliding())
        {
            Boar.SetDirection(Boar.DefaultDirection == Direction.Left ? Direction.Right : Direction.Left);
        }
    }

    public override void Exit()
    {
    }

    public override void Update(float delta)
    {
        MangeStateShift();
        if (!Boar.AnimationPlayer.IsPlaying())
        {
            Boar.AnimationPlayer.Play(AnimationName);
        }
    }

    public override void PhysicsUpdate(float delta)
    {
        Boar.Move(delta, 0f);
    }

    public override void HandleInput(InputEvent @event)
    {
    }

    public override void MangeStateShift()
    {
        //检测到玩家
        if (Boar.PlayerChecker.IsColliding())
        {
            StateMachine.TransitionTo("Running");
            return;
        }
        //时间大于2S
        if (Boar.StateMachine.StateTime > 2D)
        {
            StateMachine.TransitionTo("Walk");
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