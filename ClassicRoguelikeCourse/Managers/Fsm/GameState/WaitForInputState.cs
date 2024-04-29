using System;
using System.Threading.Tasks;
using ClassicRoguelikeCourse.UI.InventoryWindow;
using Godot;

namespace ClassicRoguelikeCourse.Managers.Fsm.GameState;

/// <summary>
/// 该状态用于相应输入事件
/// 处理（InputHandler）发送的输入事件
/// 在没有接收到新的输入事件时， 本状态持续监听并且阻断状态循环
/// </summary>
public partial class WaitForInputState : Node, IState
{
    public event Action<IState> UpdateEvent;

    private InputHandler _inputHandler;
    
    private StairManager.StairManager _stairManager;

    private InventoryWindow _inventoryWindow;

    public void Initialize()
    {
        // 获取当前场景下的InputHandler
        _inputHandler = GetTree().CurrentScene.GetNode<InputHandler>("%InputHandler");
        _stairManager = GetTree().CurrentScene.GetNode<StairManager.StairManager>("%StairManager");
        _inventoryWindow = GetTree().CurrentScene.GetNode<InventoryWindow>("%InventoryWindow");
        // 监听InputHandler的输入事件
        _inputHandler.MovementInputEvent += OnMovementInputEvent;
        _inputHandler.PickupInputEvent += OnPickupInputEvent;
        _inputHandler.ToggleInventoryWindowInputEvent += OnToggleInventoryWindowInputEvent;
        _inputHandler.UseInventoryObjectInputEvent += OnUseInventoryObjectInputEvent;
        _inputHandler.PutAwayInventoryObjectInputEvent += OnPutAwayInventoryObjectInputEvent;
        _inputHandler.GoUpStairInputEvent += OnGoUpStairInputEvent;
        _inputHandler.GoDownStairInputEvent += OnGoDownStairInputEvent;
    }
    public void Update(double delta)
    {
        _inputHandler.Update(delta);
    }
    /// <summary>
    /// 移动事件
    /// </summary>
    /// <param name="obj"></param>
    private void OnMovementInputEvent(Vector2I obj)
    {
        // 触发状态更新事件
        UpdateEvent?.Invoke(this);
    }
    /// <summary>
    /// 拾取事件
    /// </summary>
    private void OnPickupInputEvent()
    {
        // 触发状态更新事件
        UpdateEvent?.Invoke(this);
    }
    /// <summary>
    /// 打开/关闭背包窗口事件
    /// </summary>
    private void OnToggleInventoryWindowInputEvent()
    {
        _inventoryWindow.Toggle();
    }
    /// <summary>
    /// 使用背包物品事件
    /// </summary>
    private void OnPutAwayInventoryObjectInputEvent()
    {
        _inventoryWindow.UseInventoryObject();
        // 触发状态更新事件
        UpdateEvent?.Invoke(this);
    }
    /// <summary>
    /// 使用背包物品事件
    /// </summary>
    private void OnUseInventoryObjectInputEvent()
    {
        _inventoryWindow.UseInventoryObject();
        // 触发状态更新事件
        UpdateEvent?.Invoke(this);
    }
    /// <summary>
    /// 上楼事件
    /// </summary>
    private void OnGoUpStairInputEvent()
    {
        _stairManager.TryGoToPreviousScene();
    }

    /// <summary>
    /// 下楼事件
    /// </summary>
    private void OnGoDownStairInputEvent()
    {
        _stairManager.TryGoToNextScene();
    }
}