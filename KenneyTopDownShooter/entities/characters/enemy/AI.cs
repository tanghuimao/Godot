using Godot;
using System;
/// <summary>
/// Ai状态
/// </summary>
public enum AIState
{
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
    //状态
    private AIState _currentState = AIState.Patrol;
    //玩家
    private Player _player;
    //敌人
    private Enemy _enemy;
    //武器
    private Weapon _weapon;
    
    
    public override void _Ready()
    {
        _directionZoneArea2D = GetNode<Area2D>("DirectionZoneArea2D");
        _directionZoneArea2D.BodyEntered += OnBodyEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        switch (_currentState)
        {
            case AIState.Patrol:
                break;
            case AIState.EnGage:
                if (_player == null || _weapon == null)
                {
                    GD.Print("玩家或者武器为空");
                }
                else
                {
                    //敌人面向玩家
                    _enemy.LookAt(_player.Position);
                    //武器射击
                    _weapon.Shoot();
                }
                break;
        }
    }
    
    /// <summary>
    /// 初始化AI
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="weapon"></param>
    public void Init(Enemy enemy, Weapon weapon)
    {
        _enemy = enemy;
        _weapon = weapon;
    }

    private void SetState(AIState state)
    {
        if (state == _currentState) return;

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
}
