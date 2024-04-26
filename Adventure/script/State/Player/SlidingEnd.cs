using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace Adventure.script.State.Player;
/// <summary>
/// 滑铲结束
/// </summary>
public partial class SlidingEnd : PlayerState
{
    public override void Enter()
    {
        base.Enter();
        // Player.AnimationPlayer.Play(AnimationName);
    }

    public override void Exit()
    {
        //设置滑铲状态为false
        Player.SlideRequestTimer.Stop();
    }

    public override void Update(float delta)
    {
        MangeStateShift();
    }

    public override void PhysicsUpdate(float delta)
    {
        //执行玩家站立
        // Player.Stand(delta, Player.DefaultGravity, Player.FloorAcceleration);
    }

    public override void HandleInput(InputEvent @event)
    {
        
    }

    public override void MangeStateShift()
    {
         //动画播放完成后 状态转换
         if (!Player.AnimationPlayer.IsPlaying())
         {
             StateMachine.TransitionTo("Idle");
         }
    }
}