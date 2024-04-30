using System;
using ClassicRoguelikeCourse.Entites.Characters.Player;
using ClassicRoguelikeCourse.Entities.Characters.Enemies;
using Godot;

namespace ClassicRoguelikeCourse.Managers.Fsm.GameState;
/// <summary>
/// 处理战斗逻辑
/// </summary>
public partial class CombatState : Node, IState
{
    public event Action<IState> UpdateEvent;
    
    private CombatManager.CombatManager _combatManager;

    private Node _enemyContainer;
    
    public void Initialize()
    {
        _combatManager = GetTree().CurrentScene.GetNode<CombatManager.CombatManager>("%CombatManager");
        _enemyContainer = GetTree().CurrentScene.GetNode<Node>("%EnemyContainer");
    }
    
    public void Update(double delta)
    {
        _combatManager.Update(delta);
        // 遍历所有敌人
        foreach (var child in _enemyContainer.GetChildren())
        {
            if (child is Enemy enemy)
            {
                // 延迟更新
                enemy.LateUpdate();
            }
        }
        
        UpdateEvent?.Invoke(this);
    }
}