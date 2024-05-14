using Godot;
using System;
/// <summary>
/// 汽车
/// </summary>
public partial class Car : PathFollow2D
{
    private Node2D _turret;
    private Area2D _noticeArea2D;
    private Timer _hitTimer;
    private AnimationPlayer _animationPlayer;
    private bool _canAttack;
    private Line2D _line1;
    private Line2D _line2;
    private Sprite2D _gunFire1;
    private Sprite2D _gunFire2;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _noticeArea2D = GetNode<Area2D>("NoticeArea2D");
        _turret = GetNode<Node2D>("Turret");
        _noticeArea2D.BodyEntered += OnPlayerBodyEntered;
        _noticeArea2D.BodyExited += OnPlayerBodyExited;
        _line1 = GetNode<Line2D>("Turret/RayCast2D/Line2D");
        _line2 = GetNode<Line2D>("Turret/RayCast2D2/Line2D");
        _gunFire1 = GetNode<Sprite2D>("Turret/GunFire1");
        _gunFire2 = GetNode<Sprite2D>("Turret/GunFire2");
    }

    private async void OnPlayerBodyExited(Node2D body)
    {
        //暂停动画
        _animationPlayer.Pause();
        _canAttack = false;
        var tween = CreateTween();
        tween.SetParallel();
        tween.TweenProperty(_line1, "width", 0, GD.RandRange(0.1, 0.5));
        tween.TweenProperty(_line2, "width", 0, GD.RandRange(0.1, 0.5));
        await ToSignal(tween, "finished");
        //暂停动画
        _animationPlayer.Stop();
    }

    private void OnPlayerBodyEntered(Node2D body)
    {
        _animationPlayer.Play("laser_load");
        _canAttack = true;
    }

    public override void _Process(double delta)
    {
        ProgressRatio += 0.02f * (float)delta;

        if (_canAttack)
        {
            FaceToPlayer();
        }
    }
    
    private void FaceToPlayer()
    {
        _turret.LookAt(Global.PlayerPosition);
        //设置旋转
        // var angle = _turret.GlobalPosition.DirectionTo(Global.PlayerPosition).Angle();
        // GD.Print((angle));
        // _turret.Rotation = Mathf.Lerp(_turret.Rotation, angle, 0.1f);
    }

    public void Fire()
    {
        if (_canAttack)
        {
            Global.Health -= 20;
            //设置颜色 透明度
            var gunFireModulate = _gunFire1.Modulate;
            gunFireModulate.A = 1;
            _gunFire1.Modulate = gunFireModulate;
            _gunFire2.Modulate = gunFireModulate;
            //补间动画
            var tween = CreateTween();
            tween.SetParallel();
            tween.TweenProperty(_gunFire1, "modulate:a", 0, GD.RandRange(0.1, 0.5));
            tween.TweenProperty(_gunFire2, "modulate:a", 0, GD.RandRange(0.1, 0.5));
        }
        
    }
}
