using Godot;
using System;
/// <summary>
/// 角色
/// </summary>
public partial class Character : CharacterBody2D
{
    /// <summary>
    /// 角色被击中
    /// </summary>
    public event  Action<Bullet> Hit;
    /// <summary>
    /// 角色死亡
    /// </summary>
    public event  Action Died;
    
    /// <summary>
    /// 角色数据
    /// </summary>
    [Export] public CharacterData CharacterData;
    
    public virtual void OnHit(Bullet bullet)
    {
        Hit?.Invoke(bullet);
    }

    public virtual void OnDied()
    {
        Died?.Invoke();
    }
    
    /// <summary>
    /// 旋转到指定位置
    /// </summary>
    /// <param name="targetLocation"></param>
    public float RotateToTarget(Vector2 targetLocation)
    {
        var angle = GlobalPosition.DirectionTo(targetLocation).Angle();
        Rotation = Mathf.Lerp(Rotation, angle, 0.1f);
        return angle;
    }
    
    /// <summary>
    /// 移动到指定位置
    /// </summary>
    /// <param name="targetLocation"></param>
    public void MoveToTarget(Vector2 targetLocation)
    {
        // 计算速度
        Velocity = GlobalPosition.DirectionTo(targetLocation).Normalized() * CharacterData.Speed;
        // 移动
        MoveAndSlide();
    }
}
