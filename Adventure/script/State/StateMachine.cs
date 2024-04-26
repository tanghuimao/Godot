using System.Threading.Tasks;
using Godot;
using Godot.Collections;

namespace Adventure.script.State;

/// <summary>
/// 状态机  导出Godot全局类型 需要先编译一下 实现状态切换
/// </summary>
[GlobalClass]
[GodotClassName("StateMachine")]
public partial class StateMachine : Node
{
    /// <summary>
    /// 导出变量到引擎 初始状态
    /// </summary>
    [Export] public State InitialState;
    //状态字典 key 状态名称  v 状态
    private Dictionary<string, State> _states;
    //当前状态
    private State _currentState;
    //当前状态持续时间
    public double StateTime;

    /// <summary>
    /// 初始化
    /// </summary>
    public override void _Ready()
    {
        _states = new Dictionary<string, State>();
        //获取所有子节点
        var children = GetChildren();
        foreach (var child in children)
        {
            if (child is not State state) continue;
            _states[state.Name.ToString().ToUpper()] = state;
            state.StateMachine = this;
            // state.Ready();
        }
        //当前状态
        _currentState = InitialState;
        //进入状态
        _currentState.Enter();
    }
    /// <summary>
    /// 视觉帧处理
    /// </summary>
    /// <param name="delta"></param>
    public override void _Process(double delta)
    {
        _currentState.Update((float)delta);
    }
    /// <summary>
    /// 物理帧处理
    /// </summary>
    /// <param name="delta"></param>
    public override void _PhysicsProcess(double delta)
    {
        _currentState.PhysicsUpdate((float)delta);
        StateTime += delta;
    }
    /// <summary>
    /// 输入事件监听
    /// </summary>
    /// <param name="event"></param>
    public override void _UnhandledInput(InputEvent @event)
    {
        _currentState.HandleInput(@event);
    }
     
    
    public void TransitionTo(string stateName)
    {
        stateName = stateName.ToUpper();
        if (!_states.ContainsKey(stateName) || stateName == _currentState.Name.ToString().ToUpper()) {
            return;
        }
        // log state changes
        // GD.Print($"[{Engine.GetPhysicsFrames()}] {_currentState.Name.ToString().ToUpper()} => {_states[stateName].Name.ToString().ToUpper()}");
        
        _currentState.Exit();
        _currentState = _states[stateName];
        _currentState.Enter();
        //改变状态时间
        StateTime = 0D;
    }
}