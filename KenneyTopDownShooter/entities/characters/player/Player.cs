using Godot;
using System;
using KenneyTopDownShooter.common;

/// <summary>
/// 玩家
/// </summary>
public partial class Player : Character
{

    private Weapon _weapon;
    
    public override void _Ready()
    {
        _weapon = GetNode<Weapon>("Weapon");
        Hit += OnPlayerHit;
        Died += OnPlayerDied;
    }

    /// <summary>
    /// 帧循环
    /// </summary>
    /// <param name="delta"></param>
    public override void _PhysicsProcess(double delta)
    {
        Move();
    }
    /// <summary>
    /// 输入事件
    /// </summary>
    /// <param name="event"></param>
    public override void _UnhandledInput(InputEvent @event)
    {   
        // 射击
        if (@event.IsActionPressed("shoot"))
        {
            _weapon.Shoot();
        }
    }

    /// <summary>
    /// 移动
    /// </summary>
    private void Move()
    {
        var movementDirection = Input.GetVector("move_left","move_right", "move_up", "move_down");
        // var movementDirection = Vector2.Zero;
        // if (Input.IsActionPressed("move_left"))
        // {
        //     movementDirection.X = -1;
        // }
        // if (Input.IsActionPressed("move_right"))
        // {
        //     movementDirection.X = 1;
        // }
        // if (Input.IsActionPressed("move_up"))
        // {
        //     movementDirection.Y = -1;
        // }
        // if (Input.IsActionPressed("move_down"))
        // {
        //     movementDirection.Y = 1;
        // }
        // //归一化
        // movementDirection = movementDirection.Normalized();
        //设置速度
        Velocity = movementDirection * CharacterData.Speed;
        //移动
        MoveAndSlide();
        //朝向 鼠标方向
        LookAt(GetGlobalMousePosition());
    }
    /// <summary>
    /// 射击
    /// </summary>
    private void OnWeaponShootEvent(BulletArgs args)
    {
        //触发事件
        GlobalEvent.OnBulletFiredEvent(args);
    }
    /// <summary>
    /// 玩家被击中
    /// </summary>
    private void OnPlayerHit(Bullet bullet)
    {
        CharacterData.SetHealth(bullet.BulletData.Damage);
        if (CharacterData.Health <= 0)
        {
            OnDied();
        }
        GD.Print($"玩家被击中, health:{CharacterData.Health}");
    }
    /// <summary>
    /// 死亡
    /// </summary>
    private void OnPlayerDied()
    {
        QueueFree();
    }
}
