using System;
using System.Collections.Generic;
using ClassicRoguelikeCourse.Component;
using ClassicRoguelikeCourse.Managers.CombatManager;
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
    [Export] private CharacterData _characterData;
    public  CharacterData CharacterData => _characterData;
    //组件集合
    protected List<IComponent> Components = new ();
    //地图管理器
    protected MapManager _mapManager;
    //战斗管理器
    protected CombatManager _combatManager;
    //是否死亡
    protected bool _isDead;
    public bool IsDead => _isDead;
    
    public virtual void Initialize()
    {
        _mapManager = GetTree().CurrentScene.GetNode<MapManager>("%MapManager");
        _combatManager = GetTree().CurrentScene.GetNode<CombatManager>("%CombatManager");
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
        //订阅事件
        _combatManager.CharacterDead += OnCharacterDead;
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
        _characterData.Health = _characterData.Constitution * _characterData.ConstitutionIncrementEffects["Health"];
        _characterData.MaxHealth = _characterData.Constitution * _characterData.ConstitutionIncrementEffects["MaxHealth"];
        //攻击力
        _characterData.Attack = _characterData.Strength * _characterData.StrengthIncrementEffects["Attack"];
        //防御力
        _characterData.Defend = _characterData.Strength * _characterData.StrengthIncrementEffects["Defend"];
        //闪避率
        _characterData.Dodge = _characterData.Agility * _characterData.AgilityIncrementEffects["Dodge"];
        //暴击率
        _characterData.CriticalChance = _characterData.Agility * _characterData.AgilityIncrementEffects["CriticalChance"];
    }
    
    /// <summary>
    /// 改变战斗属性
    /// </summary>
    public void ChangeCombatAttributes()
    {
        //最大生命值
        _characterData.MaxHealth = _characterData.Constitution * _characterData.ConstitutionIncrementEffects["MaxHealth"];
        //攻击力
        _characterData.Attack = _characterData.Strength * _characterData.StrengthIncrementEffects["Attack"];
        //防御力
        _characterData.Defend = _characterData.Strength * _characterData.StrengthIncrementEffects["Defend"];
        //闪避率
        _characterData.Dodge = _characterData.Agility * _characterData.AgilityIncrementEffects["Dodge"];
        //暴击率
        _characterData.CriticalChance = _characterData.Agility * _characterData.AgilityIncrementEffects["CriticalChance"];
    }
    /// <summary>
    /// 改变体质
    /// </summary>
    public void ChangeConstitution(int changeConstitution)
    {
        //生命值
        _characterData.Health += changeConstitution * _characterData.ConstitutionIncrementEffects["Health"];
    }
    

    /// <summary>
    /// 获取当前角色到目标单元格距离
    /// </summary>
    /// <param name="targetCell"></param>
    /// <returns></returns>
    public int GetDistanceTo(Vector2I targetCell)
    {
        //获取当前角色到目标单元格距离
        var startCell = (Vector2I)(GlobalPosition - _mapManager.MapData.CellSize / 2) / _mapManager.MapData.CellSize;
        //计算距离
        var distanceX = Mathf.Abs(startCell.X - targetCell.X);
        var distanceY = Mathf.Abs(startCell.Y - targetCell.Y);
        //返回距离
        return Mathf.Max(distanceX, distanceY);
    }
    
    /// <summary>
    /// 死亡事件  子对象重写使用
    /// </summary>
    /// <param name="character"></param>
    /// <exception cref="NotImplementedException"></exception>
    protected virtual void OnCharacterDead(Character character)
    {
        throw new Exception("不可直接调用基类方法");
    }
}