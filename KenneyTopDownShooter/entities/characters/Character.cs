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
    public event  Action Hit;
    /// <summary>
    /// 角色死亡
    /// </summary>
    public event  Action Died;
    
    /// <summary>
    /// 角色数据
    /// </summary>
    [Export] public CharacterData CharacterData;
    
    public virtual void OnHit()
    {
        Hit?.Invoke();
    }

    public virtual void OnDied()
    {
        Died?.Invoke();
    }
}
