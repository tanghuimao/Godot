using ClassicRoguelikeCourse.Entites.Characters.Player;
using ClassicRoguelikeCourse.Entities.Characters.Enemies;
using ClassicRoguelikeCourse.Managers.SaveLoadManager;
using Godot;
using Godot.Collections;
using Player = ClassicRoguelikeCourse.Entities.Characters.Player.Player;

namespace ClassicRoguelikeCourse.Managers.FogPainterManager;

/// <summary>
/// 迷雾绘制管理器
/// </summary>
public partial class FogPainterManager : Node, IManager, ILoadable
{
    // 地图管理器
    private MapManager.MapManager _mapManager;

    // 地图瓦片地图
    private TileMap _tileMap;

    // 玩家
    private Player _player;

    // 上一帧已探索所有单元格
    private Array<Vector2I> _previousExploredCells = new();
    
    // 敌人容器
    private Node _enemyContainer;
    
    private SaveLoadManager.SaveLoadManager _saveLoadManager;

    public async void Initialize()
    {
        _mapManager = GetTree().CurrentScene.GetNode<MapManager.MapManager>("%MapManager");
        _tileMap = GetTree().CurrentScene.GetNode<TileMap>("%TileMap");
        _player = GetTree().CurrentScene.GetNode<Player>("%Player");
        _enemyContainer = GetTree().CurrentScene.GetNode<Node>("%EnemyContainer");
        _saveLoadManager = GetTree().CurrentScene.GetNode<SaveLoadManager.SaveLoadManager>("%SaveLoadManager");
        //刷新迷雾
        if (!InitializeByLoadData())
        {
            //填充地图所有格子为未探索
            FullFillWithUnexplored();
        }
        //等待一帧
        await ToSignal(GetTree(), "process_frame");
        //刷新迷雾
        RefreshFog();
        
    }

    public void Update(double delta)
    {
        // 刷新迷雾
        RefreshFog();
    }

    /// <summary>
    /// 加载地图数据
    /// </summary>
    /// <returns></returns>
    public bool InitializeByLoadData()
    {
        if (_saveLoadManager.LoadedData == null ||
            _saveLoadManager.LoadedData.Count == 0 ||
            !_saveLoadManager.LoadedData.ContainsKey("Maps") ||
            !_saveLoadManager.LoadedData.ContainsKey("Player") ) return false;
        var maps = _saveLoadManager.LoadedData["Maps"].AsGodotArray<Godot.Collections.Dictionary<string, Variant>>();
        for (int i = 0; i < maps.Count; i++)
        {
            var map = maps[i];
            //名称 和 当前保存场景相同
            if (map["SceneName"].AsString() != GetTree().CurrentScene.Name) continue;
            //获取地图单元格
            var unexploredCells = map["UnexploredCells"].AsGodotArray<Vector2I>();
            var exploredCells = map["ExploredCells"].AsGodotArray<Vector2I>();
            var visibleCells = map["VisibleCells"].AsGodotArray<Vector2I>();

            //设置地图单元格
            _tileMap.SetCellsTerrainConnect(
                (int)TileMapLayer.Fog,
                unexploredCells,
                (int)TerrainSet.Fog,
                (int)FogTerrain.Unexplored,
                false
            );
            
            _tileMap.SetCellsTerrainConnect(
                (int)TileMapLayer.Fog,
                exploredCells,
                (int)TerrainSet.Fog,
                (int)FogTerrain.Explored,
                false
            );
            _tileMap.SetCellsTerrainConnect(
                (int)TileMapLayer.Fog,
                visibleCells,
                (int)TerrainSet.Fog,
                (int)FogTerrain.Visible,
                false
            );
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 填充地图所有格子为未探索
    /// </summary>
    private void FullFillWithUnexplored()
    {
        //所有单元格  Array 类型必须是Godot.Collections.Array
        var allCells = new Array<Vector2I>();
        //生成
        for (int x = 0; x < _mapManager.MapData.MapSize.X; x++)
        {
            for (int y = 0; y < _mapManager.MapData.MapSize.Y; y++)
            {
                allCells.Add(new Vector2I(x, y));
            }
        }

        //设置地图单元格
        _tileMap.SetCellsTerrainConnect(
            (int)TileMapLayer.Fog, 
            allCells, 
            (int)TerrainSet.Fog, 
            (int)FogTerrain.Unexplored,
            false
        );
    }

    /// <summary>
    /// 刷新迷雾 将视野范围内格子设置为探索
    /// </summary>
    private void RefreshFog()
    {
        //1.获取玩家当前视野内所有应该被设置为visible的格子, 存放到单独visible容器
        var currentVisibleCells = GetCurrentVisibleCells();
        //2.从Explored容器中, 删除与visible容器设置为visible的格子相同的单元格
        foreach (var currentVisibleCell in currentVisibleCells)
        {
            if (_previousExploredCells.Contains(currentVisibleCell))
            {
                _previousExploredCells.Remove(currentVisibleCell);
            }
        }

        //3.获取所有需要修正单元格
        var fixableVisibleCells = GetFixableVisibleCells(currentVisibleCells);
        foreach (var fixableVisibleCell in fixableVisibleCells)
        {
            //当前可见单元格不包含修正单元格
            if (!currentVisibleCells.Contains(fixableVisibleCell))
            {
                currentVisibleCells.Add(fixableVisibleCell);
            }
            //已经探索单元格包含修正单元格
            if (_previousExploredCells.Contains(fixableVisibleCell))
            {
                _previousExploredCells.Remove(fixableVisibleCell);
            }
        }

        //4.将Explored容器和visible容器中剩余的格子设置为explored和visible
        _tileMap.SetCellsTerrainConnect(
            (int)TileMapLayer.Fog,
            currentVisibleCells,
            (int)TerrainSet.Fog,
            (int)FogTerrain.Visible,
            false
        );
        _tileMap.SetCellsTerrainConnect(
            (int)TileMapLayer.Fog,
            _previousExploredCells,
            (int)TerrainSet.Fog,
            (int)FogTerrain.Explored,
            false
        );
        //5.将visible容器的引用赋予Explored容器
        _previousExploredCells = currentVisibleCells;
        //6.刷新敌人可见性
        RefreshEnemiesVisibility();
    }

    /// <summary>
    /// 刷新敌人可见性
    /// </summary>
    private void RefreshEnemiesVisibility()
    {
        foreach (var child in _enemyContainer.GetChildren())
        {
            if (child is not Enemy enemy) continue;
            //计算敌人所在单元格
            var enemyCell = (Vector2I)(enemy.GlobalPosition - _mapManager.MapData.CellSize / 2) / _mapManager.MapData.CellSize;
            //获取Fog图层单元格图块数据
            var enemyTileData = _tileMap.GetCellTileData((int)TileMapLayer.Fog, enemyCell);
            //判断当前图块数据是Fog并且是Visible 则敌人可以健
            if (enemyTileData.TerrainSet == (int)TerrainSet.Fog &&
                enemyTileData.Terrain == (int)FogTerrain.Visible)
            {
                enemy.Visible = true;
            }
            else
            {
                enemy.Visible = false;
            }
        }
    }
    
    /// <summary>
    /// 获取当前可见单元格集合
    /// </summary>
    /// <returns></returns>
    private Array<Vector2I> GetCurrentVisibleCells()
    {
        //声明集合
        var visibleCells = new Array<Vector2I>();
        //玩家所在2d世界物理空间状态
        var space = _player.GetWorld2D().DirectSpaceState;
        //玩家所在单元格位置
        var playerCell = (Vector2I)(_player.GlobalPosition - _mapManager.MapData.CellSize / 2) /
                         _mapManager.MapData.CellSize;
        //玩家视野
        var playerSight = _player.CharacterData.Sight;
        //遍历视野范围内单元格
        for (int x = playerCell.X - playerSight; x < playerCell.X + playerSight; x++)
        {
            for (int y = playerCell.Y - playerSight; y < playerCell.Y + playerSight; y++)
            {
                var cell = new Vector2I(x, y);
                //构建物理射线查询参数  
                var parameters = new PhysicsRayQueryParameters2D
                {
                    From = cell * _mapManager.MapData.CellSize + _mapManager.MapData.CellSize / 2, //起点
                    To = _player.GlobalPosition, //终点
                    CollisionMask = (uint)PhysicsLayer.BlockSight, //碰撞层
                    Exclude = new Array<Rid>() { _player.GetNode<Area2D>("Area2D").GetRid() } //排除玩家区域
                };
                //发射射线检测
                var results = space.IntersectRay(parameters);
                //如果没有检测到碰撞 则视线未被遮挡
                if (results.Count == 0)
                {
                    visibleCells.Add(cell);
                }
            }
        }

        return visibleCells;
    }

    /// <summary>
    /// 获取修正后的可见单元格集合
    /// </summary>
    /// <param name="currentVisibleCells">当前可见单元格集合</param>
    /// <returns></returns>
    private Array<Vector2I> GetFixableVisibleCells(Array<Vector2I> currentVisibleCells)
    {
        //声明修正集合
        var fixableVisibleCells = new Array<Vector2I>();

        //声明8个方向的集合
        var leftUpVisibleCells = new Array<Vector2I>();
        var rightUpVisibleCells = new Array<Vector2I>();
        var leftDownVisibleCells = new Array<Vector2I>();
        var rightDownVisibleCells = new Array<Vector2I>();

        //玩家所在单元格位置
        var playerCell = (Vector2I)(_player.GlobalPosition - _mapManager.MapData.CellSize / 2) /
                         _mapManager.MapData.CellSize;
        //玩家视野
        var playerSight = _player.CharacterData.Sight;
        // 遍历当前可见单元格
        foreach (var cell in currentVisibleCells)
        {
            //如果单元格在玩家视野内 则该单元格后方的单元格可能是需要修正的单元格
            if (Mathf.Abs(cell.X - playerCell.X) < playerSight &&
                Mathf.Abs(cell.Y - playerCell.Y) < playerSight)
            {
                //判断单元格所处象限 添加到对应集合  godot y轴向下为正
                if (cell.X <= playerCell.X && cell.Y <= playerCell.Y)
                {
                    leftUpVisibleCells.Add(cell);
                }

                if (cell.X >= playerCell.X && cell.Y <= playerCell.Y)
                {
                    rightUpVisibleCells.Add(cell);
                }

                if (cell.X <= playerCell.X && cell.Y >= playerCell.Y)
                {
                    leftDownVisibleCells.Add(cell);
                }

                if (cell.X >= playerCell.X && cell.Y >= playerCell.Y)
                {
                    rightDownVisibleCells.Add(cell);
                }
            }
        }

        //获取真正需要修改的单元格  声明8个方向的集合单元格后方的单元格
        fixableVisibleCells.AddRange(GetLeftUpVisibleCells(leftUpVisibleCells));
        fixableVisibleCells.AddRange(GetRightUpVisibleCells(rightUpVisibleCells));
        fixableVisibleCells.AddRange(GetLeftDownVisibleCells(leftDownVisibleCells));
        fixableVisibleCells.AddRange(GetRightDownVisibleCells(rightDownVisibleCells));

        return fixableVisibleCells;
    }

    /// <summary>
    /// 获取左上方向需要修正的单元格
    /// </summary>
    /// <param name="leftUpVisibleCells">左上方向需要修正的单元格</param>
    /// <returns></returns>
    private Array<Vector2I> GetLeftUpVisibleCells(Array<Vector2I> leftUpVisibleCells)
    {
        //声明修正集合
        var fixableVisibleCells = new Array<Vector2I>();
        //遍历单元格
        foreach (var cell in leftUpVisibleCells)
        {
            //判断当前单元格是否阻挡视野 不阻挡
            if (!IsCellBlockSight(cell))
            {
                //当前单元格后方所在三个单元格
                fixableVisibleCells.Add(new Vector2I(cell.X - 1, cell.Y));
                fixableVisibleCells.Add(new Vector2I(cell.X, cell.Y - 1));
                fixableVisibleCells.Add(new Vector2I(cell.X - 1, cell.Y - 1));
            }
        }

        return fixableVisibleCells;
    }

    /// <summary>
    /// 获取右上方向需要修正的单元格
    /// </summary>
    /// <param name="rightUpVisibleCells">右上方向需要修正的单元格</param>
    /// <returns></returns>
    private Array<Vector2I> GetRightUpVisibleCells(Array<Vector2I> rightUpVisibleCells)
    {
        //声明修正集合
        var fixableVisibleCells = new Array<Vector2I>();
        //遍历单元格
        foreach (var cell in rightUpVisibleCells)
        {
            //判断当前单元格是否阻挡视野 不阻挡
            if (!IsCellBlockSight(cell))
            {
                //当前单元格后方所在三个单元格
                fixableVisibleCells.Add(new Vector2I(cell.X + 1, cell.Y));
                fixableVisibleCells.Add(new Vector2I(cell.X, cell.Y - 1));
                fixableVisibleCells.Add(new Vector2I(cell.X + 1, cell.Y - 1));
            }
        }

        return fixableVisibleCells;
    }

    /// <summary>
    /// 获取左下方向需要修正的单元格
    /// </summary>
    /// <param name="leftDownVisibleCells">左下方向需要修正的单元格</param>
    /// <returns></returns>
    private Array<Vector2I> GetLeftDownVisibleCells(Array<Vector2I> leftDownVisibleCells)
    {
        //声明修正集合
        var fixableVisibleCells = new Array<Vector2I>();
        //遍历单元格
        foreach (var cell in leftDownVisibleCells)
        {
            //判断当前单元格是否阻挡视野 不阻挡
            if (!IsCellBlockSight(cell))
            {
                //当前单元格后方所在三个单元格
                fixableVisibleCells.Add(new Vector2I(cell.X - 1, cell.Y));
                fixableVisibleCells.Add(new Vector2I(cell.X, cell.Y + 1));
                fixableVisibleCells.Add(new Vector2I(cell.X - 1, cell.Y + 1));
            }
        }

        return fixableVisibleCells;
    }

    /// <summary>
    /// 获取右下方向需要修正的单元格
    /// </summary>
    /// <param name="rightDownVisibleCells">右下方向需要修正的单元格</param>
    /// <returns></returns>
    private Array<Vector2I> GetRightDownVisibleCells(Array<Vector2I> rightDownVisibleCells)
    {
        //声明修正集合
        var fixableVisibleCells = new Array<Vector2I>();
        //遍历单元格
        foreach (var cell in rightDownVisibleCells)
        {
            //判断当前单元格是否阻挡视野 不阻挡
            if (!IsCellBlockSight(cell))
            {
                //当前单元格后方所在三个单元格
                fixableVisibleCells.Add(new Vector2I(cell.X + 1, cell.Y));
                fixableVisibleCells.Add(new Vector2I(cell.X, cell.Y + 1));
                fixableVisibleCells.Add(new Vector2I(cell.X + 1, cell.Y + 1));
            }
        }

        return fixableVisibleCells;
    }

    /// <summary>
    /// 判断单元格是否阻挡视野
    /// </summary>
    /// <param name="cell">单元格</param>
    /// <returns></returns>
    private bool IsCellBlockSight(Vector2I cell)
    {
        //获取单元像素坐标
        var cellPosition = cell * _mapManager.MapData.CellSize + _mapManager.MapData.CellSize / 2;
        //玩家所在2d世界物理空间状态
        var space = _player.GetWorld2D().DirectSpaceState;
        //构建物理点查询参数
        var parameters = new PhysicsPointQueryParameters2D
        {
            Position = cellPosition, //单元格中心点
            CollisionMask = (uint)PhysicsLayer.BlockSight, //碰撞层
            CollideWithAreas = true
        };

        //碰撞检测结果  如果有碰撞结果 results.Count > 0 没有则 results.Count = 0
        var results = space.IntersectPoint(parameters);
        return results.Count > 0;
    }
}