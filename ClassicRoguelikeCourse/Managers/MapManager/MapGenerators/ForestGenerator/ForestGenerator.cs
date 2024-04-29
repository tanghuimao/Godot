using System.Collections.Generic;
using ClassicRoguelikeCourse.Resources.MapData.ForestData;
using Godot;
using Godot.Collections;

namespace ClassicRoguelikeCourse.Managers.MapManager.MapGenerators.ForestGenerator;

public partial class ForestGenerator : Node, IMapGenerator
{
    //地牢地图数据
    private ForestData _forestData;

    //地牢地图瓦片集
    private TileSet _tileSet;

    //主场景 地图
    private TileMap _tileMap;

    public void Initialize()
    {
        _forestData = GetParent<MapManager>().MapData as ForestData;
        //加载地牢地图瓦片集
        _tileSet = GD.Load<TileSet>("res://Resources/TileSets/ForestTileSet.tres");
        //获取主场景 地图
        _tileMap = GetTree().CurrentScene.GetNode<TileMap>("%TileMap");
        //设置瓦片集
        _tileMap.TileSet = _tileSet;
        //生成地图
        GenerateMap();
    }


    public void Update(double delta)
    {
        throw new System.NotImplementedException();
    }

    public Vector2I GetPlayerSpawnCell()
    {
        //2D世界物理空间状态
        var space = _tileMap.GetWorld2D().DirectSpaceState;
        while (true)
        {
            //随机生成玩家位置
            var randomX = GD.RandRange(1, _forestData.MapSize.X - 2);
            var randomY = GD.RandRange(1, _forestData.MapSize.Y - 2);
            //玩家单元格
            var playerStartCell = new Vector2I(randomX, randomY);
            //检测单元格内是否存在其他碰撞体
            var parameter = new PhysicsPointQueryParameters2D
            {
                Position = playerStartCell * _forestData.CellSize + _forestData.CellSize / 2,
                CollisionMask = (uint)PhysicsLayer.BlockMovement,
                CollideWithAreas = true
            };
            //检测结果
            var results = space.IntersectPoint(parameter);
            if (results.Count > 0) continue;
            //结果集为0 则生成玩家
            GetParent<MapManager>().TryAddCharacterCellAtSpawn(playerStartCell);
            return playerStartCell;
        }
    }

    public Vector2I GetEnemySpawnCell()
    {
        while (true)
        {
            //随机生成玩家位置
            var randomX = GD.RandRange(1, _forestData.MapSize.X - 2);
            var randomY = GD.RandRange(1, _forestData.MapSize.Y - 2);
            var enemySpawnCell = new Vector2I(randomX, randomY);
            //尝试添加敌人 返回false 则继续
            if (!GetParent<MapManager>().TryAddCharacterCellAtSpawn(enemySpawnCell)) continue;
            return enemySpawnCell;
        }
    }

    /// <summary>
    /// 生成地图
    /// </summary>
    private void GenerateMap()
    {
        RandomFillTiles();
    }

    /// <summary>
    /// 随机填充地图
    /// </summary>
    private void RandomFillTiles()
    {
        //地面、草地、树、死亡树 集合
        var groundCells = new Array<Vector2I>();
        var grassCells = new Array<Vector2I>();
        var treeCells = new Array<Vector2I>();
        var deadTreeCells = new Array<Vector2I>();

        //遍历地图所有单元格
        for (int x = 0; x < _forestData.MapSize.X; x++)
        {
            for (int y = 0; y < _forestData.MapSize.Y; y++)
            {
                //获取单元格
                var cell = new Vector2I(x, y);

                //判断是否是地图边缘
                if (x == 0 || y == 0 || x == _forestData.MapSize.X - 1 || y == _forestData.MapSize.Y - 1)
                {
                    //随机生成地图边缘
                    if (GD.RandRange(0, 5) == 0)
                    {
                        //死亡树
                        deadTreeCells.Add(cell);
                    }
                    else
                    {
                        //树
                        treeCells.Add(cell);
                    }

                    continue;
                }

                //随机生成地图
                if (GD.RandRange(0, 100) <= 10)
                {
                    if (GD.RandRange(0, 5) == 0)
                    {
                        //死亡树
                        deadTreeCells.Add(cell);
                    }
                    else
                    {
                        //树
                        treeCells.Add(cell);
                    }
                }
                else
                {
                    if (GD.RandRange(0, 5) == 0)
                    {
                        //草地
                        grassCells.Add(cell);
                    }
                    else
                    {
                        //地面
                        groundCells.Add(cell);
                    }
                }
            }
        }

        //设置地图单元格
        _tileMap.SetCellsTerrainConnect(
            (int)TileMapLayer.Default,
            groundCells,
            (int)TerrainSet.Default,
            (int)ForestTerrain.Ground,
            false
        );
        _tileMap.SetCellsTerrainConnect(
            (int)TileMapLayer.Default,
            grassCells,
            (int)TerrainSet.Default,
            (int)ForestTerrain.Grass,
            false
        );
        _tileMap.SetCellsTerrainConnect(
            (int)TileMapLayer.Default,
            treeCells,
            (int)TerrainSet.Default,
            (int)ForestTerrain.Tree,
            false
        );
        _tileMap.SetCellsTerrainConnect(
            (int)TileMapLayer.Default,
            deadTreeCells,
            (int)TerrainSet.Default,
            (int)ForestTerrain.DeadTree,
            false
        );
    }
}