using ClassicRoguelikeCourse.Resources.CharacterData.PlayerData;
using Godot;

namespace ClassicRoguelikeCourse.Entities.PickableObjects.Equipments;
/// <summary>
/// 盾牌
/// </summary>
public partial class Shield : Equipment, ILeftHandHoldEquipment
{
    // 最小防御增加量
    [Export]
    private float _minDefendIncrement = 5f;
    // 最大防御增加量
    [Export]
    private float _maxDefendIncrement = 8f;
    // 实际防御增加量
    private float _actualDefendIncrement;
    
    // 最小体质增加量
    [Export]
    private int _minConstitutionIncrement = 1;
    // 最大体质增加量
    [Export]
    private int _maxConstitutionIncrement = 10;
    // 实际体质增加量
    private int _actualConstitutionIncrement;

    public override void Initialize()
    {
        base.Initialize();
        // 计算实际防御增加量
        _actualDefendIncrement = (float)GD.RandRange(_minDefendIncrement, _maxDefendIncrement);
        _actualConstitutionIncrement = GD.RandRange(_minConstitutionIncrement, _maxConstitutionIncrement);
        _description = "防御：" + _actualDefendIncrement.ToString("0.0") + "\n" +
                       "体质：" + _actualConstitutionIncrement;
    }
    
    public override void Equip()
    {
        var playerData = _player.CharacterData as PlayerData;
        // 检查是否已装备
        if (playerData.LeftHandHoldEquipment != null)
        {
            // 卸载装备
            (playerData.LeftHandHoldEquipment as Equipment).UnEquip();
        }
        // 添加效果
        playerData.Defend += _actualDefendIncrement;
        playerData.Constitution += _actualConstitutionIncrement;
        // 改变装备引用
        playerData.LeftHandHoldEquipment = this;
    }

    public override void EquipWithoutEffect()
    {
        var playerData = _player.CharacterData as PlayerData;
        // 检查是否已装备
        if (playerData.LeftHandHoldEquipment != null)
        {
            // 卸载装备
            (playerData.LeftHandHoldEquipment as Equipment).UnEquip();
        }
        // 改变装备引用
        playerData.LeftHandHoldEquipment = this;
    }

    public override void UnEquip()
    {
        var playerData = _player.CharacterData as PlayerData;
        // 装备是否为空或者不是当前装备
        if (playerData.LeftHandHoldEquipment == null || playerData.LeftHandHoldEquipment != this) return;
        // 减去效果
        playerData.Defend -= _actualDefendIncrement;
        playerData.Constitution -= _actualConstitutionIncrement;
        // 改变装备引用
        playerData.LeftHandHoldEquipment = null;
    }
}