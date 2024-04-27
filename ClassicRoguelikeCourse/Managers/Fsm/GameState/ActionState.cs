using System;
using ClassicRoguelikeCourse.Entites.Characters.Player;
using Godot;

namespace ClassicRoguelikeCourse.Managers.Fsm.GameState;

/// <summary>
/// 处理玩家、敌人等对象的相关行为逻辑
/// </summary>
public partial class ActionState : Node, IState
{
    public event Action<IState> UpdateEvent;
    private Player _player;

    public override void _Ready()
    {
        _player = GetTree().CurrentScene.GetNode<Player>("%Player");
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
        
        UpdateEvent?.Invoke(this);
    }
    
}