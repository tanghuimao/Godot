using System;
using ClassicRoguelikeCourse.Entities.PickableObjects.Equipments;
using Godot;
using Godot.Collections;

namespace ClassicRoguelikeCourse.Resources.CharacterData.PlayerData;

/// <summary>
/// 玩家数据
/// </summary>
public partial class PlayerData : CharacterData
{
    public event Action<int> LevelChanged;

    public event Action<float> ExperienceChanged;

    //等级
    protected short _level = 1;

    public short Level
    {
        get => _level;
        set
        {
            _level = value;
            LevelChanged?.Invoke(_level);
        }
    }

    //经验
    protected float _experience = 0f;

    public float Experience
    {
        get => _experience;
        set
        {
            //升级检测
            var currentExperienceThreshold = CurrentLevelUpExperienceThreshold;

            if (value >= currentExperienceThreshold)
            {
                Level++;

                for (int i = 0; i < BaseAttributePointsGainPerLevelUp; i++)
                {
                    switch (GD.RandRange(0, 2))
                    {
                        case 0:
                            Strength++;
                            break;
                        case 1:
                            Constitution++;
                            break;
                        case 2:
                            Agility++;
                            break;
                    }
                }
                value -= currentExperienceThreshold;
            }

            _experience = value;
            ExperienceChanged?.Invoke(_experience);
        }
    }

    //基础经验阈值
    public float BaseLevelUpExperienceThreshold = 10f;

    //升级经验阈值递增率
    public float LevelUpExperienceThresholdIncrementRate = 0.2f;

    //当前升级经验阈值
    public float CurrentLevelUpExperienceThreshold
    {
        get
        {
            //升级经验
            var currentExperienceThreshold = BaseLevelUpExperienceThreshold;
            for (int i = 0; i < _level; i++)
            {
                currentExperienceThreshold += currentExperienceThreshold * LevelUpExperienceThresholdIncrementRate;
            }

            return currentExperienceThreshold;
        }
    }

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