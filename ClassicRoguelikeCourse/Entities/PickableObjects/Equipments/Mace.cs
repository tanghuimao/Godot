using ClassicRoguelikeCourse.Resources.CharacterData.PlayerData;
using Godot;

namespace ClassicRoguelikeCourse.Entities.PickableObjects.Equipments;
/// <summary>
/// 战锤
/// </summary>
public partial class Mace : Equipment, IRightHandHoldEquipment
{
    // 实际攻击增加量
    [Export]
    private float _actualAttackIncrement = 18f;
    
    // 最小力量增加量
    [Export]
    private int _minStrengthIncrement = 1;
    // 最大力量增加量
    [Export]
    private int _maxStrengthIncrement = 15;
    // 实际力量增加量
    private int _actualStrengthIncrement;

    public override void Initialize()
    {
        base.Initialize();
        _actualStrengthIncrement = GD.RandRange(_minStrengthIncrement, _maxStrengthIncrement);
        _description = "攻击：" + _actualAttackIncrement.ToString("0.0") + "\n" +
                       "力量：" + _actualStrengthIncrement;
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
        playerData.Strength += _actualStrengthIncrement;
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
        playerData.Strength -= _actualStrengthIncrement;
        // 改变装备引用
        playerData.RightHandHoldEquipment = null;
        base.UnEquip();
    }
}