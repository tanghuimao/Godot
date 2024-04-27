using ClassicRoguelikeCourse.Resources.CharacterData.PlayerData;
using Godot;

namespace ClassicRoguelikeCourse.Entites.Characters.Player;
/// <summary>
/// 玩家
/// </summary>
public partial class Player : Character
{
    /// <summary>
    /// 重写父类方法
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        _mapManager.Initialized += OnInitialized;
        var data = CharacterData as PlayerData;
        GD.Print("----------------------------------");
        GD.Print($"名称: {CharacterData.Name}");
        GD.Print($"等级: {data.Level}");
        GD.Print($"经验: {data.Experience}");
        GD.Print($"经验增长率: {data.LevelUpExperienceThresholdIncrementRate}");
        GD.Print($"升级属性点: {data.BaseAttributePointsGainPerLevelUp}");
        GD.Print($"视野: {CharacterData.Sight}");
        GD.Print($"力量: {CharacterData.Strength}");
        GD.Print($"体质: {CharacterData.Constitution}");
        GD.Print($"敏捷: {CharacterData.Agility}");
        GD.Print($"生命值: {CharacterData.Health}");
        GD.Print($"最大生命值: {CharacterData.MaxHealth}");
        GD.Print($"攻击力: {CharacterData.Attack}");
        GD.Print($"防御力: {CharacterData.Defend}");
        GD.Print($"闪避: {CharacterData.Dodge}");
        GD.Print($"暴击: {CharacterData.CriticalChance}");
        GD.Print("----------------------------------");
    }

    /// <summary>
    /// 地图初始化完成 事件
    /// </summary>
    /// <param name="playerSpawnCell">玩家生成位置</param>
    /// <param name="_">敌人生成方法, 不使用</param>
    private void OnInitialized(Vector2I playerSpawnCell, Callable _)
    {
        // 设置玩家初始位置
        GlobalPosition = playerSpawnCell * _mapManager.MapData.CellSize + _mapManager.MapData.CellSize / 2;
    }
}