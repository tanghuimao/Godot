using Godot;
using System;
using KenneyTopDownShooter.common;

/// <summary>
/// 主场景
/// </summary>
public partial class Main : Node2D
{
    private Player _player;
    private BulletManager _bulletManager;
    
    public override void _Ready()
    {
        // 随机数
        GD.Randomize();
        _player = GetTree().CurrentScene.GetNode<Player>("%Player");
        _bulletManager = GetTree().CurrentScene.GetNode<BulletManager>("%BulletManager");
        // 射击事件
        GlobalEvent.BulletFiredEvent += OnBulletFiredEvent;
    }
    /// <summary>
    /// 射击事件
    /// </summary>
    /// <param name="args"></param>
    private void OnBulletFiredEvent(BulletSpawn args)
    {
        _bulletManager.SpawnBullet(args);
    }
}
