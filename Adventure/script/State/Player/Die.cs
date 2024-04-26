using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace Adventure.script.State.Player;
/// <summary>
/// 死亡
/// </summary>
public partial class Die : PlayerState
{
    public override void Enter()
    {
        base.Enter();
        Player.Interactables.Clear();
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
        //执行玩家站立
        // Player.Stand(delta, Player.DefaultGravity, Player.FloorAcceleration);
    }

    public override void HandleInput(InputEvent @event)
    {
        
    }

    public override void MangeStateShift()
    {
         //动画播放完成后
         // if (!Player.AnimationPlayer.IsPlaying())
         // {
         //     //延迟1s执行
         //     Thread.Sleep(1000);
         //     //调用死亡方法
         //     Player.Died();
         // }
    }
}