using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace Adventure.script.State.Player;
/// <summary>
/// 滑铲开始
/// </summary>
public partial class SlidingStart : PlayerState
{
    public override void Enter()
    {
        base.Enter();
        Player.EnergyComponent.SetEnergy(Player.SlidingEnergy);
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
        //动画播放完成后 状态转换
        if (!Player.AnimationPlayer.IsPlaying())
        {
            StateMachine.TransitionTo("SlidingLoop");
        }
    }
}