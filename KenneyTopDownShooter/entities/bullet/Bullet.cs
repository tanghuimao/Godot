using Godot;
using System;
using KenneyTopDownShooter.resources.bullet_data;

/// <summary>
/// 子弹
/// </summary>
public partial class Bullet : Area2D
{
    /// <summary>
    /// 子弹数据
    /// </summary>
    [Export] public BulletData BulletData;
    /// <summary>
    /// 设置子弹方向
    /// </summary>
    /// <param name="direction"></param>
    public virtual void SetDirection(Vector2 direction)
    {
        //设置角度
        Rotation += direction.Angle();
    }
}
