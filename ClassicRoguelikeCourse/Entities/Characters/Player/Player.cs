using System.Linq;
using ClassicRoguelikeCourse.Entities.PickableObjects.Items;
using ClassicRoguelikeCourse.Resources.CharacterData.PlayerData;
using Godot;

namespace ClassicRoguelikeCourse.Entites.Characters.Player;
/// <summary>
/// 玩家
/// </summary>
public partial class Player : Character
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
    /// 地图初始化完成 事件
    /// </summary>
    /// <param name="playerSpawnCell">玩家生成位置</param>
    /// <param name="_">敌人生成方法, 不使用</param>
    private void OnInitialized(Vector2I playerSpawnCell, Callable _)
    {
        // 设置玩家初始位置
        GlobalPosition = playerSpawnCell * _mapManager.MapData.CellSize + _mapManager.MapData.CellSize / 2;
    }
    
    /// <summary>
    /// 死亡  重写父类方法
    /// </summary>
    /// <param name="character"></param>
    protected override async void OnCharacterDead(Character character)
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