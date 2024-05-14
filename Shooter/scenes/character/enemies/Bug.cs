using Godot;
using System;
using System.Threading.Tasks;

/// <summary>
/// 虫子
/// </summary>
public partial class Bug : CharacterBody2D
{
    [Export]
    public int Health = 100;
    [Export]
    public int Speed = 300;
    //玩家靠近
    private bool _active;
    private Area2D _noticeArea2D;
    private Area2D _attackArea2D;
    private Timer _attackCoolDownTimer;
    private Timer _hitTimer;
    private AnimatedSprite2D _animatedSprite2D;
    private GpuParticles2D _hitGpuParticles2D;
    
    private bool _canAttack;
    private bool _canHit = true;
    private bool _isDied = false;
    
    public override void _Ready()
    {
        _noticeArea2D = GetNode<Area2D>("NoticeArea2D");
        _noticeArea2D.BodyEntered += OnNoticeBodyEntered;
        _noticeArea2D.BodyExited += OnNoticeBodyExited;
        _attackArea2D = GetNode<Area2D>("AttackArea2D");
        _attackArea2D.BodyEntered += OnAttackBodyEntered;
        _attackArea2D.BodyExited += OnAttackBodyExited;
        _attackCoolDownTimer = GetNode<Timer>("TimerNode/AttackCoolDownTimer");
        _hitTimer = GetNode<Timer>("TimerNode/HitTimer");
        _attackCoolDownTimer.Timeout += OnBugAttackTimeout;
        _hitTimer.Timeout += OnBugHitTimeout;
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _hitGpuParticles2D = GetNode<GpuParticles2D>("Particles/HitGPUParticles2D");
        _animatedSprite2D.AnimationFinished += OnAnimationFinished;
    }
    

    public override void _Process(double delta)
    {
        if (!_active) return;
        var direction = (Global.PlayerPosition - GlobalPosition).Normalized();
        Velocity = direction * Speed;
        MoveAndSlide();
        FaceToPlayer();

    }
    

    public override void _ExitTree()
    {
        _noticeArea2D.BodyEntered -= OnNoticeBodyEntered;
        _noticeArea2D.BodyExited -= OnNoticeBodyExited;
        _attackArea2D.BodyEntered -= OnAttackBodyEntered;
        _attackArea2D.BodyExited -= OnAttackBodyExited;
        _attackCoolDownTimer.Timeout -= OnBugAttackTimeout;
        _hitTimer.Timeout -= OnBugHitTimeout;
    }
    /// <summary>
    /// 命中
    /// </summary>
    public async void Hit()
    {
        if (!_canHit) return;
        //播放粒子特效
        _hitGpuParticles2D.Emitting = true;
        //材质转换为sharder材质
        var sprite2DMaterial = _animatedSprite2D.Material as ShaderMaterial;
        //设置参数
        sprite2DMaterial?.SetShaderParameter("progress", 1);
        Health -= 10;
        if (Health <= 0 && !_isDied)
        {
            _isDied = true;
            await Task.Delay(500);
            QueueFree();
        }
        _canHit = false;
        _hitTimer.Start();
    }
    
    private void OnNoticeBodyEntered(Node2D body)
    {
        
        _active = true;
        _animatedSprite2D.Play("walk");
    }
    
    private void OnNoticeBodyExited(Node2D body)
    {
        _active = false;
        _animatedSprite2D.Stop();
    }
    
    private void OnAttackBodyEntered(Node2D body)
    {
        _canAttack = true;
        _animatedSprite2D.Play("attack");
    }
    
    private void OnAttackBodyExited(Node2D body)
    {
        _canAttack = false;
        _animatedSprite2D.Play("walk");
    }
    
    private void OnAnimationFinished()
    {
        if (_canAttack)
        {
            Global.Health -= 10;
            _attackCoolDownTimer.Start();
        }
    }
    
    private void OnBugAttackTimeout()
    {
        _animatedSprite2D.Play("attack");
    }    
    private void OnBugHitTimeout()
    {
        _canHit = true;
        //材质转换为sharder材质
        var sprite2DMaterial = _animatedSprite2D.Material as ShaderMaterial;
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
