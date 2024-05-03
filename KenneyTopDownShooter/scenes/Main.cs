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

        _player.ShootEvent += OnShootEvent;
    }

    private void OnShootEvent(BulletArgs args)
    {
        _bulletManager.SpawnBullet(args);
    }
}
