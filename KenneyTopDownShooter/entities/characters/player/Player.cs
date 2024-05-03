using Godot;
using System;
/// <summary>
/// 玩家
/// </summary>
public partial class Player : Character
{
    public event Action<BulletArgs> ShootEvent; 
    
    //枪口标记
    private Marker2D _endOfGunMarker;
    //枪口方向标记
    private Marker2D _gunDirectionMarker;
    //攻击冷却时间
    private Timer _attackCoolDownTimer;
    //枪口火光
    private AnimationPlayer _muzzleFlashAnimationPlayer;
    public override void _Ready()
    {
        _endOfGunMarker = GetNode<Marker2D>("EndOfGunMarker");
        _gunDirectionMarker = GetNode<Marker2D>("GunDirectionMarker");
        _attackCoolDownTimer = GetNode<Timer>("AttackCoolDownTimer");
        _muzzleFlashAnimationPlayer = GetNode<AnimationPlayer>("MuzzleFlashAnimationPlayer");
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
            Shoot();
        }
    }

    /// <summary>
    /// 移动
    /// </summary>
    private void Move()
    {
        var movementDirection = Vector2.Zero;
        if (Input.IsActionPressed("move_left"))
        {
            movementDirection.X = -1;
        }
        if (Input.IsActionPressed("move_right"))
        {
            movementDirection.X = 1;
        }
        if (Input.IsActionPressed("move_up"))
        {
            movementDirection.Y = -1;
        }
        if (Input.IsActionPressed("move_down"))
        {
            movementDirection.Y = 1;
        }
        //归一化
        movementDirection = movementDirection.Normalized();
        //设置速度
        Velocity = movementDirection * CharacterData.BaseSpeed;
        //移动
        MoveAndSlide();
        //朝向 鼠标方向
        LookAt(GetGlobalMousePosition());
    }
    /// <summary>
    /// 射击
    /// </summary>
    private void Shoot()
    {
        //攻击冷却后才可以射击
        if (_attackCoolDownTimer.IsStopped())
        {
            //枪口火光
            _muzzleFlashAnimationPlayer.Play("muzzle_flash");
            //生成子弹
            ShootEvent?.Invoke(new BulletArgs
            {
                Bullet = Bullet.Instantiate<Bullet>(), //生成子弹
                Position = _endOfGunMarker.GlobalPosition, //子弹位置
                Direction = (_gunDirectionMarker.GlobalPosition - _endOfGunMarker.GlobalPosition).Normalized() //子弹方向
            });
            //攻击冷却
            _attackCoolDownTimer.Start();
            
            
        }
        
    }
}
