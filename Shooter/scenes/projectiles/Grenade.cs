using Godot;
using System;
using Shooter.scenes.projectiles;

public partial class Grenade : RigidBody2D
{
    //导出速度
    [Export] public float Speed = 500;

    private AnimationPlayer _animationPlayer;

    //是否爆炸
    private bool _isExplode = false;
    //爆炸范围
    private float _explodeRadius = 400;

    public override void _Ready()
    {
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
    }

    /// <summary>
    /// 设置弹道参数
    /// </summary>
    /// <param name="param"></param>
    public void SetProjectParam(ProjectParam param)
    {
        Position = param.Position;
        LinearVelocity = param.Direction * Speed;
    }

    /// <summary>
    /// 爆炸函数
    /// </summary>
    public void Explode()
    {
        _animationPlayer.Play("explosion");
        _isExplode = true;
    }
}