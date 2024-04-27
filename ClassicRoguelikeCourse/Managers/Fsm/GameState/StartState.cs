using System;
using ClassicRoguelikeCourse.Entites.Characters.Player;
using Godot;

namespace ClassicRoguelikeCourse.Managers.Fsm.GameState;
/// <summary>
/// 状态循环启动时首先进入该状态
/// 并且在本状态中初始化除状态机以及状态模块外的所有其他entity和manager
/// 在切换到其他主场景前不会再次进入该状态
/// </summary>
public partial class StartState : Node, IState
{
    public event Action<IState> UpdateEvent;

    private MapManager.MapManager _mapManager;
    private InputHandler _inputHandler;
    private Player _player;
    private EnemySpawner _enemySpawner;


    public override void _Ready()
    {
        // 获取节点
        _inputHandler = GetTree().CurrentScene.GetNode<InputHandler>("%InputHandler");
        _player = GetTree().CurrentScene.GetNode<Player>("%Player");
        _mapManager = GetTree().CurrentScene.GetNode<MapManager.MapManager>("%MapManager");
        _enemySpawner = GetTree().CurrentScene.GetNode<EnemySpawner>("%EnemySpawner");
        // 初始化
        _player.Initialize();
        _enemySpawner.Initialize();
        _inputHandler.Initialize();
        _mapManager.Initialize();
    }

    public void Enter()
    {
        // GD.Print("StartState Enter");
    }

    public void Exit()
    {
        // GD.Print("StartState Exit");
    }

    public void Update(double delta)
    {
        UpdateEvent?.Invoke(this);
    }
}