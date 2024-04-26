using Godot;

namespace Adventure.script.State.Boar;
/// <summary>
/// 行走
/// </summary>
public partial class Walk : BoarState
{
    public override void Enter()
    {
        base.Enter();
        //如果没有在地面 切换朝向
        if (!Boar.FloorChecker.IsColliding())
        {
            Boar.SetDirection(Boar.DefaultDirection == Direction.Left ? Direction.Right : Direction.Left);
        }
        //强制更新  （godot会缓存检查结果 所以需要强制更新）
        Boar.FloorChecker.ForceRaycastUpdate();
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
        Boar.Move(delta, Boar.MaxSpeed / 3f);
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
        //检测到墙 或者 没有地面
        if (Boar.WallChecker.IsColliding() || !Boar.FloorChecker.IsColliding()) {
            StateMachine.TransitionTo("Idle");
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