using ClassicRoguelikeCourse.Component.AiComponent.Ai;
using ClassicRoguelikeCourse.Entities.Characters.Enemies;
using ClassicRoguelikeCourse.Entities.Characters.Player;
using ClassicRoguelikeCourse.Managers;
using ClassicRoguelikeCourse.Managers.AStarGridManager;
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
    
    // A*网格管理器
    private AStarGridManager _aStarGridManager;
    
    public void Initialize()
    {
        _inputHandler = GetTree().CurrentScene.GetNode<InputHandler>("%InputHandler");
        _mapManager = GetTree().CurrentScene.GetNode<MapManager>("%MapManager");
        _aStarGridManager = GetTree().CurrentScene.GetNode<AStarGridManager>("%AStarGridManager");

        //获取组件拥有者
        var owner = GetOwner<Node>();
        // 敌人 并且拥有AI组件
        if (owner is Enemy && owner.HasNode("AiComponent"))
        {
            // AI 闲逛组件存在
            if (owner.HasNode("AiComponent/WalkAroundAi"))
            {
                // AI 闲逛组件执行事件
                owner.GetNode<WalkAroundAi>("AiComponent/WalkAroundAi").ExecutedEvent += direction => _currentDirection = direction;
            }
            
            // AI 追击组件存在
            if (owner.HasNode("AiComponent/ChaseAi"))
            {
                // AI 追击组件执行事件
                owner.GetNode<ChaseAi>("AiComponent/ChaseAi").ExecutedEvent += direction => _currentDirection = direction;
            }
        }
        // 玩家
        else if (owner is Player)
        {
            //绑定输入事件
            _inputHandler.MovementInputEvent += direction => _currentDirection = direction;
        }

        
    }

    public void Update(double delta)
    {
        //获取组件拥有者
        var owner = GetOwner<Node2D>();

        if (_currentDirection == Vector2I.Zero)
        {
            //尝试设置敌人拥有者单元格为固体
            TrySetEnemyOwnerCellSolid(owner);
            return;
        }

        if (IsMovementBlock())
        {
            //尝试设置敌人拥有者单元格为固体
            TrySetEnemyOwnerCellSolid(owner);
            //重置方向
            _currentDirection = Vector2I.Zero;
            return;
        }
        
        //_CurrentDirection * new Vector2I(16, 16) 像素偏移量  GlobalPosition += 偏移量   目前硬编码 后面使用地图数据替换  
        owner.GlobalPosition += _currentDirection * _mapManager.MapData.CellSize;
        //尝试设置敌人拥有者单元格为固体
        TrySetEnemyOwnerCellSolid(owner);
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
            Position = targetPosition, //目标位置
            CollideWithAreas = true, //碰撞区域
            CollisionMask = (uint)PhysicsLayer.BlockMovement //碰撞层
        };
        //碰撞检测结果  如果有碰撞结果 results.Count > 0 没有则 results.Count = 0
        var results = space.IntersectPoint(parameters);
        return results.Count > 0;
    }

    /// <summary>
    /// 设置敌人移动单元格为固体
    /// </summary>
    /// <param name="owner"></param>
    private void TrySetEnemyOwnerCellSolid(Node2D owner)
    {
        if (owner is Enemy)
        {
            //设置敌人所在单元格为固体
            _aStarGridManager.AStarGrid2D.SetPointSolid(
                //敌人所在单元格
                (Vector2I)(owner.GlobalPosition - _mapManager.MapData.CellSize / 2) / _mapManager.MapData.CellSize
                );
        }
        
    }
}