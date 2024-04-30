using System.Collections.Generic;
using ClassicRoguelikeCourse.Entities.Characters.Enemies;
using ClassicRoguelikeCourse.Managers.SaveLoadManager;
using Godot;
using Godot.Collections;

namespace ClassicRoguelikeCourse.Managers.EnemyManager;

/// <summary>
/// 敌人生成器
/// </summary>
public partial class EnemyManager : Node, IManager, ILoadable
{
    // 敌人数量
    [Export]
    private short _maxEnemyCount = 30;
    // 敌人场景权重
    [Export]
    private Godot.Collections.Dictionary<PackedScene, float> _enemyScenes = new();
    // boss场景
    [Export]
    private Array<PackedScene> _bossScenes = new();
    
    // 敌人容器
    private Node _enemyContainer;
    
    public Node EnemyContainer => _enemyContainer;
    // 存档管理器
    private  SaveLoadManager.SaveLoadManager _saveLoadManager;
    public void Initialize()
    {
        _enemyContainer = GetTree().CurrentScene.GetNode<Node>("%EnemyContainer");
        _saveLoadManager = GetTree().CurrentScene.GetNode<SaveLoadManager.SaveLoadManager>("%SaveLoadManager");
        if (!InitializeByLoadData())
        {
            //生成敌人
            SpawnEnemy();
            //boss生成
            SpawnBoss();
        }
    }

    public void Update(double delta)
    {
    }
    
    /// <summary>
    /// 存档加载数据
    /// </summary>
    /// <returns></returns>
    public bool InitializeByLoadData()
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
            // 获取敌人
            var enemies = map["Enemies"].AsGodotArray<Godot.Collections.Dictionary<string, Variant>>();
            // 遍历
            for (int j = 0; j < enemies.Count; j++)
            {
                // 获取
                var enemy = enemies[j];
                // 实例化
                var enemyInstance = GD.Load<PackedScene>(enemy["ScenePath"].AsString()).Instantiate<Enemy>();
                // 添加
                _enemyContainer.AddChild(enemyInstance);
                
                var enemyIndex = enemy["Index"].AsInt32();
                // _enemyContainer.MoveChild(enemyInstance, enemyIndex);
                enemyInstance.GlobalPosition = enemy["Position"].AsVector2();
                enemyInstance.Visible = enemy["Visible"].AsBool();
                enemyInstance.Initialize();
            }

            return true;
        }

        return false;
    }
    
    /// <summary>
    /// 敌人生成
    /// </summary>
    private void SpawnEnemy()
    {
        // 生成列表
        var spawnList = GetSpawnList();
        // 生成
        for (int i = 0; i < spawnList.Count; i++)
        {
            var randomIndex = GD.RandRange(0, spawnList.Count - 1);
            var enemyInstance = spawnList[randomIndex].Instantiate<Enemy>();
            _enemyContainer.AddChild(enemyInstance);
            enemyInstance.Initialize();
        }
    }
    
    /// <summary>
    /// boss生成
    /// </summary>
    private void SpawnBoss()
    {
        foreach (var bossScene in _bossScenes)
        {
            var bossInstance = bossScene.Instantiate<Enemy>();
            _enemyContainer.AddChild(bossInstance);
            bossInstance.Initialize();
        }
    }
    
    /// <summary>
    /// 获取生成列表
    /// </summary>
    /// <returns></returns>
    private List<PackedScene> GetSpawnList()
    {
        // 生成列表
        var spawnList = new List<PackedScene>();
        // 总权重
        var totalWeight = GetSpawnWeight();
        //生成敌人数量不一致
        // foreach (var item in _enemyScenes)
        // {
        //     // 随机权重
        //     var randomWeight = (float)GD.RandRange(0, totalWeight);
        //     // 计算生成
        //     while (randomWeight > 0)
        //     {
        //         spawnList.Add(item.Key);
        //         randomWeight -= item.Value;
        //     }
        // }

        foreach (var item in _enemyScenes)
        {
            var number = item.Value / totalWeight * _maxEnemyCount;
            for (int i = 0; i < number; i++)
            {
                spawnList.Add(item.Key);
            }
        }
        return  spawnList;
    }
    
    
    /// <summary>
    /// 获取总权重
    /// </summary>
    /// <returns></returns>
    private float GetSpawnWeight()
    {
        var weight = 0f;

        foreach (var item in _enemyScenes)
        {
            weight += item.Value;
        }
        
        return weight;
    }
}