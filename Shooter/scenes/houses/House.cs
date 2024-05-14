using Godot;
using System;
/// <summary>
/// 房间
/// </summary>
public partial class House : Node2D
{
    //进入
    public event Action EnterHouseEvent;
    //退出
    public event Action ExitHouseEvent;
    
    private Area2D _area2D;
    public override void _Ready()
    {
        _area2D = GetNode<Area2D>("Area2D");
        _area2D.BodyEntered += OnBodyEntered;
        _area2D.BodyExited += OnBodyExited;
    }

    public override void _ExitTree()
    {
        _area2D.BodyEntered -= OnBodyEntered;
        _area2D.BodyExited -= OnBodyExited;
    }

    private void OnBodyExited(Node2D body)
    {
        ExitHouseEvent?.Invoke();
    }
    
    private void OnBodyEntered(Node2D body)
    {
        EnterHouseEvent?.Invoke();
    }
}
