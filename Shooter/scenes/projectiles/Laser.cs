using Godot;
using System;
using Shooter.scenes.projectiles;

/// <summary>
/// 激光
/// </summary>
public partial class Laser : Area2D
{
    //导出速度
    [Export] public float Speed = 1000;

    private  Vector2 _driection = Vector2.Up;
    private VisibleOnScreenNotifier2D _visibleOnScreenNotifier2D;

    public override void _Ready()
    {
         _visibleOnScreenNotifier2D = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
        _visibleOnScreenNotifier2D.ScreenExited += QueueFree;
        BodyEntered += OnLaserBodyEntered;
    }

    public override void _ExitTree()
    {
        _visibleOnScreenNotifier2D.ScreenExited -= QueueFree;
        BodyEntered -= OnLaserBodyEntered;
    }
    
    public override void _Process(double delta)
    {
        //移动
        Position += _driection * Speed * (float)delta;
    }
    /// <summary>
    /// 设置弹道参数
    /// </summary>
    /// <param name="param"></param>
    public void SetProjectParam(ProjectParam param)
    {
        //设置位置
        Position = param.Position;
        //设置旋转角度
        RotationDegrees = Mathf.RadToDeg(param.Direction.Angle()) + 90;
        //设置移动方向
        _driection = param.Direction;
    }
    /// <summary>
    /// 碰撞检测
    /// </summary>
    /// <param name="body"></param>
    private void OnLaserBodyEntered(Node2D body)
    {
        if (body.HasMethod("Hit"))
        {
            body.Call("Hit");
        }
        QueueFree();
    }
}
