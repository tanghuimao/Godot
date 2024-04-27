using System;
using ClassicRoguelikeCourse.Entites.Characters.Player;
using Godot;

namespace ClassicRoguelikeCourse.Managers.Fsm.GameState;

/// <summary>
/// 处理玩家、敌人等对象的相关行为逻辑
/// </summary>
public partial class ActionState : Node, IState
{
    // 状态更新事件
    public event Action<IState> UpdateEvent;
    // 玩家对象
    private Player _player;
    // 敌人容器
    public Node _enemyContainer;
    public override void _Ready()
    {
        _player = GetTree().CurrentScene.GetNode<Player>("%Player");
        _enemyContainer = GetTree().CurrentScene.GetNode<Node>("%EnemyContainer");
    }

    public void Enter()
    {   
        // GD.Print("ActionState Enter");
      
    }

    public void Exit()
    {
        // GD.Print("ActionState Exit");
    }

    public void Update(double delta)
    {
        //执行玩家的逻辑
        _player.Update(delta);
        //执行敌人的逻辑
        foreach (var child in _enemyContainer.GetChildren())
        {
            if (child is Enemy enemy)
            {
                enemy.Update(delta);
            }
        }
        UpdateEvent?.Invoke(this);
    }
    
}