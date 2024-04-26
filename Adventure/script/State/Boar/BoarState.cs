using System;
using Godot;

namespace Adventure.script.State.Boar;

/// <summary>
/// 玩家状态 父类
/// </summary>
public partial class BoarState : State
{
    public script.Boar Boar;
    //动画名称
    [Export] protected String AnimationName;
    
    public override void Enter()
    {
        Boar?.AnimationPlayer.Play(AnimationName);
    }
    
}