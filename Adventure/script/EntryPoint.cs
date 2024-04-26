using Godot;

namespace Adventure.script;
/// <summary>
/// 启动点  入口标记点
/// </summary>
[GlobalClass]
[GodotClassName("EntryPoint")]
public partial class EntryPoint : Marker2D
{
    [Export] public Direction Direction = Direction.Left;
    public override void _Ready()
    {
        // 添加到组
        AddToGroup("EntryPoints");       
    }
}