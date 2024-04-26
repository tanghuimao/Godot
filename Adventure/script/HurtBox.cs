using Godot;

namespace Adventure.script;
/// <summary>
/// 受击框
/// </summary>
[GlobalClass]
[GodotClassName("HurtBox")]
public partial class HurtBox : Area2D
{
    //自定义信号
    [Signal]
    public delegate void HurtEventHandler(HitBox hitBox);
}