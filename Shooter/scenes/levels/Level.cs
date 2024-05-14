using Godot;
using System;
using Shooter.scenes.projectiles;

public partial class Level : Node2D
{
    //激光
    private PackedScene _laser = GD.Load<PackedScene>("res://scenes/projectiles/Laser.tscn");
    //榴弹
    private PackedScene _grenade = GD.Load<PackedScene>("res://scenes/projectiles/Grenade.tscn");
    //物品
    private PackedScene _item = GD.Load<PackedScene>("res://scenes/items/Item.tscn");
    //投射物
    private Node2D _projectiles;
    //物品
    private Node2D _items;

    //玩家
    public Player Player;
    //相机
    public Camera2D Camera;
    
    //玩家状态ui
    private UI _ui;
    

    public override void _Ready()
    {
        //投射物
        _projectiles = GetNode<Node2D>("Projectiles");
        _items = GetNode<Node2D>("Items");
        //玩家
        Player = GetNode<Player>("Player");
        Camera = GetNode<Camera2D>("Player/Camera2D");
        Player.ShootLaserEvent += OnPlayerShootLaserEvent;
        Player.ShootGrenadeEvent += OnPlayerShootGrenadeEvent;
        _ui = GetNode<UI>("UI");
        
        //获取分组对象
        foreach (var node in GetTree().GetNodesInGroup("Container"))
        {
            if (node is Container container)
            {
                //监听事件
                container.OpenEvent += OnContainerOpenEvent;
            }
        }
        
        foreach (var node in GetTree().GetNodesInGroup("Scout"))
        {
            if (node is Scout scout)
            {
                scout.ShootLaserEvent += OnScoutShootLaserEvent;
            }
        }
    }



    /// <summary>
    /// 容器被打开
    /// </summary>
    /// <param name="param"></param>
    private void OnContainerOpenEvent(OpenParam param)
    {
        var item = _item.Instantiate<Item>();
        item.Position = param.Position;
        item.Direction = param.Direction;
        //延迟调用
        Callable.From(() =>  _items.AddChild(item)).CallDeferred();
    }

    public override void _ExitTree()
    {
        Player.ShootLaserEvent -= OnPlayerShootLaserEvent;
        Player.ShootGrenadeEvent -= OnPlayerShootGrenadeEvent;
    }


    /// <summary>
    /// 玩家发射激光
    /// </summary>
    private void OnPlayerShootLaserEvent(ProjectParam param)
    {
        CreateLaser(param);
    }

    /// <summary>
    /// 创建激光
    /// </summary>
    /// <param name="param"></param>
    private void CreateLaser(ProjectParam param)
    {
        var laser = _laser.Instantiate<Laser>();
        _projectiles.AddChild(laser);
        laser.SetProjectParam(param);
    }

    /// <summary>
    /// 玩家发射手榴弹
    /// </summary>
    private void OnPlayerShootGrenadeEvent(ProjectParam param)
    {
        var grenade = _grenade.Instantiate<Grenade>();
        _projectiles.AddChild(grenade);
        grenade.SetProjectParam(param);
    }
    
    private void OnScoutShootLaserEvent(ProjectParam param)
    {
        CreateLaser(param);
    }
}
