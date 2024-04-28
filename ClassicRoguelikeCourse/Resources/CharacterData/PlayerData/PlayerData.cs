using ClassicRoguelikeCourse.Entities.PickableObjects.Equipments;
using Godot;
using Godot.Collections;

namespace ClassicRoguelikeCourse.Resources.CharacterData.PlayerData;
/// <summary>
/// 玩家数据
/// </summary>
public partial class PlayerData : CharacterData
{
    //等级
    public short Level = 1;
    //经验
    public float Experience = 0f;
    //升级经验阈值
    public float BaseLevelUpExperienceThreshold = 10f;
    //升级经验阈值递增率
    public float LevelUpExperienceThresholdIncrementRate = 0.2f;
    //基础升级获取属性点
    public short BaseAttributePointsGainPerLevelUp = 5;
    //背包
    public Array<PickableObject> Inventory = new();

    //装备
    public ILeftHandHoldEquipment LeftHandHoldEquipment;
    public IRightHandHoldEquipment RightHandHoldEquipment;
    public IBodyWearEquipment BodyWearEquipment;
    public INeckWearEquipment NeckWearEquipment;
    public IFingerWearEquipment FingerWearEquipment;
}