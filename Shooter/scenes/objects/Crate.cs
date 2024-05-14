using Godot;
using System;
/// <summary>
/// 箱子
/// </summary>
public partial class Crate : Container
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
        for (int i = 0; i < 5; i++)
        {
            var childIndex  = GD.RandRange(0,SpawnPosition.GetChildCount() -1);
            var marker = SpawnPosition.GetChild(childIndex) as Marker2D;
            OnOpenEvent(new OpenParam()
            {
                Position = marker.GlobalPosition,
                Direction = CurrentDirection //当前方向
            });
        }
        IsOpen = true;

    }
}
