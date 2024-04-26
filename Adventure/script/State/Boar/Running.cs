using Godot;

namespace Adventure.script.State.Boar;
/// <summary>
/// 奔跑
/// </summary>
public partial class Running : BoarState
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        Boar.CalmDownTimer.Stop();
    }

    public override void Update(float delta)
    {
        MangeStateShift();
    }

    public override void PhysicsUpdate(float delta)
    {
        //碰到墙和悬崖立即转身
        if (Boar.WallChecker.IsColliding() || !Boar.FloorChecker.IsColliding())
        {
            Boar.SetDirection(Boar.DefaultDirection == Direction.Left ? Direction.Right : Direction.Left);
        }
        Boar.Move(delta, Boar.MaxSpeed);
        //能够看见玩家
        if (Boar.PlayerChecker.IsColliding())
        {
            //启动计时器
            Boar.CalmDownTimer.Start();
        }
    }

    public override void HandleInput(InputEvent @event)
    {
    }

    public override void MangeStateShift()
    {
        if (Boar.CalmDownTimer.IsStopped())
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