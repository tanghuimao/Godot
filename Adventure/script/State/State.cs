using Godot;

namespace Adventure.script.State;

/// <summary>
/// 状态 定义状态入口  导出Godot全局类型 需要先编译一下
/// </summary>
[GlobalClass]
[GodotClassName("State")]
public partial class State : Node
{
    /// <summary>
    /// 状态机
    /// </summary>
    public StateMachine StateMachine;
    //进入
    public virtual void Enter() {}
    //退出
    public virtual void Exit() {}
    //视觉帧调用
    public virtual void Update(float delta) {}
    //物理帧调用
    public virtual void PhysicsUpdate(float delta) {}
    //输入事件状态
    public virtual void HandleInput(InputEvent @event) {}
    //状态转移
    public virtual void MangeStateShift() {}
}