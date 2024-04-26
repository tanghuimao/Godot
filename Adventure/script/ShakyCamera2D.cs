using System;
using Godot;

namespace Adventure.script;

/// <summary>
/// 相机 震屏和顿帧
/// </summary>
public partial class ShakyCamera2D : Camera2D
{
    //震屏 强度
    [Export] public float ShakeStrength = 0f;
    //恢复速度
    [Export] public float RecoverDuration = 16f;


    public override void _Ready()
    {
        //监听相机震动
        GameGlobal.CameraShark += v => ShakeStrength = v;
    }

    public override void _PhysicsProcess(double delta)
    {
        //震屏  设置相机偏移量
        Offset = new Vector2((float)GD.RandRange(-ShakeStrength, ShakeStrength), 
            (float)GD.RandRange(-ShakeStrength, ShakeStrength));
        //恢复相机
        ShakeStrength = (float)Mathf.MoveToward(ShakeStrength, 0, RecoverDuration * delta);

    }
}