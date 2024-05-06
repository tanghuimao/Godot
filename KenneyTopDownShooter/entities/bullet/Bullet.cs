using Godot;
using System;
using KenneyTopDownShooter.resources.bullet_data;

/// <summary>
/// 子弹
/// </summary>
public partial class Bullet : Area2D
{
    /// <summary>
    /// 子弹数据
    /// </summary>
    [Export] public BulletData BulletData;

    /// <summary>
    /// 移动方向
    /// </summary>
    private Vector2 _direction = Vector2.Zero;
    /// <summary>
    /// 角色
    /// </summary>
    private Character _character;

    public override void _Ready()
    {
        // 子弹退出屏幕
        GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D").ScreenExited += QueueFree;
        // 碰撞检测
        BodyEntered += OnBodyEntered;
    }

    public override void _ExitTree()
    {
        BodyEntered -= OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Character character && character != _character)
        {
            character.OnHit(this);
            QueueFree();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_direction != Vector2.Zero)
        {
            GlobalPosition += _direction * BulletData.Speed;
        }
    }

    /// <summary>
    /// 设置子弹方向
    /// </summary>
    /// <param name="direction"></param>
    public void SetDirection(Vector2 direction)
    {
        _direction = direction;
        //设置角度
        Rotation += direction.Angle();
    }
    
    /// <summary>
    /// 设置子弹来源
    /// </summary>
    /// <param name="character"></param>
    public void SetOrigin(Character character)
    {
        _character = character;
    }
}