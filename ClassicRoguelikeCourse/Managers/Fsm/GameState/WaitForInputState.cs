using System;
using System.Threading.Tasks;
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

    public override void _Ready()
    {
        // 获取当前场景下的InputHandler
        _inputHandler = GetTree().CurrentScene.GetNode<InputHandler>("%InputHandler");
        // 监听InputHandler的输入事件
        _inputHandler.MovementInputEvent += OnMovementInputEvent;
    }

    public void Enter()
    {
        // GD.Print("WaitForInputState Enter");
    }

    public void Exit()
    {
        // GD.Print("WaitForInputState Exit");
    }

    public void Update(double delta)
    {
        _inputHandler.Update(delta);
    }
    /// <summary>
    /// 输入事件
    /// </summary>
    /// <param name="obj"></param>
    private void OnMovementInputEvent(Vector2I obj)
    {
        // 触发状态更新事件
        UpdateEvent?.Invoke(this);
    }
}