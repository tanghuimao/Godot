using Godot;
using System;
using System.Threading.Tasks;
using FlappyBird.Common;

/// <summary>
/// 鸟
/// </summary>
public partial class Bird : RigidBody2D
{
    public override void _Ready()
    {
        // 碰撞检测
        BodyEntered += OnBodyEntered;
    }
    
    public override void _PhysicsProcess(double delta)
    {
        // 鼠标左键点击
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            AudioManager.PlaySFX(SFX.sfx_swooshing);
            // 向上飞 线性向上
            LinearVelocity = Vector2.Up * 300;
            // 角速度
            AngularVelocity = -3;
        }
        
        // 最大旋转角度 30
        if (RotationDegrees < -30f)
        {
            RotationDegrees = -30;
            AngularVelocity = 0;
        }
        //下落时
        if (LinearVelocity.Y > 0)
        {
            AngularVelocity = 1.5f;
        }
    }
    /// <summary>
    /// 碰撞检测 检测其他区域进入
    /// </summary>
    /// <param name="body"></param>
    private void OnBodyEntered(Node body)
    {
        if (body is StaticBody2D)
        {
            AudioManager.PlaySFX(SFX.sfx_hit);
            Game.OnGameOverEvent();
        }
    }
}
