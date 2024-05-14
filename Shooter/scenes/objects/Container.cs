using Godot;
using System;

/// <summary>
/// 容器父类对象
/// </summary>
public partial class Container : StaticBody2D
{
    //容器打开事件
    public event Action<OpenParam> OpenEvent;
    //当前方向
    public Vector2 CurrentDirection;
    //是否打开
    public bool IsOpen = false;

    public Node2D SpawnPosition;
    public Sprite2D LidSprite;
    
    public override void _Ready()
    {
        //添加到容器组
        // GetTree().CurrentScene.AddToGroup("Container");
        // 旋转得到当前方向
        CurrentDirection = Vector2.Down.Rotated(Rotation);
        SpawnPosition = GetNode<Node2D>("SpawnPosition");
        LidSprite = GetNode<Sprite2D>("LidSprite");
    }

    public virtual void Hit()
    {
    }
    
    protected void OnOpenEvent(OpenParam param)
    {
        OpenEvent?.Invoke(param);
    }
}

/// <summary>
/// 打开参数
/// </summary>
public class OpenParam
{
    //位置
    public Vector2 Position;
    //方向
    public Vector2 Direction;
}