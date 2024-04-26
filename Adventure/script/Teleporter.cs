using Godot;

namespace Adventure.script;
/// <summary>
/// 传送门
/// </summary>
[GlobalClass]
[GodotClassName("Teleporter")]
public partial class Teleporter : Interactable
{
    //导出场景路径
    [Export(PropertyHint.File, "*.tscn")] public string ScenePath;
    //导出入口点
    [Export] public string EntryPoint;

    /// <summary>
    /// 交互 转换场景
    /// </summary>
    public override void Interact()
    {
        base.Interact();
        //转换场景
        GameGlobal.GetInstance().ChangeScene(ScenePath, EntryPoint, null, null);
    }
}