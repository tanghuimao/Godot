using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace Adventure.script.State.Player;
/// <summary>
/// 滑铲中
/// </summary>
public partial class SlidingLoop : PlayerState
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
        //执行玩家滑铲
        Player.Sliding(delta);
    }

    public override void HandleInput(InputEvent @event)
    {
        
    }

    public override void MangeStateShift()
    {
        if (Player.StateMachine.StateTime >= Player.SlidingTime)
        {
            StateMachine.TransitionTo("SlidingEnd");
        }
    }
}