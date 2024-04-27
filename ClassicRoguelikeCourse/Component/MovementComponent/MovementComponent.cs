using ClassicRoguelikeCourse.Managers;
using ClassicRoguelikeCourse.Managers.MapManager;
using Godot;

namespace ClassicRoguelikeCourse.Component.MovementComponent;

/// <summary>
/// 移动组件
/// </summary>
public partial class MovementComponent : Node2D, IComponent
{
    //输入事件
    private InputHandler _inputHandler;
    
    //当前方向
    private Vector2I _currentDirection;
    
    //地图管理器
    private MapManager _mapManager; 
    
    public void Initialize()
    {
        _inputHandler = GetTree().CurrentScene.GetNode<InputHandler>("%InputHandler");
        _mapManager = GetTree().CurrentScene.GetNode<MapManager>("%MapManager");
        //绑定输入事件
        _inputHandler.MovementInputEvent += OnMovementInputEvent;
    }

    public void Update(double delta)
    {
        //获取组件拥有者
        var owner = GetOwner<Node2D>();

        if (_currentDirection == Vector2I.Zero) return;

        if (IsMovementBlock())
        {
            //重置方向
            _currentDirection = Vector2I.Zero;
            return;
        }
        
        //_CurrentDirection * new Vector2I(16, 16) 像素偏移量  GlobalPosition += 偏移量   目前硬编码 后面使用地图数据替换  
        owner.GlobalPosition += _currentDirection * new Vector2I(16, 16);
        //重置方向
        _currentDirection = Vector2I.Zero;
    }

    /// <summary>
    /// 移动是否被阻挡
    /// </summary>
    /// <returns></returns>
    private bool IsMovementBlock()
    {
        //获取组件拥有者
        var owner = GetOwner<Node2D>();
        //目标位置
        var targetPosition = owner.GlobalPosition + _currentDirection * _mapManager.MapData.CellSize;
        //2D空间
        var space = owner.GetWorld2D().DirectSpaceState;
        //碰撞检测
        var parameters = new PhysicsPointQueryParameters2D
        {
            Position = targetPosition,
            CollideWithAreas = true,
            CollisionMask = (uint)PhysicsLayer.BlockMovement
        };
        //碰撞检测结果  如果有碰撞结果 results.Count > 0 没有则 results.Count = 0
        var results = space.IntersectPoint(parameters);
        return results.Count > 0;
    }
    
    /// <summary>
    /// 移动输入事件
    /// </summary>
    /// <param name="direction"></param>
    private void OnMovementInputEvent(Vector2I direction)
    {
        _currentDirection = direction;
    }
}