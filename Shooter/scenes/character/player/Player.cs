using Godot;
using System;
using Shooter.scenes.projectiles;

/// <summary>
/// 玩家
/// </summary>
public partial class Player : CharacterBody2D
{
    public event Action<ProjectParam> ShootLaserEvent;
    public event Action<ProjectParam> ShootGrenadeEvent;

    [Export] public float MaxSpeed = 500;
    private float _speed;
    //条件
    private bool _canLaser = true;
    private bool _canGrenade = true;
    private bool _canHit = true;
    //计时器
    private Timer _laserTimer;
    private Timer _grenadeTimer;
    private Timer _hitTimer;
    //标记生成点
    private Marker2D _startMarker;
    private Marker2D _endMarker;
    
    //粒子
    private GpuParticles2D _gPUParticles2D;
    
    
    public override void _Ready()
    {
        _speed = MaxSpeed;
        _laserTimer = GetNode<Timer>("TimerNode/LaserTimer");
        _laserTimer.Timeout += () => _canLaser = true;
        _grenadeTimer = GetNode<Timer>("TimerNode/GrenadesTimer");
        _grenadeTimer.Timeout += () => _canGrenade = true;
        _hitTimer = GetNode<Timer>("TimerNode/HitTimer");
        _hitTimer.Timeout += () => _canHit = true;
        _startMarker = GetNode<Marker2D>("WeaponPositions/StartMarker2D");
        _endMarker = GetNode<Marker2D>("WeaponPositions/EndMarker2D");
        _gPUParticles2D = GetNode<GpuParticles2D>("GPUParticles2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        Move();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        //获取方向
        var direction = (_endMarker.GlobalPosition - _startMarker.GlobalPosition).Normalized();
        if (@event.IsActionPressed("primary_action") && _canLaser && Global.LaserAmount > 0)
        {
            //消耗
            Global.LaserAmount -= 1;
            //粒子
            _gPUParticles2D.Emitting = true;
            //发射
            ShootLaserEvent?.Invoke(new ProjectParam
            {
                Position = _startMarker.GlobalPosition,
                Direction = direction,
            });
            _canLaser = false;
            _laserTimer.Start();
            return;
        }
        if (@event.IsActionPressed("second_action") && _canGrenade && Global.GrenadeAmount > 0)
        {
            //消耗
            Global.GrenadeAmount -= 1;
            //发射
            ShootGrenadeEvent?.Invoke(new ProjectParam
            {
                Position = _startMarker.GlobalPosition,
                Direction = direction,
            });
            _canGrenade = false;
            _grenadeTimer.Start();
            return;
        }
    }

    private void Move()
    {
        //获取移动方向
        var movementDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        //设置速度
        Velocity = movementDirection * _speed;
        //移动
        MoveAndSlide();
        //朝向 鼠标方向
        LookAt(GetGlobalMousePosition());
        //变更玩家全局位置
        Global.PlayerPosition = GlobalPosition;
    }

    public void AddItem(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Lazer1:
            case ItemType.Lazer2:
            case ItemType.Lazer3:
                Global.LaserAmount += 10;
                break;
            case ItemType.Grenade:
                Global.GrenadeAmount += 2;
                break;
            case ItemType.Health:
                Global.Health += 20;
                break;
        }
    }
    
    /// <summary>
    /// 命中
    /// </summary>
    public void Hit()
    {
        if (!_canHit) return;
        _canHit = false;
        _hitTimer.Start();
        Global.Health -= 10;
    }
}