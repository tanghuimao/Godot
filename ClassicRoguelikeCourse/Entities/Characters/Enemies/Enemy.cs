using System.Collections.Generic;
using ClassicRoguelikeCourse.Component;
using ClassicRoguelikeCourse.Entites;
using ClassicRoguelikeCourse.Entites.Characters;
using ClassicRoguelikeCourse.Managers.AStarGridManager;
using Godot;

/// <summary>
/// 敌人
/// </summary>
public partial class Enemy : Character, ILateUpdateEntity
{
    private AStarGridManager _aStarGridManager;

    private List<ILateUpdateComponent> _lateUpdateComponents = new();
    
    public override void Initialize()
    {
        base.Initialize();
        // 注册事件
        _mapManager.Initialized += OnMapManagerInitialized;
        _aStarGridManager = GetTree().CurrentScene.GetNode<AStarGridManager>("%AStarGridManager");
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
        
        //获取所有子节点
        foreach (var child in GetChildren())
        {
            if (child is not ILateUpdateComponent) continue;
            var component = child as ILateUpdateComponent;
            _lateUpdateComponents.Add(component);
        }
    }

    public void LateUpdate()
    {
        //遍历组件 调用更新方法
        foreach (var component in _lateUpdateComponents)
        {
            component.LateUpdate();
        }
    }

    /// <summary>
    /// 地图初始化
    /// </summary>
    /// <param name="playerSpawnCell">玩家生成位置</param>
    /// <param name="getEnemySpawnCell">敌人生成方法</param>
    private void OnMapManagerInitialized(Vector2I playerSpawnCell , Callable getEnemySpawnCell)
    {
        // 获取敌人生成位置 转换类型
        var enemySpawnCell = getEnemySpawnCell.Call().AsVector2I();
        // 设置位置
        GlobalPosition = enemySpawnCell * _mapManager.MapData.CellSize + _mapManager.MapData.CellSize / 2;
    }
    /// <summary>
    /// 死亡  重写父类方法
    /// </summary>
    /// <param name="character"></param>
    protected override void OnCharacterDead(Character character)
    {
        // 判断是否死亡
        if (character != this || IsDead) return;
        // 设置地图点为可通行
        _aStarGridManager.AStarGrid2D.SetPointSolid(
            (Vector2I) (GlobalPosition - _mapManager.MapData.CellSize / 2) / _mapManager.MapData.CellSize,
            false);
        // 销毁
        QueueFree();
        GD.Print(($"{CharacterData.Name} 被击杀"));
        // 设置死亡
        _isDead = true;
    }
}
