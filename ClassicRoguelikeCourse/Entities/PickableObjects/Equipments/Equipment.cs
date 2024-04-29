using ClassicRoguelikeCourse.Resources.CharacterData.PlayerData;

namespace ClassicRoguelikeCourse.Entities.PickableObjects.Equipments;

/// <summary>
/// 装备父类接口
/// </summary>
public partial class Equipment : PickableObject
{

    public bool IsEquipped
    {
        // 判断是否装备
        get
        {
            var playerData = _player.CharacterData as PlayerData;

            if (playerData.LeftHandHoldEquipment == this ||
                playerData.RightHandHoldEquipment == this ||
                playerData.BodyWearEquipment == this ||
                playerData.NeckWearEquipment == this ||
                playerData.FingerWearEquipment == this)
            {
                return true;
            }

            return false;
        }
    }
    
    /// <summary>
    /// 装备
    /// </summary>
    public virtual void Equip()
    {
        throw new System.Exception("不能直接调用基类方法");
    }

    /// <summary>
    /// 装备不产生效果
    /// </summary>
    public virtual void EquipWithoutEffect()
    {
        throw new System.Exception("不能直接调用基类方法");
    }

    /// <summary>
    /// 卸下
    /// </summary>
    public virtual void UnEquip()
    {
        throw new System.Exception("不能直接调用基类方法");
    }
}