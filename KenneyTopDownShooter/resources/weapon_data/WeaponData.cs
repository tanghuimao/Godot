using Godot;
using System;
using KenneyTopDownShooter.resources.bullet_data;

/// <summary>
/// 武器数据
/// </summary>
public partial class WeaponData : Resource
{
    //名称
    [Export] public string Name { get; set; }
    //枪口火光动画名称
    [Export] public string MuzzleFlashName { get; set; } = "muzzle_flash";
    //子弹
    [Export] public PackedScene Bullet { get; set; }

    //子弹数量
    [Export] public short BulletCount { get; set; } = 30;
    
    //射速
    [Export] public short ShootSpeed { get; set; } = 10;
    
    //射击间隔 0.01/s
    [Export] public float ShootInterval { get; set; } = 0.1f;
}