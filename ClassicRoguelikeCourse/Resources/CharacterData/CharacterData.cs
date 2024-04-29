using System;
using Godot;
using Godot.Collections;

namespace ClassicRoguelikeCourse.Resources.CharacterData;

/// <summary>
/// 角色数据  父类
/// </summary>
public partial class CharacterData : Resource
{
    //定义属性变更事件
    public event Action<int> SightChanged;
    public event Action<int> StrengthChanged;
    public event Action<int> ConstitutionChanged;
    public event Action<int> AgilityChanged;
    public event Action<float> HealthChanged;
    public event Action<float> MaxHealthChanged;
    public event Action<float> AttackChanged;
    public event Action<float> DefendChanged;
    public event Action<float> DodgeChanged;
    public event Action<float> CriticalChanceChanged;

    //角色名称
    [Export] public string Name;

    //角色视野
    [Export] protected int _sight = 6;

    public int Sight
    {
        get => _sight;
        set
        {
            _sight = value;
            SightChanged?.Invoke(value);
        }
    }

    /// <summary>
    /// 基础属性
    /// </summary>
    //力量
    [Export] protected int _strength = 8;

    public int Strength
    {
        get => _strength;
        set
        {
            //力量属性对攻击、防御 影响
            var valueDifference = value - _strength;
            Attack += valueDifference * StrengthIncrementEffects["Attack"];
            Defend += valueDifference * StrengthIncrementEffects["Defend"];
            _strength = value;
            StrengthChanged?.Invoke(value);
        }
    }

    //体质
    [Export] protected int _constitution = 8;

    public int Constitution
    {
        get => _constitution;
        set
        {
            //体质属性对生命 影响
            var valueDifference = value - _constitution;
            MaxHealth += valueDifference * ConstitutionIncrementEffects["MaxHealth"];
            Health += valueDifference * ConstitutionIncrementEffects["Health"];
            Defend += valueDifference * ConstitutionIncrementEffects["Defend"];
            _constitution = value;
            ConstitutionChanged?.Invoke(value);
        }
    }

    //敏捷
    [Export] protected int _agility = 8;

    public int Agility
    {
        get => _agility;
        set
        {
            //敏捷属性对闪避、暴击 影响
            var valueDifference = value - _agility;
            Dodge += valueDifference * AgilityIncrementEffects["Dodge"];
            CriticalChance += valueDifference * AgilityIncrementEffects["CriticalChance"];
            _agility = value;
            AgilityChanged?.Invoke(value);
        }
    }

    /// <summary>
    /// 基础属性对战斗属性影响  Dictionary使用godot集合中的字典
    /// </summary>
    [Export] public Dictionary<string, float> StrengthIncrementEffects = new()
    {
        { "Attack", 2f }, { "Defend", 1f }
    };

    [Export] public Dictionary<string, float> ConstitutionIncrementEffects = new()
    {
        { "MaxHealth", 3f }, { "Health", 3f }, { "Defend", 1f }
    };

    [Export] public Dictionary<string, float> AgilityIncrementEffects = new()
    {
        { "Dodge", 0.01f }, { "CriticalChance", 0.005f }
    };

    /// <summary>
    /// 战斗属性
    /// </summary>
    // 生命
    protected float _health;

    public float Health
    {
        get => _health;
        set
        {
            _health = value;
            HealthChanged?.Invoke(value);
        }
    }

    //最大生命
    protected float _maxHealth;

    public float MaxHealth
    {
        get => _maxHealth;
        set
        {
            _maxHealth = value;
            MaxHealthChanged?.Invoke(value);
        }
    }

    //攻击
    protected float _attack;

    public float Attack
    {
        get => _attack;
        set
        {
            _attack = value;
            AttackChanged?.Invoke(value);
        }
    }

    //防御
    protected float _defend;

    public float Defend
    {
        get => _defend;
        set
        {
            _defend = value;
            DefendChanged?.Invoke(value);
        }
    }

    //闪避
    protected float _dodge;

    public float Dodge
    {
        get => _dodge;
        set
        {
            _dodge = value;
            DodgeChanged?.Invoke(value);
        }
    }

    //暴击
    protected float _criticalChance;

    public float CriticalChance
    {
        get => _criticalChance;
        set
        {
            _criticalChance = value;
            CriticalChanceChanged?.Invoke(value);
        }
    }
}