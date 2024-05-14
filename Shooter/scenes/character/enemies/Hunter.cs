using Godot;
using System;
/// <summary>
/// 猎人
/// </summary>
public partial class Hunter : CharacterBody2D
{
    [Export]
    public int Health = 500;
    [Export]
    public int Speed = 300;
    //玩家靠近
    private bool _active;
    private Area2D _noticeArea2D;
    private Area2D _attackArea2D;
    private Timer _navigationTimer;
    private Timer _hitTimer;
    private AnimationPlayer _animationPlayer;
    private GpuParticles2D _hitGpuParticles2D;
    private NavigationAgent2D _navigationAgent2D;
    
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
        _navigationTimer = GetNode<Timer>("TimerNode/NavigationTimer");
        _navigationTimer.Timeout += OnNavigationTimeout;
        _hitTimer = GetNode<Timer>("TimerNode/HitTimer");
        _hitTimer.Timeout += OnBugHitTimeout;
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        //导航
        _navigationAgent2D = GetNode<NavigationAgent2D>("NavigationAgent2D");
        //设置目标路径
        _navigationAgent2D.TargetPosition = Global.PlayerPosition;
        //设置路径距离
        _navigationAgent2D.PathDesiredDistance = 4f;
        _navigationAgent2D.TargetDesiredDistance = 4f;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_active)
        {
            //获取导航路径
            var nextPathPosition = _navigationAgent2D.GetNextPathPosition();
            //移动方向
            var direction = (nextPathPosition - GlobalPosition).Normalized();
            //设置速度
            Velocity = direction * Speed;
            //移动
            MoveAndSlide();
            //设置朝向
            var angle = direction.Angle();
            // Rotation = Mathf.Lerp(Rotation, angle + Mathf.Pi / 2, 0.1f);
            Rotation = angle + Mathf.Pi / 2;
        }
    }
    
    private void OnNavigationTimeout()
    {
        if (_active)
        {
            //设置目标路径
            _navigationAgent2D.TargetPosition = Global.PlayerPosition;
        }
        
    }
    
    private void OnNoticeBodyEntered(Node2D body)
    {
        _active = true;
        _animationPlayer.Play("walk");
    }
    
    private void OnNoticeBodyExited(Node2D body)
    {
        _active = false;
    }
    
    private void OnAttackBodyEntered(Node2D body)
    {
        _canAttack = true;
        _animationPlayer.Play("attack");
    }
    private void OnAttackBodyExited(Node2D body)
    {
        _canAttack = false;
        _active = true;
        _animationPlayer.Play("walk");
    }

    public void Attack()
    {
        if (_canAttack)
        {
            Global.Health -= 20;
        }
    }
    
    private void OnBugHitTimeout()
    {
        if (!_canHit) return;
        Health -= 10;
        if (Health <= 0)
        {
            _active = false;
            _animationPlayer.Play("explosion");
        }
        _canHit = false;
        _hitTimer.Start();
    }
}
