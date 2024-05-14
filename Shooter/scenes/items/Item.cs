using Godot;
using System;
using System.Collections.Generic;
using Vector2 = Godot.Vector2;

/// <summary>
/// 可拾取物品
/// </summary>
public partial class Item : Area2D
{
    private int _rotationSpeed = 4;
    
    private  ItemType _currentType;
    
    private Sprite2D _sprite;
    
    //方向
    public Vector2 Direction;
    //距离
    public int Distance = GD.RandRange(150,250);
    
    
    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _currentType = Enum.Parse<ItemType>(GD.RandRange(0, 4).ToString());

        if (_currentType == ItemType.Health)
        {
            _sprite.Modulate = new Color(0.1f, 0.8f, 0.1f); //绿色
        }
        else if (_currentType == ItemType.Grenade)
        {
            _sprite.Modulate = new Color(0.8f, 0.2f, 0.1f); //红色
        }
        else
        {
            _sprite.Modulate = new Color(0.1f, 0.2f, 0.8f); //蓝色
        }
        //移动后位置
        var targetPosition = Position + Direction * Distance;
        //补间动画
        var tween = CreateTween();
        tween.SetParallel(); //设置并行
        tween.TweenProperty(this, "position", targetPosition, 0.5);
        tween.TweenProperty(this, "scale", new Vector2(1f, 1f), 0.3).From(new Vector2(0, 0));
        //区域检测
        BodyEntered += OnItemBodyEntered;
    }

    public override void _ExitTree()
    {
        //区域检测
        BodyEntered -= OnItemBodyEntered;
    }
    /// <summary>
    /// 区域进入检测
    /// </summary>
    /// <param name="body"></param>
    private void OnItemBodyEntered(Node2D body)
    {
        if (body is not Player player) return;
        player.AddItem(_currentType);
        QueueFree();
    }

    public override void _Process(double delta)
    {
        Rotation += _rotationSpeed * (float)delta;
    }
}
/// <summary>
/// 物品类型
/// </summary>
public enum ItemType
{
    Lazer1,
    Lazer2,
    Lazer3,
    Grenade,
    Health,
}
