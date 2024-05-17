using Godot;
using System;
/// <summary>
/// 史莱姆
/// </summary>
public partial class Slime : CharacterBody2D
{
    [Export] public int Speed = 30;
    [Export] public int PatrolRange = 50;
    
    private AnimationPlayer _animationPlayer;
    private Area2D _hurtArea2D;
    private  CollisionShape2D _collisionShape2D;
    
    // 原始位置
    private Vector2 _originalPosition;
    // 巡逻计时器
    private Timer _patrolTimer;
    // 巡逻位置
    private Vector2 _patrolLocation;
    // 是否可以巡逻
    private bool _canPatrol;
    // 是否死亡
    private bool _isDied;
    
    public override void _Ready()
    {
        _originalPosition = Position;
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        _hurtArea2D = GetNode<Area2D>("HurtArea2D");
        _hurtArea2D.BodyEntered += OnHurtBodyEntered;
        _patrolTimer = GetNode<Timer>("PatrolTimer");
        _patrolTimer.Timeout += OnPatrolTimeout;
        OnPatrolTimeout();
    }
    /// <summary>
    /// 碰撞体进入
    /// </summary>
    /// <param name="body"></param>
    private async void OnHurtBodyEntered(Node2D body)
    {
        if (body is not StaticBody2D staticBody2D) return;
        GD.Print((staticBody2D.Name));
        if (body.Name == "Sword")
        {
            Callable.From(() => _collisionShape2D.Disabled = true).CallDeferred();
            _isDied = true;
            _animationPlayer.Play("died");
            await ToSignal(_animationPlayer, "animation_finished");
            QueueFree();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isDied) return;
        Move();
    }
    /// <summary>
    /// 移动
    /// </summary>
    private void Move()
    {
        if (_canPatrol)
        {
            var distance = GlobalPosition.DistanceTo(_patrolLocation);
            if (distance < 5)
            {
                _patrolTimer.Start();
                _canPatrol = false;
                return;
            }
            var direction = GlobalPosition.DirectionTo(_patrolLocation).Normalized();
            //更新动画
            UpdateAnimation(direction);
            //设置速度
            Velocity = direction * Speed;
            //平滑移动
            var moveAndSlide = MoveAndSlide();
            //存在碰撞
            if (moveAndSlide)
            {
                _patrolTimer.Start();
                _canPatrol = false;
                return;
            }
        }
       
       
    }
    /// <summary>
    /// 更新动画
    /// </summary>
    /// <param name="direction"></param>
    private void UpdateAnimation(Vector2 direction)
    {
        if (direction == Vector2.Zero)
        {
            if (_animationPlayer.IsPlaying())
            {
                _animationPlayer.Stop();
            }
        }
        else
        {
            var animationName = "walk_";

            if (direction.Y < 0 )
            {
                animationName += "up";
            }
            else if (direction.Y > 0)
            {
                animationName += "down";
            }
            else if (direction.X < 0)
            {
                animationName += "left";
            }
            else if (direction.X > 0)
            {
                animationName += "right";
            }
            _animationPlayer.Play(animationName);
        }
    }
    
    /// <summary>
    /// 巡逻计时器超时
    /// </summary>
    private void OnPatrolTimeout()
    {
        //随机范围
        var randomX = GD.RandRange(-PatrolRange, PatrolRange);
        var randomY = GD.RandRange(-PatrolRange, PatrolRange);
        _patrolLocation = new Vector2(randomX, randomY) + _originalPosition;
        _canPatrol = true;
    }
}
