using Godot;

/// <summary>
/// 角色数据
/// </summary>
public partial class CharacterData : Resource
{
    /// <summary>
    /// 移动速度
    /// </summary>
    [Export] public int BaseSpeed { get; set; } = 100;
    /// <summary>
    /// 基础血量
    /// </summary>
    [Export] public int Health { get; set; } = 100;
}
