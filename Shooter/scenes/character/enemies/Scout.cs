using Godot;
using System;
using Shooter.scenes.projectiles;

/// <summary>
/// 侦察兵
/// </summary>
public partial class Scout : CharacterBody2D
{
    //射击事件
    public event Action<ProjectParam> ShootLaserEvent;
    [Export]
    public int Health = 80;
    //玩家离开区域
    private bool _nearPlayer;
    private Area2D _area2D;
    private Timer _laserCoolDownTimer;
    private Timer _hitTimer;
    private Node2D _laserSpawnPositions;
    private Sprite2D _sprite2D;
    private int _laserSpawnIndex;
    
    private bool _canLaser = true;
    private bool _canHit = true;
    
    public override void _Ready()
    {
        _area2D = GetNode<Area2D>("Area2D");
        _sprite2D = GetNode<Sprite2D>("Sprite2D");
        _area2D.BodyEntered += OnPlayerBodyEntered;
        _area2D.BodyExited += OnPlayerBodyExited;
        _laserCoolDownTimer = GetNode<Timer>("TimerNode/LaserCoolDownTimer");
        _hitTimer = GetNode<Timer>("TimerNode/HitTimer");
        _laserCoolDownTimer.Timeout += OnScoutLaserTimeout;
        _hitTimer.Timeout += OnScoutHitTimeout;
        _laserSpawnPositions = GetNode<Node2D>("LaserSpawnPositions");
    }



    public override void _Process(double delta)
    {
        if (_nearPlayer)
        {
            FaceToPlayer();

            if (!_canLaser) return;
            var marker = _laserSpawnPositions.GetChild<Marker2D>(_laserSpawnIndex);
            var direction = (Global.PlayerPosition - GlobalPosition).Normalized();
            ShootLaserEvent?.Invoke(new ProjectParam
            {
                Position = marker.GlobalPosition,
                Direction = direction,
            });
            _canLaser = false;
            _laserCoolDownTimer.Start();
            _laserSpawnIndex = _laserSpawnIndex == 0? 1 : 0;
        }
        
    }
    

    public override void _ExitTree()
    {
        _area2D.BodyEntered -= OnPlayerBodyEntered;
        _area2D.BodyExited -= OnPlayerBodyExited;
        _laserCoolDownTimer.Timeout -= OnScoutLaserTimeout;
    }
    /// <summary>
    /// 命中
    /// </summary>
    public void Hit()
    {
        if (!_canHit) return;
        //材质转换为sharder材质
        var sprite2DMaterial = _sprite2D.Material as ShaderMaterial;
        //设置参数
        sprite2DMaterial?.SetShaderParameter("progress", 1);
        Health -= 10;
        if (Health <= 0)
        {
            QueueFree();
        }
        _canHit = false;
        _hitTimer.Start();
    }
    
    private void OnPlayerBodyEntered(Node2D body)
    {
        _nearPlayer = true;
    }
    
    private void OnPlayerBodyExited(Node2D body)
    {
        _nearPlayer = false;
    }
    
    private void OnScoutLaserTimeout()
    {
        _canLaser = true;
    }    
    private void OnScoutHitTimeout()
    {
        _canHit = true;
        //材质转换为sharder材质
        var sprite2DMaterial = _sprite2D.Material as ShaderMaterial;
        //设置参数
        if (sprite2DMaterial != null) sprite2DMaterial.SetShaderParameter("progress", 0);

    }
    
    private void FaceToPlayer()
    {
        // LookAt(Global.PlayerPosition);
        //设置旋转
        var angle = GlobalPosition.DirectionTo(Global.PlayerPosition).Angle();
        Rotation = Mathf.Lerp(Rotation, angle, 0.1f);
    }
}
