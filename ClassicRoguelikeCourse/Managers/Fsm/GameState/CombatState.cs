using System;
using ClassicRoguelikeCourse.Entites.Characters.Player;
using Godot;

namespace ClassicRoguelikeCourse.Managers.Fsm.GameState;
/// <summary>
/// 处理战斗逻辑
/// </summary>
public partial class CombatState : Node, IState
{
    public event Action<IState> UpdateEvent;
    
    private CombatManager.CombatManager _combatManager;
    
    public void Initialize()
    {
        _combatManager = GetTree().CurrentScene.GetNode<CombatManager.CombatManager>("%CombatManager");
    }
    
    public void Update(double delta)
    {
        _combatManager.Update(delta);
        UpdateEvent?.Invoke(this);
    }
}