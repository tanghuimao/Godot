using Godot;

namespace Adventure.script.State.Boar;

public partial class Die : BoarState
{
    public override void Enter()
    {
        base.Enter();
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
        // Boar.Move(delta, 0);
    }

    public override void HandleInput(InputEvent @event) 
    {
    }

    public override void MangeStateShift()
    {
        //动画播放完成
        if (!Boar.AnimationPlayer.IsPlaying())
        {
            //释放
            Boar.QueueFree();
        }
    }
}