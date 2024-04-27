using System;
using System.Collections.Generic;
using ClassicRoguelikeCourse.Component;
using ClassicRoguelikeCourse.Managers.MapManager;
using ClassicRoguelikeCourse.Resources.CharacterData;
using ClassicRoguelikeCourse.Resources.MapData;
using Godot;

namespace ClassicRoguelikeCourse.Entites.Characters;

/// <summary>
/// 角色抽象父类接口
/// </summary>
public partial class Character : Node2D, IEntity
{
    //角色数据
    [Export] public CharacterData CharacterData;
    //组件集合
    protected List<IComponent> Components = new ();
    //地图管理器
    protected MapManager _mapManager;
    
    public virtual void Initialize()
    {
        _mapManager = GetTree().CurrentScene.GetNode<MapManager>("%MapManager");
        //获取所有子节点
        foreach (var child in GetChildren())
        {
            if (child is not IComponent) continue;
            var component = child as IComponent;
            component.Initialize();
            Components.Add(component);
        }
        //初始化战斗属性
        InitializeCombatAttributes();
    }

    public virtual void Update(double delta)
    {
        //遍历组件 调用更新方法
        foreach (var component in Components)
        {
            component.Update(delta);
        }
    }

    /// <summary>
    /// 初始化战斗属性
    /// </summary>
    private void InitializeCombatAttributes()
    {
        //生命值
        CharacterData.Health = CharacterData.Constitution * CharacterData.ConstitutionIncrementEffects["Health"];
        CharacterData.MaxHealth = CharacterData.Constitution * CharacterData.ConstitutionIncrementEffects["MaxHealth"];
        //攻击力
        CharacterData.Attack = CharacterData.Strength * CharacterData.StrengthIncrementEffects["Attack"];
        //防御力
        CharacterData.Defend = CharacterData.Strength * CharacterData.StrengthIncrementEffects["Defend"];
        //闪避率
        CharacterData.Dodge = CharacterData.Agility * CharacterData.AgilityIncrementEffects["Dodge"];
        //暴击率
        CharacterData.CriticalChance = CharacterData.Agility * CharacterData.AgilityIncrementEffects["CriticalChance"];
    }
}