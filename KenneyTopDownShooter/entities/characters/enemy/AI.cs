using Godot;
using System;
/// <summary>
/// Ai状态
/// </summary>
public enum AIState
{
    None, // 无状态
    Patrol, // 巡逻
    EnGage, // 进入战斗
}

/// <summary>
/// 敌人AI
/// </summary>
public partial class AI : Node2D
{
    public event Action StateChangeEvent;
    
    //2d 区域
    private Area2D _directionZoneArea2D;
    //巡逻时间
    private Timer _patrolTimer;
    
    //状态
    private AIState _currentState;
    //玩家
    private Player _player;
    //敌人
    private Enemy _enemy;
    //武器
    private Weapon _weapon;
    //原始位置
    private Vector2 _originPosition;
    //巡逻位置
    private Vector2 _patrolLocation;
    //是否到达巡逻位置
    private bool _isPatrolLocationReachEd = true;
    
    public override void _Ready()
    {
        _directionZoneArea2D = GetNode<Area2D>("DirectionZoneArea2D");
        _patrolTimer = GetNode<Timer>("PatrolTimer");
        //巡逻时间
        _patrolTimer.Timeout += OnPatolTimerTimeout;
        //监听进入区域事件
        _directionZoneArea2D.BodyEntered += OnBodyEntered;
        _directionZoneArea2D.BodyExited += OnBodyExited;
    }
    
    public override void _ExitTree()
    {
        _directionZoneArea2D.BodyEntered -= OnBodyEntered;
        _directionZoneArea2D.BodyExited -= OnBodyExited;
    }

    public override void _PhysicsProcess(double delta)
    {
        switch (_currentState)
        {
            case AIState.Patrol:
                if (!_isPatrolLocationReachEd)
                {
                    // _enemy.LookAt(_patrolLocation);
                    // 敌人和玩家角度
                    _enemy.RotateToTarget(_patrolLocation);
                    // 移动
                    _enemy.MoveToTarget(_patrolLocation);
                    // 到达目标
                    if (_enemy.GlobalPosition.DistanceTo(_patrolLocation) < 5)
                    {
                        _isPatrolLocationReachEd = true;
                        _patrolTimer.Start();
                    };
                }
                break;
            case AIState.EnGage:
                if (_player == null || _weapon == null)
                {
                    GD.Print("玩家或者武器为空");
                }
                else
                {
                    //敌人面向玩家
                    // _enemy.LookAt(_player.Position);
                    // 敌人和玩家角度
                    var angle_to_player = _enemy.RotateToTarget(_player.GlobalPosition);
                    // 旋转角度差小于0.1 向玩家射击
                    if (Mathf.Abs(_enemy.Rotation - angle_to_player) < 0.1)
                    {
                        //武器射击
                        _weapon.Shoot();
                    }
                    
                }
                break;
        }
    }
    
    /// <summary>
    /// 初始化AI
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="weapon"></param>
    public void Initialize(Enemy enemy, Weapon weapon)
    {
        _enemy = enemy;
        _weapon = weapon;
        //设置状态
        SetState(AIState.Patrol);
    }

    public void SetState(AIState state)
    {
        if (state == _currentState) return;
        //巡逻
        if (state == AIState.Patrol)
        {
            //存储初始位置
            _originPosition = _enemy.GlobalPosition;
            //开始计时器
            _patrolTimer.Start();
            _isPatrolLocationReachEd = true;
        }
        
        _currentState = state;
        //触发状态变更事件
        StateChangeEvent?.Invoke();
    }
    /// <summary>
    /// 玩家进入区域
    /// </summary>
    /// <param name="body"></param>
    private void OnBodyEntered(Node2D body)
    {
        if (body is Player player)
        {
            //设置状态
            SetState(AIState.EnGage);
            _player = player;
        }
    }
    private void OnBodyExited(Node2D body)
    {
        if (_player != null && body is Player)
        {
            SetState(AIState.Patrol);
            _player = null;
        }
    }
    
    /// <summary>
    /// 巡逻时间
    /// </summary>
    private void OnPatolTimerTimeout()
    {
        //随机范围
        var patrolRange = 100;
        var randomX = GD.RandRange(-patrolRange, patrolRange);
        var randomY = GD.RandRange(-patrolRange, patrolRange);
        _patrolLocation = new Vector2(randomX, randomY) + _originPosition;
        _isPatrolLocationReachEd = false;
    }
}
