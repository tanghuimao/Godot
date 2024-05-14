using Godot;
using System;
/// <summary>
/// 无人机
/// </summary>
public partial class Drone : CharacterBody2D
{
    [Export]
    public int Health = 50;
    [Export]
    public int Speed;
    public int MaxSpeed = 600;
    private bool _active;
    private bool _canHit = true;
    //是否爆炸
    private bool _isExplode = false;
    //爆炸范围
    private float _explodeRadius = 400;
    
    private Area2D _noticeArea2D;
    private Timer _hitTimer;
    private Sprite2D _sprite2D;
    private Sprite2D _explosion;
    private AnimationPlayer _animationPlayer;
    

    public override void _Ready()
    {
        _noticeArea2D = GetNode<Area2D>("NoticeArea2D");
        _noticeArea2D.BodyEntered += body =>
        {
            _active = true;
            var tween = CreateTween();
            tween.TweenProperty(this, "Speed", MaxSpeed, 6);
        };
        _noticeArea2D.BodyExited += body => _active = false;
        _hitTimer = GetNode<Timer>("HitTimer");
        _hitTimer.Timeout += () =>
        {
            //材质转换为sharder材质
            var sprite2DMaterial = _sprite2D.Material as ShaderMaterial;
            //设置参数
            sprite2DMaterial?.SetShaderParameter("progress", 0);
            _canHit = true;
        };
        _sprite2D = GetNode<Sprite2D>("Sprite2D");
        _explosion = GetNode<Sprite2D>("Explosion");
        _explosion.Hide();
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    

    public override void _Process(double delta)
    {
        if (_isExplode)
        {
            var nodes = GetTree().GetNodesInGroup("Entity");
            foreach (var node in nodes)
            {
                if (node is Node2D node2D)
                {
                    var distance = GlobalPosition.DistanceTo(node2D.GlobalPosition);
                    if (node.HasMethod("Hit") && distance <= _explodeRadius)
                    {
                        node.Call("Hit");
                    }
                }
            }
        }

        if (_active)
        {
            var direction = (Global.PlayerPosition - GlobalPosition).Normalized();
            Velocity = direction * Speed;
            FaceToPlayer();
            var collision = MoveAndSlide();
            //发送碰撞检测
            if (!collision) return;
            _animationPlayer.Play("explosion");
            _active = false;
            _isExplode = true;
        }
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
            _active = false;
            _animationPlayer.Play("explosion");
        }
        _canHit = false;
        _hitTimer.Start();
    }
    
    private void FaceToPlayer()
    {
        // LookAt(Global.PlayerPosition);
        //设置旋转
        var angle = GlobalPosition.DirectionTo(Global.PlayerPosition).Angle();
        Rotation = Mathf.Lerp(Rotation, angle, 0.1f);
    }
}
