using ClassicRoguelikeCourse.Resources.CharacterData.PlayerData;
using Godot;

namespace ClassicRoguelikeCourse.Entities.PickableObjects.Equipments;
/// <summary>
/// 项链
/// </summary>
public partial class Necklace : Equipment, INeckWearEquipment
{
    // 最小闪避增加量
    [Export]
    private float _minDodgeIncrement = 0.1f;
    // 最大闪避增加量
    [Export]
    private float _maxDodgeIncrement = 0.5f;
    // 实际闪避增加量
    private float _actualDodgeIncrement;
    
    // 最小力量增加量
    [Export]
    private int _minStrengthIncrement = 1;
    // 最大力量增加量
    [Export]
    private int _maxStrengthIncrement = 8;
    // 实际力量增加量
    private int _actualStrengthIncrement;
    
    // 最小体质增加量
    [Export]
    private int _minConstitutionIncrement = 1;
    // 最大体质增加量
    [Export]
    private int _maxConstitutionIncrement = 15;
    // 实际体质增加量
    private int _actualConstitutionIncrement;

    public override void Initialize()
    {
        base.Initialize();
        // 计算实际闪避增加量
        _actualDodgeIncrement = (float)GD.RandRange(_minDodgeIncrement, _maxDodgeIncrement);
        _actualStrengthIncrement = GD.RandRange(_minStrengthIncrement, _maxStrengthIncrement);
        _actualConstitutionIncrement = GD.RandRange(_minConstitutionIncrement, _maxConstitutionIncrement);
        _description = "闪避：" + _actualDodgeIncrement.ToString("0.00") + "%\n" +
                       "力量：" + _actualStrengthIncrement + "\n" +
                       "体质：" + _actualConstitutionIncrement;
    }
    
    public override void Equip()
    {
        var playerData = _player.CharacterData as PlayerData;
        // 检查是否已装备
        if (playerData.NeckWearEquipment != null)
        {
            // 卸载装备
            (playerData.NeckWearEquipment as Equipment).UnEquip();
        }
        // 添加效果
        playerData.Dodge += _actualDodgeIncrement;
        playerData.Strength += _actualStrengthIncrement;
        playerData.Constitution += _actualConstitutionIncrement;
        // 改变装备引用
        playerData.NeckWearEquipment = this;
        base.Equip();
        _player.ChangeConstitution(_actualConstitutionIncrement);
    }

    public override void EquipWithoutEffect()
    {
        var playerData = _player.CharacterData as PlayerData;
        // 检查是否已装备
        if (playerData.NeckWearEquipment != null)
        {
            // 卸载装备
            (playerData.NeckWearEquipment as Equipment).UnEquip();
        }
        // 改变装备引用
        playerData.NeckWearEquipment = this;
    }

    public override void UnEquip()
    {
        var playerData = _player.CharacterData as PlayerData;
        // 装备是否为空或者不是当前装备
        if (playerData.NeckWearEquipment == null || playerData.NeckWearEquipment != this) return;
        // 减去效果
        playerData.Dodge -= _actualDodgeIncrement;
        playerData.Strength -= _actualStrengthIncrement;
        playerData.Constitution -= _actualConstitutionIncrement;
        // 改变装备引用
        playerData.NeckWearEquipment = null;
        base.UnEquip();
        _player.ChangeConstitution(-_actualConstitutionIncrement);
    }
}