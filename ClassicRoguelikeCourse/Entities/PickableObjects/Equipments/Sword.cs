using ClassicRoguelikeCourse.Resources.CharacterData.PlayerData;
using Godot;

namespace ClassicRoguelikeCourse.Entities.PickableObjects.Equipments;
/// <summary>
/// 长剑
/// </summary>
public partial class Sword : Equipment, IRightHandHoldEquipment
{
    // 最小攻击增加量
    [Export]
    private float _minAttackIncrement = 3f;
    // 最大攻击增加量
    [Export]
    private float _maxAttackIncrement = 20f;
    // 实际攻击增加量
    private float _actualAttackIncrement;
    
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
        // 计算实际攻击增加量
        _actualAttackIncrement = (float)GD.RandRange(_minAttackIncrement, _maxAttackIncrement);
        _actualAgilityIncrement = GD.RandRange(_minAgilityIncrement, _maxAgilityIncrement);
        _description = "攻击：" + _actualAttackIncrement.ToString("0.0") + "\n" +
                       "敏捷：" + _actualAgilityIncrement;
    }

    public override void Equip()
    {
        var playerData = _player.CharacterData as PlayerData;
        // 检查是否已装备
        if (playerData.RightHandHoldEquipment != null)
        {
            // 卸载装备
            (playerData.RightHandHoldEquipment as Equipment).UnEquip();
        }
        // 添加效果
        playerData.Attack += _actualAttackIncrement;
        playerData.Agility += _actualAgilityIncrement;
        // 改变装备引用
        playerData.RightHandHoldEquipment = this;
        base.Equip();
    }

    public override void EquipWithoutEffect()
    {
        var playerData = _player.CharacterData as PlayerData;
        // 检查是否已装备
        if (playerData.RightHandHoldEquipment != null)
        {
            // 卸载装备
            (playerData.RightHandHoldEquipment as Equipment).UnEquip();
        }
        // 改变装备引用
        playerData.RightHandHoldEquipment = this;
    }

    public override void UnEquip()
    {
        var playerData = _player.CharacterData as PlayerData;
        // 装备是否为空或者不是当前装备
        if (playerData.RightHandHoldEquipment == null || playerData.RightHandHoldEquipment != this) return;
        // 减去效果
        playerData.Attack -= _actualAttackIncrement;
        playerData.Agility -= _actualAgilityIncrement;
        // 改变装备引用
        playerData.RightHandHoldEquipment = null;
        base.UnEquip();
    }
}