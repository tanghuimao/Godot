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
        //初始化
        _startState.Initialize();
        _waitForInputState.Initialize();
        _actionState.Initialize();
        _combatState.Initialize();
        //订阅状态
        _startState.UpdateEvent += OnStartStateUpdate;
        _waitForInputState.UpdateEvent += OnWaitForInputStateUpdate;
        _actionState.UpdateEvent += OnActionStateUpdate;
        _combatState.UpdateEvent += OnCombatStateUpdate;
        
        //初始状态
        _currentState = _startState;
        
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
        _currentState = _waitForInputState;
        // GD.Print("状态切换至：", nameof(_waitForInputState));
    }
    /// <summary>
    /// 状态切换
    /// </summary>
    /// <param name="state"></param>
    private void OnWaitForInputStateUpdate(IState state)
    {
        _currentState = _actionState;
        // GD.Print("状态切换至：", nameof(_actionState));
    }
    ///  <summary>
    /// 状态切换
    /// </summary>
    /// <param name="state"></param>
    private void OnActionStateUpdate(IState state)
    {
        _currentState = _combatState;
        // GD.Print("状态切换至：", nameof(_combatState));
    }
    ///  <summary>
    /// 状态切换
    /// </summary>
    /// <param name="state"></param>
    private void OnCombatStateUpdate(IState state)
    {
        _currentState = _startState;
        // GD.Print("状态切换至：", nameof(_startState));
    }


}