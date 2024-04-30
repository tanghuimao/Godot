using Godot;
using System;
using FlappyBird.Common;

/// <summary>
/// 管道
/// </summary>
public partial class Pipe : Node2D
{
    private float _minSpeed = 100f;
    private float _maxSpeed = 500f;
    private float _currentSpeed;

    private Area2D _area2DScore;
    
    public override void _Ready()
    {
        _currentSpeed = _minSpeed;
        _area2DScore = GetNode<Area2D>("Area2DScore");
        //得分区域
        _area2DScore.BodyExited += OnBodyExited;
    }
    
    public override void _Process(double delta)
    {
        Position += Vector2.Left * _currentSpeed* (float)delta;

        // 移除
        if (Position.X < -40)
        {
            QueueFree();
        }
    }
    /// <summary>
    /// 得分
    /// </summary>
    /// <param name="body"></param>
    private void OnBodyExited(Node2D body)
    {
        if (body is Bird)
        {
            AudioManager.PlaySFX(SFX.sfx_point);
            Game.OnAddScoreEvent();
        }
    }
}
