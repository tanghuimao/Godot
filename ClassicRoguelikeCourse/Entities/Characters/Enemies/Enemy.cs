using System;
using System.Collections.Generic;
using ClassicRoguelikeCourse.Component;
using ClassicRoguelikeCourse.Entites;
using ClassicRoguelikeCourse.Managers.AStarGridManager;
using ClassicRoguelikeCourse.Resources.CharacterData.EnemyData;
using Godot;

namespace ClassicRoguelikeCourse.Entities.Characters.Enemies;

/// <summary>
/// 敌人
/// </summary>
public partial class Enemy : Character, ILateUpdateEntity
{
    // 骷髅王死亡事件
    public event Action SkeletonKingDead;
    
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

    public override Godot.Collections.Dictionary<string, Variant> GetDataForSave()
    {
        // 获取父类数据
        var enemyDataForSave = base.GetDataForSave();
        // 获取数据
        var enemyData = _characterData as EnemyData;
        // 死亡掉落
        var deathDropPickableObjects = new Godot.Collections.Dictionary<string, float>();
        // 遍历死亡掉落
        foreach (var deathDropPickableObject in enemyData.DeathDropPickableObjects)
        {
            // 添加数据 ResourcePath 掉落概率
            deathDropPickableObjects.Add(deathDropPickableObject.Key.ResourcePath, deathDropPickableObject.Value);
        }
        // 添加数据
        enemyDataForSave.Add("DeathDropPickableObjects", deathDropPickableObjects);
        enemyDataForSave.Add("DeathDropExperience", enemyData.DeathDropExperience);
        enemyDataForSave.Add("Visible", Visible);
        enemyDataForSave.Add("ScenePath", SceneFilePath);
        enemyDataForSave.Add("Index", GetIndex());
        enemyDataForSave.Add("Position", GlobalPosition);

        return enemyDataForSave;

    }

    /// <summary>
    /// 存档加载数据
    /// </summary>
    /// <returns></returns>
    private new bool InitializeByLoadData()
    {
        if (_saveLoadManager.LoadedData == null ||
            _saveLoadManager.LoadedData.Count == 0 ||
            !_saveLoadManager.LoadedData.ContainsKey("Maps")) return false;

        var maps = _saveLoadManager.LoadedData["Maps"].AsGodotArray<Godot.Collections.Dictionary<string, Variant>>();
        for (int i = 0; i < maps.Count; i++)
        {
            var map = maps[i];
            //名称 和 当前保存场景相同
            if (map["SceneName"].AsString() != GetTree().CurrentScene.Name) continue;
            //获取存档敌人集合
            var enemies = map["Enemies"].AsGodotArray<Godot.Collections.Dictionary<string, Variant>>();
                
            for (int j = 0; j < enemies.Count; j++)
            {
                var enemy = enemies[j];
                if (enemy["Index"].AsInt32() == GetIndex())
                {
                    // 死亡掉落经验
                    (_characterData as EnemyData).DeathDropExperience = enemy["DeathDropExperience"].AsInt32();
                    // 死亡掉落道具
                    foreach (var deathDropPickableObject in enemy["DeathDropPickableObjects"].AsGodotDictionary<string,float>())
                    {
                        (_characterData as EnemyData).DeathDropPickableObjects[
                            GD.Load<PackedScene>(deathDropPickableObject.Key)
                        ] = deathDropPickableObject.Value;
                    }
                    return true;
                }
            }
                
        }

        return false;
    }

    /// <summary>
    /// 地图初始化
    /// </summary>
    /// <param name="playerSpawnCell">玩家生成位置</param>
    /// <param name="getEnemySpawnCell">敌人生成方法</param>
    private void OnMapManagerInitialized(Vector2I playerSpawnCell , Callable getEnemySpawnCell)
    {
        if (InitializeByLoadData()) return;
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
        GD.Print(($"{_characterData.Name} 被击杀"));
        if (string.Equals(_characterData.Name, "骷髅王"))
        {
            SkeletonKingDead.Invoke();
        }
        // 设置死亡
        _isDead = true;
    }
}