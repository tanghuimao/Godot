using Godot;

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
}