using Godot;

/// <summary>
/// 角色数据
/// </summary>
public partial class CharacterData : Resource
{
    /// <summary>
    /// 移动速度
    /// </summary>
    [Export] public int Speed { get; set; } = 100;
    /// <summary>
    /// 基础血量
    /// </summary>
    [Export] public int Health { get; set; } = 100;

    /// <summary>
    /// 减少血量
    /// </summary>
    /// <param name="health"></param>
    /// <returns></returns>
    public int SetHealth(int health)
    {
        Health -= health;
        Health = Mathf.Clamp(Health, 0, 100);
        return Health;
    }
}
