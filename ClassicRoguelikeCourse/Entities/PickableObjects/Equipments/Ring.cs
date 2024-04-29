using ClassicRoguelikeCourse.Resources.CharacterData.PlayerData;
using Godot;

namespace ClassicRoguelikeCourse.Entities.PickableObjects.Equipments;
/// <summary>
/// 戒指
/// </summary>
public partial class Ring : Equipment, IFingerWearEquipment
{
    // 最小暴击增加量
    [Export]
    private float _minCriticalChanceIncrement = 0.005f;
    // 最大暴击增加量
    [Export]
    private float _maxCriticalChanceIncrement = 0.03f;
    // 实际暴击增加量
    private float _actualCriticalChanceIncrement;
    
    // 最小力量增加量
    [Export]
    private int _minStrengthIncrement = 1;
    // 最大力量增加量
    [Export]
    private int _maxStrengthIncrement = 10;
    // 实际力量增加量
    private int _actualStrengthIncrement;
    
    // 最小敏捷增加量
    [Export]
    private int _minAgilityIncrement = 1;
    // 最大敏捷增加量
    [Export]
    private int _maxAgilityIncrement = 10;
    // 实际敏捷增加量
    private int _actualAgilityIncrement;

    public override void Initialize()
    {
        base.Initialize();
        // 计算实际暴击增加量
        _actualCriticalChanceIncrement = (float)GD.RandRange(_minCriticalChanceIncrement, _maxCriticalChanceIncrement);
        _actualStrengthIncrement = GD.RandRange(_minStrengthIncrement, _maxStrengthIncrement);
        _actualAgilityIncrement = GD.RandRange(_minAgilityIncrement, _maxAgilityIncrement);
        _description = "暴击：" + _actualCriticalChanceIncrement.ToString("0.00") + "%\n" +
                       "力量：" + _actualStrengthIncrement + "\n" +
                       "敏捷：" + _minAgilityIncrement;
    }
    
    public override void Equip()
    {
        var playerData = _player.CharacterData as PlayerData;
        // 检查是否已装备
        if (playerData.FingerWearEquipment != null)
        {
            // 卸载装备
            (playerData.FingerWearEquipment as Equipment).UnEquip();
        }
        // 添加效果
        playerData.CriticalChance += _actualCriticalChanceIncrement;
        playerData.Strength += _actualStrengthIncrement;
        playerData.Agility += _actualAgilityIncrement;
        // 改变装备引用
        playerData.FingerWearEquipment = this;
    }

    public override void EquipWithoutEffect()
    {
        var playerData = _player.CharacterData as PlayerData;
        // 检查是否已装备
        if (playerData.FingerWearEquipment != null)
        {
            // 卸载装备
            (playerData.FingerWearEquipment as Equipment).UnEquip();
        }
        // 改变装备引用
        playerData.FingerWearEquipment = this;
    }

    public override void UnEquip()
    {
        var playerData = _player.CharacterData as PlayerData;
        // 装备是否为空或者不是当前装备
        if (playerData.FingerWearEquipment == null || playerData.FingerWearEquipment != this) return;
        // 减去效果
        playerData.CriticalChance -= _actualCriticalChanceIncrement;
        playerData.Strength -= _actualStrengthIncrement;
        playerData.Agility -= _actualAgilityIncrement;
        // 改变装备引用
        playerData.FingerWearEquipment = null;
    }
}