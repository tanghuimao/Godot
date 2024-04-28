using System;
using System.Collections.Generic;
using ClassicRoguelikeCourse.Resources.MapData.DungeonData;
using Godot;
using Godot.Collections;

namespace ClassicRoguelikeCourse.Managers.MapManager.MapGenerators.DungeonGenerator;

/// <summary>
/// 地牢地图生成器
/// </summary>
public partial class DungeonGenerator : Node, IMapGenerator
{
    //地牢地图数据
    private DungeonData _dungeonData;

    //地牢地图瓦片集
    private TileSet _tileSet;

    //主场景 地图
    private TileMap _tileMap;
    private List<Rect2I> _rooms = new();

    public void Initialize()
    {
        _dungeonData = GetParent<MapManager>().MapData as DungeonData;
        //加载地牢地图瓦片集
        _tileSet = GD.Load<TileSet>("res://Resources/TileSets/DungeonTileSet.tres");
        //获取主场景 地图
        _tileMap = GetTree().CurrentScene.GetNode<TileMap>("%TileMap");
        //设置瓦片集
        _tileMap.TileSet = _tileSet;
        //生成地图
        GenerateMap();
    }

    public void Update(double delta)
    {
    }

    public Vector2I GetPlayerSpawnCell()
    {
        //随机获取一个房间
        var randIndex = GD.RandRange(0, _rooms.Count - 1);
        //获取中心点
        var playerStartCell = _rooms[randIndex].GetCenter();
        //尝试添加玩家
        GetParent<MapManager>().TryAddCharacterCellAtSpawn(playerStartCell);
        return playerStartCell;
    }

    public Vector2I GetEnemySpawnCell()
    {
        while (true)
        {
            //随机获取一个房间
            var randIndex = GD.RandRange(0, _rooms.Count - 1);
            //获取房间
            var room = _rooms[randIndex];
            //随机获取 x,y
            var randomX = GD.RandRange(room.Position.X, room.Position.X + room.Size.X - 1);
            var randomY = GD.RandRange(room.Position.Y, room.Position.Y + room.Size.Y - 1);
        
            var spawnCell = new Vector2I(randomX, randomY);
            //尝试添加敌人 返回false 则继续
            if (!GetParent<MapManager>().TryAddCharacterCellAtSpawn(spawnCell)) continue;
            return spawnCell;
        }
       
    }

    /// <summary>
    /// 生成地图
    /// </summary>
    private void GenerateMap()
    {
        //1.填充地图墙体
        FullFillWithWall();
        //2.随机生成房间
        RandomDigRoom();
        //3.随机生成走廊
        RandomDigCorridors();
    }

    /// <summary>
    /// 填充地图墙体
    /// </summary>
    private void FullFillWithWall()
    {
        //所有单元格  Array 类型必须是Godot.Collections.Array
        var allCells = new Array<Vector2I>();
        //生成
        for (int x = 0; x < _dungeonData.MapSize.X; x++)
        {
            for (int y = 0; y < _dungeonData.MapSize.Y; y++)
            {
                allCells.Add(new Vector2I(x, y));
            }
        }

        //设置地图单元格
        _tileMap.SetCellsTerrainConnect(
            (int)TileMapLayer.Default,
            allCells,
            (int)TerrainSet.Default,
            (int)DungeonTerrain.Wall
        );
    }

    /// <summary>
    /// 随机生成房间
    /// </summary>
    private void RandomDigRoom()
    {
        //所有房间单元格
        var allRoomCells = new Array<Vector2I>();
        //循环挖掘一千次
        for (int i = 0; i < 1000; i++)
        {
            //获取随机房间
            var room = GetRandomRoom();
            //判断房间是否与其他房间相交
            if (IsRoomIntersectOthers(room)) continue;
            //遍历创建房间
            for (int x = room.Position.X; x < room.Position.X + room.Size.X; x++)
            {
                for (int y = room.Position.Y; y < room.Position.Y + room.Size.Y; y++)
                {
                    allRoomCells.Add(new Vector2I(x, y));
                }
            }
            //添加房间
            _rooms.Add(room);
        }
        //排序
        if (GD.RandRange(0,1) == 0)
        {
            SortRoomsFromLeftToRight();
        }
        else
        {
            SortRoomsFromTopToBottom();
        }
        
        //设置地图单元格
        _tileMap.SetCellsTerrainConnect(
            (int)TileMapLayer.Default,
            allRoomCells,
            (int)TerrainSet.Default,
            (int)DungeonTerrain.Floor
        );
    }

    /// <summary>
    /// 获取随机房间
    /// </summary>
    /// <returns></returns>
    private Rect2I GetRandomRoom()
    {
        //房间宽度
        var roomSizeX = GD.RandRange(_dungeonData.MinRoomSize.X, _dungeonData.MaxRoomSize.X);
        //房间高度
        var roomSizeY = GD.RandRange(_dungeonData.MinRoomSize.Y, _dungeonData.MaxRoomSize.Y);
        //房间坐标 随机 1(地图边缘不被挖掘 从1开始), _dungeonData.MapSize.X - 1 - roomSizeX （保证右边边界不被挖掘）
        var roomX = GD.RandRange(1, _dungeonData.MapSize.X - 1 - roomSizeX);
        var roomY = GD.RandRange(1, _dungeonData.MapSize.Y - 1 - roomSizeY);
        //返回房间矩形 
        // roomX  -------  roomSizeX
        //       |      |
        // roomY -------  roomSizeY
        return new Rect2I(roomX, roomY, roomSizeX, roomSizeY);
    }

    /// <summary>
    /// 判断房间是否与其他房间相交
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    private bool IsRoomIntersectOthers(Rect2 room)
    {
        for (int i = 0; i < _rooms.Count; i++)
        {
            //判断是否相交
            if (room.Intersects(_rooms[i], true))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 从左到右排序房间
    /// </summary>
    private void SortRoomsFromLeftToRight()
    {
        _rooms.Sort((a, b) => a.Position.X.CompareTo(b.Position.X));
    }
    
    private void SortRoomsFromTopToBottom()
    {
        _rooms.Sort((a, b) => a.Position.Y.CompareTo(b.Position.Y));
    }
    
    
    /// <summary>
    /// 随机生成走廊
    /// </summary>
    private void RandomDigCorridors()
    {
        //所有走廊单元格
        var allCorridorCells = new Array<Vector2I>();


        for (int i = 0; i < _rooms.Count - 1; i++)
        {
            //获取房间
            var room1 = _rooms[i];
            var room2 = _rooms[i + 1];
            //获取房间中心点
            var x1 = room1.GetCenter().X;
            var y1 = room1.GetCenter().Y;
            var x2 = room2.GetCenter().X;
            var y2 = room2.GetCenter().Y;
            //判断挖掘方式  0-水平->垂直 1-垂直->水平
            if (GD.RandRange(0, 1) == 0)
            {
                //  ------
                //       |
                allCorridorCells.AddRange(GetHorizontalCorridorCells(x1, x2, y1));
                allCorridorCells.AddRange(GetVerticalCorridorCells(y1, y2, x2));
            }
            else
            {
                //  |
                //  ------
                allCorridorCells.AddRange(GetVerticalCorridorCells(y1, y2, x1));
                allCorridorCells.AddRange(GetHorizontalCorridorCells(x1, x2, y2));
            }
        }
        
        //设置地图单元格
        _tileMap.SetCellsTerrainConnect(
            (int)TileMapLayer.Default,
            allCorridorCells,
            (int)TerrainSet.Default,
            (int)DungeonTerrain.Floor
        );
    }

    /// <summary>
    /// 获取水平走廊单元格
    /// </summary>
    /// <param name="x1">横坐标开始位置</param>
    /// <param name="x2">横坐标结束位置</param>
    /// <param name="y">纵坐标不变</param>
    /// <returns></returns>
    private Array<Vector2I> GetHorizontalCorridorCells(int x1, int x2, int y)
    {
        //横坐标向量集合
        var corridorCells = new Array<Vector2I>();
        //横坐标步长
        var step = x2 - x1;
        if (step >= 0)
        {
            for (int x = x1; x <= x2; x++)
            {
                corridorCells.Add(new Vector2I(x, y));
            }
        }
        else
        {
            for (int x = x2; x <= x1; x++)
            {
                corridorCells.Add(new Vector2I(x, y));
            }
        }

        return corridorCells;
    }
    /// <summary>
    /// 获取垂直走廊单元格
    /// </summary>
    /// <param name="y1">纵坐标开始位置</param>
    /// <param name="y2">纵坐标结束位置</param>
    /// <param name="x">横坐标不变</param>
    /// <returns></returns>
    private Array<Vector2I> GetVerticalCorridorCells(int y1, int y2, int x)
    {
        //纵坐标向量集合
        var corridorCells = new Array<Vector2I>();

        //纵坐标步长  godot中 y向下为正
        var step = y2 - y1;
        if (step >= 0)
        {
            for (int y = y1; y <= y2; y++)
            {
                corridorCells.Add(new Vector2I(x, y));
            }
        }
        else
        {
            for (int y = y2; y <= y1; y++)
            {
                corridorCells.Add(new Vector2I(x, y));
            }
        }
        return corridorCells;
    }

}