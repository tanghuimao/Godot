using ClassicRoguelikeCourse.Entites.Characters.Player;
using Godot;
using Godot.Collections;
using Player = ClassicRoguelikeCourse.Entities.Characters.Player.Player;

namespace ClassicRoguelikeCourse.Managers.AStarGridManager;

/// <summary>
/// 2D A*路径规划  网格寻路
/// </summary>
public partial class AStarGridManager : Node, IManager
{
    //地图管理器
    private MapManager.MapManager _mapManager;
    //A*网格
    private AStarGrid2D _aStarGrid2D;
    //A*网格
    public AStarGrid2D AStarGrid2D => _aStarGrid2D;
    
    public void Initialize()
    {
        //获取地图管理器
        _mapManager = GetTree().CurrentScene.GetNode<MapManager.MapManager>("%MapManager");
        //初始化A*网格
        _aStarGrid2D = new AStarGrid2D
        {
            //地图区域
            Region = new Rect2I(0, 0, _mapManager.MapData.MapSize),
            //单元格大小
            CellSize = _mapManager.MapData.CellSize,
        };
        //更新网格
        _aStarGrid2D.Update();

        //遍历地图
        for (int x = 0; x < _mapManager.MapData.MapSize.X; x++)
        {
            for (int y = 0; y < _mapManager.MapData.MapSize.Y; y++)
            {
                //获取单元格
                var cell = new Vector2I(x, y);
                if (IsCellShouldSetSolid(cell))
                {
                    //设置为不可通过
                    _aStarGrid2D.SetPointSolid(cell);
                }
            }
        }
    }

    /// <summary>
    /// 判断单元格是否应该设置为不可通过
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public bool IsCellShouldSetSolid(Vector2I cell)
    {
        //目标位置
        var targetPosition = cell * _mapManager.MapData.CellSize + _mapManager.MapData.CellSize / 2;
        
        //2D空间
        var space = GetTree().CurrentScene.GetNode<Player>("%Player").GetWorld2D().DirectSpaceState;
        //碰撞检测
        var parameters = new PhysicsPointQueryParameters2D
        {
            Position = targetPosition, //目标位置
            CollideWithAreas = true, //碰撞区域
            CollisionMask = (uint)PhysicsLayer.BlockMovement, //碰撞掩码
            Exclude = new Array<Rid> //排除
            {
                //排除玩家碰撞区域
                GetTree().CurrentScene.GetNode<Area2D>("%Player/Area2D").GetRid(),
            }
        };
        //碰撞检测结果  如果有碰撞结果 results.Count > 0 没有则 results.Count = 0
        var results = space.IntersectPoint(parameters);
        return results.Count > 0;
    }

    public void Update(double delta)
    {
    }
}