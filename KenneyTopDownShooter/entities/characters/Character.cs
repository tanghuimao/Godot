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
}
