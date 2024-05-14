using Godot;
using System;

/// <summary>
/// 马桶
/// </summary>
public partial class Toilet : Container
{
    public override void _Ready()
    {
        base._Ready();
    }

    public override void Hit()
    {
        if (IsOpen) return;
        //关闭
        LidSprite.Visible = false;
        var marker = SpawnPosition.GetNode<Marker2D>("Marker2D");
        OnOpenEvent(new OpenParam()
        {
            Position = marker.GlobalPosition,
            Direction = CurrentDirection //当前方向
        });
        IsOpen = true;
    }
}