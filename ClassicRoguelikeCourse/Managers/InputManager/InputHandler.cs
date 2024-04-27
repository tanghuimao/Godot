using System;
using ClassicRoguelikeCourse.Entites.Characters.Player;
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
    
    // 地图管理器
    private MapManager.MapManager _mapManager;
    
    // 玩家
    private Player _player;
    // 战斗管理器
    private CombatManager.CombatManager _combatManager;
    public void Initialize()
    {
        _mapManager = GetTree().CurrentScene.GetNode<MapManager.MapManager>("%MapManager");
        _player = GetTree().CurrentScene.GetNode<Player>("%Player");
        _interruptMovementTimer = GetNode<Timer>("InterruptMovementTimer");
        _combatManager = GetTree().CurrentScene.GetNode<CombatManager.CombatManager>("%CombatManager");
    }

    public void Update(double delta)
    {
        // 判断玩家是否死亡
        if (_player.IsDead) return;
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

        //尝试处理近战攻击
        TryHandleMeleeAttack(direction);
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

    /// <summary>
    /// 尝试处理近战攻击
    /// </summary>
    /// <param name="direction">攻击方向</param>
    private void TryHandleMeleeAttack(Vector2I direction)
    {
        //攻击目标位置
        var targetPosition = _player.GlobalPosition + direction * _mapManager.MapData.CellSize;
        //获取物理空间状态
        var space = _player.GetWorld2D().DirectSpaceState;

        var parameters = new PhysicsPointQueryParameters2D
        {
            Position = targetPosition, //目标位置
            CollideWithAreas = true, //碰撞区域
            CollisionMask = (uint)PhysicsLayer.BlockMovement, //碰撞掩码
            CollideWithBodies = false, //忽略其他碰撞体  关注Area2D
        };
        //碰撞检测结果  如果有碰撞结果 results.Count > 0 没有则 results.Count = 0
        var results = space.IntersectPoint(parameters);
        //如果没有碰撞结果
        if (results.Count == 0) return;
        
        foreach (var result in results)
        {
            //获取碰撞体
            var collider = result["collider"].As<Area2D>();
            //如果碰撞体是敌人
            if (collider.Owner is Enemy enemy)
            {
                //攻击将纳入战斗结算
                _combatManager.AddCombatant(_player, enemy);
                GD.Print($"玩家近战攻击了{enemy.CharacterData.Name}");
            }
        }

    }
}