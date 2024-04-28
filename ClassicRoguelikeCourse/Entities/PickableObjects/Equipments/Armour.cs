using ClassicRoguelikeCourse.Resources.CharacterData.PlayerData;
using Godot;

namespace ClassicRoguelikeCourse.Entities.PickableObjects.Equipments;
/// <summary>
/// 盔甲
/// </summary>
public partial class Armour : Equipment, IBodyWearEquipment
{
    // 最小防御增加量
    [Export]
    private float _minDefendIncrement = 3f;
    // 最大防御增加量
    [Export]
    private float _maxDefendIncrement = 10f;
    // 实际防御增加量
    private float _actualDefendIncrement;
    
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
        if (playerData.BodyWearEquipment != null)
        {
            // 卸载装备
            (playerData.BodyWearEquipment as Equipment).UnEquip();
        }
        // 添加效果
        playerData.Defend += _actualDefendIncrement;
        playerData.Constitution += _actualConstitutionIncrement;
        // 改变装备引用
        playerData.BodyWearEquipment = this;
        base.Equip();
        _player.ChangeConstitution(_actualConstitutionIncrement);
    }

    public override void EquipWithoutEffect()
    {
        var playerData = _player.CharacterData as PlayerData;
        // 检查是否已装备
        if (playerData.BodyWearEquipment != null)
        {
            // 卸载装备
            (playerData.BodyWearEquipment as Equipment).UnEquip();
        }
        // 改变装备引用
        playerData.BodyWearEquipment = this;
    }

    public override void UnEquip()
    {
        var playerData = _player.CharacterData as PlayerData;
        // 装备是否为空或者不是当前装备
        if (playerData.BodyWearEquipment == null || playerData.BodyWearEquipment != this) return;
        // 减去效果
        playerData.Defend -= _actualDefendIncrement;
        playerData.Constitution -= _actualConstitutionIncrement;
        // 改变装备引用
        playerData.BodyWearEquipment = null;
        base.UnEquip();
        _player.ChangeConstitution(-_actualConstitutionIncrement);
    }
}