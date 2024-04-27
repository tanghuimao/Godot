using Godot;
using System;
using ClassicRoguelikeCourse.Entites.Characters;
using ClassicRoguelikeCourse.Resources.CharacterData.EnemyData;

/// <summary>
/// 敌人
/// </summary>
public partial class Enemy : Character
{
    public override void Initialize()
    {
        base.Initialize();
        // 注册事件
        _mapManager.Initialized += OnInitialized;
        
        // GD.Print("----------------------------------");
        // GD.Print($"名称: {CharacterData.Name}");
        // GD.Print($"视野: {CharacterData.Sight}");
        // GD.Print($"力量: {CharacterData.Strength}");
        // GD.Print($"体质: {CharacterData.Constitution}");
        // GD.Print($"敏捷: {CharacterData.Agility}");
        // GD.Print($"生命值: {CharacterData.Health}");
        // GD.Print($"最大生命值: {CharacterData.MaxHealth}");
        // GD.Print($"攻击力: {CharacterData.Attack}");
        // GD.Print($"防御力: {CharacterData.Defend}");
        // GD.Print($"闪避: {CharacterData.Dodge}");
        // GD.Print($"暴击: {CharacterData.CriticalChance}");
        // GD.Print("----------------------------------");
    }

    /// <summary>
    /// 地图初始化
    /// </summary>
    /// <param name="playerSpawnCell">玩家生成位置</param>
    /// <param name="getEnemySpawnCell">敌人生成方法</param>
    private void OnInitialized(Vector2I playerSpawnCell , Callable getEnemySpawnCell)
    {
        // 获取敌人生成位置 转换类型
        var enemySpawnCell = getEnemySpawnCell.Call().AsVector2I();
        // 设置位置
        GlobalPosition = enemySpawnCell * _mapManager.MapData.CellSize + _mapManager.MapData.CellSize / 2;
    }
}
