using Godot;
using System;
/// <summary>
/// 角色
/// </summary>
public partial class Character : CharacterBody2D
{
    /// <summary>
    /// 角色数据
    /// </summary>
    [Export] public CharacterData CharacterData;
    
    /// <summary>
    /// 子弹
    /// </summary>
    [Export] public PackedScene Bullet;
}
