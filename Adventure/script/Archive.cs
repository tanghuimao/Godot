using Adventure.script.Component;
using Godot;

namespace Adventure.script;
/// <summary>
/// 存档文件
/// </summary>
public class Archive
{
    public PlayerData PlayerData;
    public WorldState WorldState;
}

/// <summary>
/// 玩家数据
/// </summary>
public class PlayerData
{
    //基础生命值
    public double BaseHealth;
    //当前生命值
    public double CurrentHealth;
    //最大能量
    public double MaxEnergy;
    //当前能量
    public double CurrentEnergy;
    //位置
    public Vector2 GlobalPosition;
    //方向
    public Direction DefaultDirection;
    //场景
    public string CurrentScene;
}

