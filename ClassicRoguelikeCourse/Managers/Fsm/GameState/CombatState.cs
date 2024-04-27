using System;
using Godot;

namespace ClassicRoguelikeCourse.Managers.Fsm.GameState;
/// <summary>
/// 处理战斗逻辑
/// </summary>
public partial class CombatState : Node, IState
{
    public event Action<IState> UpdateEvent;
    public void Enter()
    {
        // GD.Print("CombatState Enter");
    }

    public void Exit()
    {
        // GD.Print("CombatState Exit");
    }

    public void Update(double delta)
    {
        UpdateEvent?.Invoke(this);
    }
}