using Godot;
using Godot.Collections;

namespace ClassicRoguelikeCourse.Resources.CharacterData;
/// <summary>
/// 角色数据  父类
/// </summary>
public partial class CharacterData : Resource
{
    //角色名称
    [Export]
    public string Name;
    //角色视野
    [Export]
    public int Sight = 6;

    /// <summary>
    /// 基础属性
    /// </summary>
    //力量
    [Export]
    public int Strength = 8;
    //体质
    [Export]
    public int Constitution = 8;
    //敏捷
    [Export]
    public int Agility = 8;
    /// <summary>
    /// 基础属性对战斗属性影响  Dictionary使用godot集合中的字典
    /// </summary>
    [Export]
    public Dictionary<string, float> StrengthIncrementEffects = new()
    {
        {"Attack", 2f}, {"Defend", 1f}
    }; 
    [Export]
    public Dictionary<string, float> ConstitutionIncrementEffects = new()
    {
        {"MaxHealth", 3f}, {"Health", 3f}
    };
    [Export]
    public Dictionary<string, float> AgilityIncrementEffects = new()
    {
        {"Dodge", 0.01f}, {"CriticalChance", 0.005f}
    }; 
    
    /// <summary>
    /// 战斗属性
    /// </summary>
    // 生命
    public float Health;
    //最大生命
    public float MaxHealth;
    //攻击
    public float Attack;
    //防御
    public float Defend;
    //闪避
    public float Dodge;
    //暴击
    public float CriticalChance;
}