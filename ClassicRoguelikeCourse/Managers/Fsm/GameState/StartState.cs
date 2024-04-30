using System;
using ClassicRoguelikeCourse.Entites.Characters.Player;
using ClassicRoguelikeCourse.UI.AttributePanel;
using ClassicRoguelikeCourse.UI.InventoryWindow;
using Godot;
using Player = ClassicRoguelikeCourse.Entities.Characters.Player.Player;

namespace ClassicRoguelikeCourse.Managers.Fsm.GameState;

/// <summary>
/// 状态循环启动时首先进入该状态
/// 并且在本状态中初始化除状态机以及状态模块外的所有其他entity和manager
/// 在切换到其他主场景前不会再次进入该状态
/// </summary>
public partial class StartState : Node, IState
{
    // 状态更新事件
    public event Action<IState> UpdateEvent;

    // 地图管理器
    private MapManager.MapManager _mapManager;

    // 输入管理器
    private InputHandler _inputHandler;

    // 玩家
    private Player _player;

    // 敌人管理器
    private EnemyManager.EnemyManager _enemyManager;

    // A*网格管理器
    private AStarGridManager.AStarGridManager _aStarGridManager;

    // 背包窗口
    private InventoryWindow _inventoryWindow;

    // 迷雾管理器
    private FogPainterManager.FogPainterManager _fogPainterManager;

    // 属性面板
    private AttributePanel _attributePanel;
    // 楼梯管理器
    private StairManager.StairManager _stairManager;
    // 存档管理器
    private SaveLoadManager.SaveLoadManager _saveLoadManager;
    // 拾取物生成器
    private PickableObjectSpawner.PickableObjectSpawner _pickableObjectSpawner;


    public async void Initialize()
    {
        // 获取节点
        _inputHandler = GetTree().CurrentScene.GetNode<InputHandler>("%InputHandler");
        _player = GetTree().CurrentScene.GetNode<Player>("%Player");
        _mapManager = GetTree().CurrentScene.GetNode<MapManager.MapManager>("%MapManager");
        _enemyManager = GetTree().CurrentScene.GetNode<EnemyManager.EnemyManager>("%EnemyManager");
        _aStarGridManager = GetTree().CurrentScene.GetNode<AStarGridManager.AStarGridManager>("%AStarGridManager");
        _inventoryWindow = GetTree().CurrentScene.GetNode<InventoryWindow>("%InventoryWindow");
        _fogPainterManager = GetTree().CurrentScene.GetNode<FogPainterManager.FogPainterManager>("%FogPainterManager");
        _attributePanel = GetTree().CurrentScene.GetNode<AttributePanel>("%AttributePanel");
        _stairManager = GetTree().CurrentScene.GetNode<StairManager.StairManager>("%StairManager");
        _saveLoadManager = GetTree().CurrentScene.GetNode<SaveLoadManager.SaveLoadManager>("%SaveLoadManager");
        _pickableObjectSpawner = GetTree().CurrentScene.GetNode<PickableObjectSpawner.PickableObjectSpawner>("%PickableObjectSpawner");
        // 初始化
        _saveLoadManager.Initialize();
        _inputHandler.Initialize();
        _player.Initialize();
        _enemyManager.Initialize();
        _pickableObjectSpawner.Initialize();
        _mapManager.Initialize();
        _inventoryWindow.Initialize();
        _attributePanel.Initialize();
        // 等待一帧
        await ToSignal(GetTree(), "process_frame");
        //最后初始化
        _aStarGridManager.Initialize();
        _stairManager.Initialize();
        _fogPainterManager.Initialize();
    }

    public void Update(double delta)
    {
        UpdateEvent?.Invoke(this);
    }
}