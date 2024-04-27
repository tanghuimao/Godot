using ClassicRoguelikeCourse.Managers.Fsm.GameState;
using Godot;

namespace ClassicRoguelikeCourse.Managers.Fsm;
/// <summary>
/// 有限状态机
/// </summary>
public partial class Fsm : Node, IManager
{
    //状态
    private StartState _startState;
    private WaitForInputState _waitForInputState;
    private ActionState _actionState;
    private CombatState _combatState;
    //当前状态
    private  IState _currentState;
    
    public void Initialize()
    {
        //获取节点
        _startState = GetNode<StartState>("StartState");
        _waitForInputState = GetNode<WaitForInputState>("WaitForInputState");
        _actionState = GetNode<ActionState>("ActionState");
        _combatState = GetNode<CombatState>("CombatState");
        //状态切换
        _startState.UpdateEvent += OnStartStateUpdate;
        _waitForInputState.UpdateEvent += OnWaitForInputStateUpdate;
        _actionState.UpdateEvent += OnActionStateUpdate;
        _combatState.UpdateEvent += OnCombatStateUpdate;
        
        //初始状态
        _currentState = _startState;
        
        _currentState.Enter();
    }
    
    //视觉帧调用
    public void Update(double delta)
    {
        _currentState.Update(delta);
    }
    
    /// <summary>
    /// 状态切换
    /// </summary>
    /// <param name="state"></param>
    private void OnStartStateUpdate(IState state)
    {
        state.Exit();
        _currentState = _waitForInputState;
        _currentState.Enter();
        // GD.Print("状态切换至：", nameof(_waitForInputState));
    }
    /// <summary>
    /// 状态切换
    /// </summary>
    /// <param name="state"></param>
    private void OnWaitForInputStateUpdate(IState state)
    {
        state.Exit();
        _currentState = _actionState;
        _currentState.Enter();
        // GD.Print("状态切换至：", nameof(_actionState));
    }
    ///  <summary>
    /// 状态切换
    /// </summary>
    /// <param name="state"></param>
    private void OnActionStateUpdate(IState state)
    {
        state.Exit();
        _currentState = _combatState;
        _currentState.Enter();
        // GD.Print("状态切换至：", nameof(_combatState));
    }
    ///  <summary>
    /// 状态切换
    /// </summary>
    /// <param name="state"></param>
    private void OnCombatStateUpdate(IState state)
    {
        state.Exit();
        _currentState = _startState;
        _currentState.Enter();
        // GD.Print("状态切换至：", nameof(_startState));
    }


}