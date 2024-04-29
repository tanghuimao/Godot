using System;
using ClassicRoguelikeCourse.Entites.Characters.Player;
using ClassicRoguelikeCourse.UI.InventoryWindow;
using Godot;

namespace ClassicRoguelikeCourse.Managers;

/// <summary>
/// 输入事件处理器
/// </summary>
public partial class InputHandler : Node, IManager
{
    //定义移动事件 Vector2I 整数向量
    public event Action<Vector2I> MovementInputEvent;
    //定义拾取事件
    public event Action PickupInputEvent;
    //定义打开背包事件
    public event Action ToggleInventoryWindowInputEvent;
    //定义使用背包物品事件
    public event Action UseInventoryObjectInputEvent;
    //定义丢弃背包物品事件
    public event Action PutAwayInventoryObjectInputEvent;
    //定义上楼梯事件
    public event  Action GoUpStairInputEvent;
    //定义下楼梯事件
    public event  Action GoDownStairInputEvent;
    
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
    // 背包窗口
    private InventoryWindow _inventoryWindow;
    
    public void Initialize()
    {
        _mapManager = GetTree().CurrentScene.GetNode<MapManager.MapManager>("%MapManager");
        _player = GetTree().CurrentScene.GetNode<Player>("%Player");
        _interruptMovementTimer = GetNode<Timer>("InterruptMovementTimer");
        _combatManager = GetTree().CurrentScene.GetNode<CombatManager.CombatManager>("%CombatManager");
        _inventoryWindow = GetTree().CurrentScene.GetNode<InventoryWindow>("%InventoryWindow");
    }

    public void Update(double delta)
    {
        // 判断玩家是否死亡
        if (_player.IsDead) return;
        // 处理使用背包物品事件
        if (_inventoryWindow.Visible && HandleUseInventoryObjectInputEvent()) return;
        // 处理丢弃背包物品事件
        if (_inventoryWindow.Visible && HandlePutAwayInventoryObjectInputEvent()) return;
        // 处理打开背包事件
        if (HandleToggleInventoryWindowInput()) return;
        // 判断背包窗口是否可见
        if (_inventoryWindow.Visible) return;
        // 处理上楼梯事件
        if (HandleGoUpStairInputEvent()) return;
        // 处理下楼梯事件
        if (HandleGoDownStairInputEvent()) return;
        // 处理拾取事件
        if (HandlePickUpInput()) return;
        // 处理移动事件
        HandleMovementInput();
    }
    
    /// <summary>
    /// 处理上楼梯事件
    /// </summary>
    /// <returns></returns>
    public bool HandleGoUpStairInputEvent()
    {
        if (Input.IsActionJustPressed("go_up_stair"))
        {
            GoUpStairInputEvent?.Invoke();
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 处理下楼梯事件
    /// </summary>
    /// <returns></returns>
    public bool HandleGoDownStairInputEvent()
    {
        if (Input.IsActionJustPressed("go_down_stair"))
        {
            GoDownStairInputEvent?.Invoke();
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 处理使用背包物品事件
    /// </summary>
    /// <returns></returns>
    public bool HandleUseInventoryObjectInputEvent()
    {
        if (Input.IsActionJustPressed("use_inventory_object"))
        {
            UseInventoryObjectInputEvent?.Invoke();
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 处理丢弃背包物品事件
    /// </summary>
    /// <returns></returns>
    public bool HandlePutAwayInventoryObjectInputEvent()
    {
        if (Input.IsActionJustPressed("put_away_inventory_object"))
        {
            PutAwayInventoryObjectInputEvent?.Invoke();
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 处理打开背包事件
    /// </summary>
    /// <returns></returns>
    public bool HandleToggleInventoryWindowInput()
    {
        if (Input.IsActionJustPressed("toggle_inventory"))
        {
            ToggleInventoryWindowInputEvent?.Invoke();
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 处理拾取事件
    /// </summary>
    /// <returns></returns>
    private bool HandlePickUpInput()
    {
        if (Input.IsActionJustPressed("pick_up"))
        {
            PickupInputEvent?.Invoke();
            return true;
        }
        return false;
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
        //碰撞检测参数
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