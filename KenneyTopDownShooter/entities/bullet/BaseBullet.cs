using Godot;
using System;
/// <summary>
/// 基础子弹
/// </summary>
public partial class BaseBullet : Bullet
{
    private  Vector2 _direction = Vector2.Zero;

    public override void _Ready()
    {
        // 子弹死亡计时器
        GetNode<Timer>("KillTimer").Timeout += () =>
        {
            QueueFree();
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_direction != Vector2.Zero)
        {
            GlobalPosition += _direction * BulletData.Speed;
        }
    }

    public override void SetDirection(Vector2 direction)
    {
        base.SetDirection(direction);
        _direction = direction;
    }
}
