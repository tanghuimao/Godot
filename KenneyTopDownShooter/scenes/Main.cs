using Godot;
using System;
/// <summary>
/// 主场景
/// </summary>
public partial class Main : Node2D
{
    private Player _player;
    private BulletManager _bulletManager;
    
    public override void _Ready()
    {
        _player = GetTree().CurrentScene.GetNode<Player>("%Player");
        _bulletManager = GetTree().CurrentScene.GetNode<BulletManager>("%BulletManager");
        // 监听玩家射击事件
        _player.PlayerShootEvent += OnPlayerShootEvent;
    }
    /// <summary>
    /// 玩家射击事件
    /// </summary>
    /// <param name="args"></param>
    private void OnPlayerShootEvent(BulletArgs args)
    {
        _bulletManager.SpawnBullet(args);
    }
}
