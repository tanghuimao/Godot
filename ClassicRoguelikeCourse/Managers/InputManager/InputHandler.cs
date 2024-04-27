using System;
using Godot;

namespace ClassicRoguelikeCourse.Managers;

/// <summary>
/// 输入事件处理器
/// </summary>
public partial class InputHandler : Node, IManager
{
    //定义移动事件 Vector2I 整数向量
    public event  Action<Vector2I> MovementInputEvent;
    //定义中断移动计时器
    private Timer _interruptMovementTimer;
    //定义中断移动计时器最大值
    private float _maxDurationInterruptMovement = 0.5f;
    //定义中断移动计时器最小值
    private float _minDurationInterruptMovement = 0.05f;
    //定义中断移动计时器当前值
    private float _currentDurationInterruptMovement;
    public void Initialize()
    {
        _interruptMovementTimer = GetNode<Timer>("InterruptMovementTimer");
    }

    public void Update(double delta)
    {
        HandleMovementInput();
    }
    /// <summary>
    /// 处理移动事件
    /// </summary>
    /// <returns></returns>
    private bool HandleMovementInput()
    {
        var direction = GetMovementDirection();

        //判断移动方向
        if (direction == Vector2I.Zero)
        {
            //停止中断移动计时器
            _interruptMovementTimer.Stop();
            //重置中断移动计时器当前值
            _currentDurationInterruptMovement = _maxDurationInterruptMovement;
            return false;
        }

        //判断是否中断移动计时器 未停止则不能移动
        if (!_interruptMovementTimer.IsStopped()) return false;
        
        
        //触发移动事件
        MovementInputEvent?.Invoke(direction);
        //启动中断移动计时器
        _interruptMovementTimer.Start(_currentDurationInterruptMovement);
        //重置中断移动计时器当前值
        _currentDurationInterruptMovement = _minDurationInterruptMovement;
        return true;
    }

    /// <summary>
    /// 获取移动方向
    /// </summary>
    /// <returns></returns>
    private Vector2I GetMovementDirection()
    {
        /*//获取移动水平方向  存在三个值  -1（左）  0（未移动）  1 （右）
        var movementH = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        //获取移动垂直方向  存在三个值  -1（上）  0（未移动）  1（下）
        var movementV = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");
        //获取移动方向
        var direction = new Vector2(movementH, movementV);*/
        //获取移动方向
        var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        return (Vector2I)direction.Sign();
    }
}