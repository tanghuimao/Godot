using System;
using Godot;
using KenneyTopDownShooter.common;

public partial class Weapon : Node
{
    /// <summary>
    /// 武器数据
    /// </summary>
    [Export] public WeaponData WeaponData;
    //子弹生成
    public Marker2D BulletSpawnMarker;
    //枪口方向
    public Marker2D GunDirectionMarker;
    //攻击冷却
    public Timer AttackCoolDownTimer;
    //枪口动画
    public AnimationPlayer MuzzleFlashAnimationPlayer;
    
    public override void _Ready()
    {
        BulletSpawnMarker = GetNode<Marker2D>("BulletSpawnMarker");
        GunDirectionMarker = GetNode<Marker2D>("GunDirectionMarker");
        AttackCoolDownTimer = GetNode<Timer>("AttackCoolDownTimer");
        MuzzleFlashAnimationPlayer = GetNode<AnimationPlayer>("MuzzleFlashAnimationPlayer");
        // 设置射击间隔
        AttackCoolDownTimer.WaitTime = WeaponData.ShootInterval;
    }

    /// <summary>
    /// 射击
    /// </summary>
    public virtual void Shoot()
    {
        if (AttackCoolDownTimer.IsStopped() && WeaponData.Bullet != null)
        {
            MuzzleFlashAnimationPlayer.Play(WeaponData.MuzzleFlashName);
            // 发射子弹
            GlobalEvent.OnBulletFiredEvent(new BulletSpawn
            {
                Character = GetParent<Character>(),
                Bullet = WeaponData.Bullet.Instantiate<Bullet>(), //生成子弹
                Position = BulletSpawnMarker.GlobalPosition, //子弹位置
                Direction = (GunDirectionMarker.GlobalPosition - BulletSpawnMarker.GlobalPosition).Normalized() //子弹方向
            });
            AttackCoolDownTimer.Start();
        }
    }
}
