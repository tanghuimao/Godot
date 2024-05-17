using Godot;
using System;
/// <summary>
/// 地图
/// </summary>
public partial class World : Node2D
{
    //地图瓦片
    private TileMap _tileMap;
    //相机
    private Camera2D _camera2D;
    //玩家
    private Player _player;
    //生命值
    private HealthControl _healthControl;
    //背包
    private InventoryControl _inventoryControl;
    public override void _Ready()
    {
        _tileMap = GetNode<TileMap>("TileMap");
        _camera2D = GetNode<Camera2D>("%Camera2D");
        _player = GetNode<Player>("Player");
        _healthControl = GetNode<HealthControl>("%HeathContainer");
        _inventoryControl = GetNode<InventoryControl>("%InventoryControl");
        _inventoryControl.OpenOrCloseEvent += OnOpenOrCloseEvent;
        //设置初始值
        _healthControl.SetHealth(_player.MaxHealth);
        _healthControl.UpdateHealth(_player.MaxHealth);
        //注册事件
        _player.HealthChangedEvent += OnPlayerHealthChangedEvent;
        
        //获取地图瓦片
        var mapRect = _tileMap.GetUsedRect();
        //获取地图瓦片大小
        var tileSize = _tileMap.TileSet.TileSize;
        
        //设置相机显现位置
        _camera2D.LimitTop = mapRect.Position.Y * tileSize.Y;
        _camera2D.LimitRight = mapRect.End.X * tileSize.X;
        _camera2D.LimitBottom = mapRect.End.Y * tileSize.Y;
        _camera2D.LimitLeft = mapRect.Position.X * tileSize.X;
        
        //重置
        _camera2D.ResetSmoothing();
        //强制刷新
        _camera2D.ForceUpdateScroll();
    }
    /// <summary>
    /// 背包打开或者关闭
    /// </summary>
    /// <param name="obj"></param>
    private void OnOpenOrCloseEvent(bool isOpen)
    {
        GetTree().Paused = isOpen;
    }

    /// <summary>
    /// 玩家生命值改变事件
    /// </summary>
    /// <param name="currentHealth"></param>
    private void OnPlayerHealthChangedEvent(int currentHealth)
    {
        _healthControl.UpdateHealth(currentHealth);
    }
}
