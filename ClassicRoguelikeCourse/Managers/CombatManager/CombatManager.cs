using System;
using System.Collections.Generic;
using ClassicRoguelikeCourse.Entites.Characters;
using Godot;

namespace ClassicRoguelikeCourse.Managers.CombatManager;
/// <summary>
/// 战斗管理器
/// 处理战斗结算
/// </summary>
public partial class CombatManager : Node, IManager
{
    // 角色死亡事件
    public event Action<Character> CharacterDead; 
    
    // 战斗记录  key 攻击者  value 被攻击者
    private List<KeyValuePair<Character, Character>> _combatants = new();
    
    public void Initialize()
    {
    }

    public void Update(double delta)
    {
        HandleCombatants();
    }
    /// <summary>
    /// 添加战斗记录
    /// </summary>
    public void AddCombatant(Character attacker, Character defender)
    {
        _combatants.Add(new KeyValuePair<Character, Character>(attacker, defender));
    }
    
    /// <summary>
    /// 处理本轮所有战斗信息
    /// </summary>
    private void HandleCombatants()
    {
        // 遍历所有战斗
        foreach (var combatant in _combatants)
        {
            // 处理战斗
            HandleCombat(combatant.Key, combatant.Value);
        }
        _combatants.Clear();
    }
    
    /// <summary>
    /// 战斗处理
    /// </summary>
    /// <param name="attacker">攻击者</param>
    /// <param name="defender">被攻击者（防御者）</param>
    private void HandleCombat(Character attacker, Character defender)
    {
        //1.检测被攻击者是否成功闪避攻击
        if (IsVictimDodged(defender)) return;
        //2.检测攻击者是否暴击
        var isCriticalChance = IsAttackCriticalChance(attacker);
        //3.获得攻击者攻击力
        var attackerAttackPower = GetAttackerAttackPower(attacker, isCriticalChance);
        //4.获得被攻击者防御力
        var defenderDefensePower = GetDefenderDefensePower(defender);
        //5.计算被攻击者收到伤害
        var damage = GetDamage(attackerAttackPower, defenderDefensePower);
        //6.扣除被攻击者血量
        HandleDefenderDamage(defender, damage);
        
        GD.Print($"{attacker.CharacterData.Name}对{defender.CharacterData.Name}造成了{damage}点伤害");
    }
    /// <summary>
    /// 检测是否闪避
    /// </summary>
    /// <param name="defender">被攻击者（防御者）</param>
    /// <returns></returns>
    private bool IsVictimDodged(Character defender)
    {
        var randomNumber = GD.RandRange(0f, 1f);
        if (defender.CharacterData.Dodge >= randomNumber)
        {
            GD.Print(defender.CharacterData.Name + " 闪避了攻击");
            return true;
        }
        
        return false;
    }
    /// <summary>
    /// 检测暴击
    /// </summary>
    /// <param name="attacker">攻击者</param>
    /// <returns></returns>
    private bool IsAttackCriticalChance(Character attacker)
    {
        var randomNumber = GD.RandRange(0f, 1f);
        if (attacker.CharacterData.CriticalChance >= randomNumber)
        {
            GD.Print(attacker.CharacterData.Name + "成功施展暴击");
            return true;
        }
        return false;
    }
    /// <summary>
    /// 获得攻击者攻击力
    /// </summary>
    /// <param name="attacker">攻击者</param>
    /// <param name="isCriticalChance">是否暴击</param>
    /// <returns></returns>
    private float GetAttackerAttackPower(Character attacker, bool isCriticalChance)
    {
        return  isCriticalChance ? attacker.CharacterData.Attack * 2 : attacker.CharacterData.Attack;
    }
    /// <summary>
    /// 获得被攻击者防御力
    /// </summary>
    /// <param name="defender"></param>
    /// <returns></returns>
    private float GetDefenderDefensePower(Character defender)
    {
        return defender.CharacterData.Defend;
    }
    /// <summary>
    /// 计算伤害
    /// </summary>
    /// <param name="attackerAttackPower">攻击者攻击力</param>
    /// <param name="defenderDefensePower">被攻击者防御力</param>
    /// <returns></returns>
    private float GetDamage(float attackerAttackPower, float defenderDefensePower)
    {
        var damage = attackerAttackPower - defenderDefensePower;
        if (damage <= 0)
        {
            damage = GD.RandRange(0, 1);
        }
        return damage;
    }
    /// <summary>
    /// 扣除被攻击者血量
    /// </summary>
    /// <param name="defender">被攻击者</param>
    /// <param name="damage">伤害</param>
    private void HandleDefenderDamage(Character defender, float damage)
    {
        // 扣除血量
        defender.CharacterData.Health -= damage;
        // 检测是否死亡
        if (defender.CharacterData.Health <= 0)
        {
            defender.CharacterData.Health = 0;
            CharacterDead?.Invoke(defender);
        }
    }
    
}