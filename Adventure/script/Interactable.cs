using Godot;

namespace Adventure.script;

/// <summary>
/// 交互器
/// </summary>
[GlobalClass]
[GodotClassName("Interactable")]
public partial class Interactable : Area2D
{
    //自定义信号 交互
    [Signal]
    public delegate void InteractedEventHandler();

    public override void _Ready()
    {
        //碰撞层
        CollisionLayer = 0;
        //碰撞掩码
        CollisionMask = 0;
        //设置碰撞层2
        SetCollisionMaskValue(2, true);
        
        //进入信号处理
        BodyEntered += OnBodyEntered;
        //退出信号处理
        BodyExited += OnBodyExited;
    }
    //交互
    public virtual void Interact()
    {
        // GD.Print("Interact : ", Name);
        //触发交互信号
        EmitSignal(SignalName.Interacted);
    }

    //进入信号处理
    private void OnBodyEntered(Node2D body)
    {
        //设置玩家交换对象
        if (body is Player player) player.Interactables.Add(this);
    }
    //退出信号处理
    private void OnBodyExited(Node2D body)
    {
        //设置玩家交换对象
        if (body is Player player) player.Interactables.Remove(this);
    }

    
}