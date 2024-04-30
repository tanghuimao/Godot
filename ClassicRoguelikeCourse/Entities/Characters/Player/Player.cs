using System.Linq;
using ClassicRoguelikeCourse.Entities.PickableObjects.Equipments;
using ClassicRoguelikeCourse.Entities.PickableObjects.Items;
using ClassicRoguelikeCourse.Resources.CharacterData;
using ClassicRoguelikeCourse.Resources.CharacterData.PlayerData;
using Godot;
using Godot.Collections;

namespace ClassicRoguelikeCourse.Entities.Characters.Player;
/// <summary>
/// 玩家
/// </summary>
public partial class Player : Entities.Characters.Character
{
    private Sprite2D _deathSprite2D;
    
    /// <summary>
    /// 重写父类方法
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        _deathSprite2D = GetNode<Sprite2D>("DeathSprite2D");
        _deathSprite2D.Visible = false;
        _mapManager.Initialized += OnInitialized;
        // var data = CharacterData as PlayerData;
        // GD.Print("----------------------------------");
        // GD.Print($"名称: {CharacterData.Name}");
        // GD.Print($"等级: {data.Level}");
        // GD.Print($"经验: {data.Experience}");
        // GD.Print($"经验增长率: {data.LevelUpExperienceThresholdIncrementRate}");
        // GD.Print($"升级属性点: {data.BaseAttributePointsGainPerLevelUp}");
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
    /// 加载数据
    /// </summary>
    /// <returns></returns>
    private new bool InitializeByLoadData()
    {
        if (_saveLoadManager.LoadedData == null ||
            _saveLoadManager.LoadedData.Count == 0 ||
            !_saveLoadManager.LoadedData.ContainsKey("Maps") ||
            !_saveLoadManager.LoadedData.ContainsKey("Player") ) return false;
        // 玩家数据
        var player = _saveLoadManager.LoadedData["Player"].AsGodotDictionary<string, Variant>();
        (_characterData as PlayerData).Level = (short)player["Level"].AsInt32();
        (_characterData as PlayerData).Experience = (short)player["Experience"].AsInt32();
        // 背包数据
        var inventoryPickableObjects = player["InventoryPickableObjects"].AsGodotArray<string>();
        var inventoryEquipState = player["InventoryEquipState"].AsGodotArray<bool>();
        //遍历背包数据
        for (var i = 0; i < inventoryPickableObjects.Count; i++)
        {   
            // 获取物品场景路径
            var pickableObjectScenePath = inventoryPickableObjects[i];
            // 获取物品是否装备状态
            var pickableObjectEquipState = inventoryEquipState[i];
            // 实例化物品
            var pickableObjectInstance = GD.Load<PackedScene>(pickableObjectScenePath).Instantiate<PickableObject>();
            // 隐藏物品
            pickableObjectInstance.Visible = false;
            // 添加物品到场景
            GetTree().CurrentScene.AddChild(pickableObjectInstance);
            // 初始化物品
            pickableObjectInstance.Initialize();
            // 移除物品
            GetTree().CurrentScene.RemoveChild(pickableObjectInstance);
            // 添加物品到背包
            (_characterData as PlayerData).Inventory.Add(pickableObjectInstance);
            // 判断是否装备
            if (pickableObjectEquipState)
            {
                (pickableObjectInstance as Equipment).EquipWithoutEffect();
                
            }
        }
        
        var maps = _saveLoadManager.LoadedData["Maps"].AsGodotArray<Godot.Collections.Dictionary<string, Variant>>();
        for (int i = 0; i < maps.Count; i++)
        {
            var map = maps[i];
            //名称 和 当前保存场景相同
            if (map["SceneName"].AsString() != GetTree().CurrentScene.Name) continue;
            //获取存档敌人集合
            GlobalPosition = map["PlayerLastPosition"].AsVector2I();

            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 地图初始化完成 事件
    /// </summary>
    /// <param name="playerSpawnCell">玩家生成位置</param>
    /// <param name="_">敌人生成方法, 不使用</param>
    private void OnInitialized(Vector2I playerSpawnCell, Callable _)
    {
        if (InitializeByLoadData()) return;
        
        // 设置玩家初始位置
        GlobalPosition = playerSpawnCell * _mapManager.MapData.CellSize + _mapManager.MapData.CellSize / 2;
    }
    /// <summary>
    /// 获取保存数据
    /// </summary>
    /// <returns></returns>
    public override Dictionary<string, Variant> GetDataForSave()
    {
        //父类对象
        var playerDataForSave = base.GetDataForSave();
        //玩家数据
        var playerData = CharacterData as PlayerData;
        //添加数据
        playerDataForSave.Add("Level", playerData.Level);
        playerDataForSave.Add("Experience", playerData.Experience);
        //背包数据
        var inventoryPickableObjects = new Array<string>();
        var inventoryEquipState = new Array<bool>();
        
        foreach (var item in playerData.Inventory)
        {
            //添加物品场景文件路径
            inventoryPickableObjects.Add(item.SceneFilePath);
            //添加装备状态
            inventoryEquipState.Add(item is Equipment && (item as Equipment).IsEquipped);
        }
        playerDataForSave.Add("InventoryPickableObjects", inventoryPickableObjects);
        playerDataForSave.Add("InventoryEquipState", inventoryEquipState);
        
        return  playerDataForSave;
    }

    /// <summary>
    /// 死亡  重写父类方法
    /// </summary>
    /// <param name="character"></param>
    protected override async void OnCharacterDead(Entities.Characters.Character character)
    {
        // 判断是否死亡
        if (character != this || IsDead) return;
        // 设置死亡
        _isDead = true;
        // 显示死亡动画
        _deathSprite2D.Visible = true;
        
        //获取玩家背包中首个死亡物品
        var playerData = CharacterData as PlayerData;
        var firstDeadEffectItem = playerData.Inventory.FirstOrDefault(item => item is IDeadEffectItem);
        if (firstDeadEffectItem != null)
        {
            //复活玩家
            GD.Print($"玩家被击败，等待若干秒复活！");
            await (firstDeadEffectItem as IDeadEffectItem).DoDeadEffect();
            _deathSprite2D.Visible = false;
            _isDead = false;
            return;
        }
        GD.Print(($"{CharacterData.Name} 被击杀!"));
    }
    /// <summary>
    /// 添加经验
    /// </summary>
    /// <param name="exp">经验</param>
    public void AddExperience(float exp)
    {
        var characterData = CharacterData as PlayerData;
        characterData.Experience += exp;
    }
}