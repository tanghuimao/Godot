using System;
using System.Collections.Generic;
using ClassicRoguelikeCourse.Entites.Characters.Player;
using ClassicRoguelikeCourse.Entities.Characters.Enemies;
using ClassicRoguelikeCourse.Managers.SaveLoadManager;
using ClassicRoguelikeCourse.Resources.MapData;
using Godot;
using Godot.Collections;
using Player = ClassicRoguelikeCourse.Entities.Characters.Player.Player;

namespace ClassicRoguelikeCourse.Managers.MapManager;
/// <summary>
/// 地图管理器
/// </summary>
public partial class MapManager : Node, IManager, ISavable
{
    //初始化事件  Callable方法
    public event Action<Vector2I, Callable> Initialized;
    
    //导出地图数据
    [Export]
    private Resources.MapData.MapData _mapData;
    
    //地图数据
    public Resources.MapData.MapData MapData { get => _mapData; }
    
    //地图生成器
    private IMapGenerator _mapGenerator;
    public IMapGenerator MapGenerator => _mapGenerator;
    
    //角色出生位置 玩家、敌人
    private List<Vector2I> _characterCellAtSpawn = new();

    private SaveLoadManager.SaveLoadManager _saveLoadManager;
    
    public void Initialize()
    {
        //如果没有地图生成器 抛出异常
        if (GetChildCount() != 1 || GetChild(0) is not IMapGenerator)
        {
            throw new System.Exception("MapManager must have one child and it must be IMapGenerator");
        }
        _saveLoadManager = GetTree().CurrentScene.GetNode<SaveLoadManager.SaveLoadManager>("%SaveLoadManager");
        //获取地图生成器
        _mapGenerator = GetChild(0) as IMapGenerator;
        //初始化
        _mapGenerator.Initialize();
        //触发初始化事件

        if (IsMapGenerated())
        {
            Initialized?.Invoke(Vector2I.Zero, Callable.From(null));
        }
        else
        {
            Initialized?.Invoke(_mapGenerator.GetPlayerSpawnCell(), Callable.From(_mapGenerator.GetEnemySpawnCell));
        }
    }

    public void Update(double delta)
    {
    }

    /// <summary>
    /// 尝试添加角色出生位置
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public bool TryAddCharacterCellAtSpawn(Vector2I cell)
    {
        if (_characterCellAtSpawn.Contains(cell)) return false;
        _characterCellAtSpawn.Add(cell);
        return true;
    }
    /// <summary>
    /// 保存数据
    /// </summary>
    /// <returns></returns>
    public Godot.Collections.Dictionary<string, Variant> GetDataForSave()
    {
        //楼梯
        var upStairCell = Vector2I.Zero;
        var downStairCell = Vector2I.Zero;
        //未探索、探索、可见 迷雾
        var unexploredCells = new Array<Vector2I>();
        var exploredCells = new Array<Vector2I>();
        var visibleCells = new Array<Vector2I>();
        //获取地图瓦片地图
        var tileMap = GetTree().CurrentScene.GetNode<TileMap>("%TileMap");
        //遍历地图
        for (int x = 0; x < _mapData.MapSize.X; x++)
        {
            for (int y = 0; y < _mapData.MapSize.Y; y++)
            {
                //获取单元格
                var cell = new Vector2I(x, y);
                //获取单元格瓦片数据
                var tileData = tileMap.GetCellTileData((int)TileMapLayer.Default, cell);
                //判断地图图层
                if (tileData.TerrainSet == (int)TerrainSet.Stair)
                {
                    switch (tileData.Terrain)
                    {
                        case (int)StairTerrain.UpStair:
                            upStairCell = cell;
                            break;
                        case (int)StairTerrain.DownStair:
                            downStairCell = cell;
                            break;
                    }
                }
                //判断迷雾图层
                var fogTileData = tileMap.GetCellTileData((int)TileMapLayer.Fog, cell);
                if (fogTileData.TerrainSet == (int)TerrainSet.Fog)
                {
                    switch (fogTileData.Terrain)
                    {
                        case (int)FogTerrain.Unexplored:
                            unexploredCells.Add(cell);
                            break;
                        case (int)FogTerrain.Explored:
                            exploredCells.Add(cell);
                            break;
                        case (int)FogTerrain.Visible:
                            visibleCells.Add(cell);
                            break;
                    }
                }
            }
        }
        //获取敌人容器
        var enemyContainer = GetTree().CurrentScene.GetNode<Node>("%EnemyContainer");
        //获取敌人数据
        var enemies = new Array<Godot.Collections.Dictionary<string, Variant>>();
        foreach (var child in enemyContainer.GetChildren())
        {
            if (child is not Enemy enemy) continue;
            enemies.Add(enemy.GetDataForSave());
        }
        //获取物品容器
        var pickableObjectContainer = GetTree().CurrentScene.GetNode<Node>("%PickableObjectContainer");
        //获取物品数据
        var pickableObjects = new Array<Godot.Collections.Dictionary<string, Variant>>();
        foreach (var child in pickableObjectContainer.GetChildren())
        {
            if (child is not PickableObject pickableObject) continue;
            pickableObjects.Add(pickableObject.GetDataForSave());
        }
        //获取玩家数据
        var player = GetTree().CurrentScene.GetNode<Player>("%Player");

        var mapDataForSave = new Godot.Collections.Dictionary<string, Variant>
        {
            { "SceneName", GetTree().CurrentScene.Name},
            { "UpStairCell", upStairCell },
            { "DownStairCell", downStairCell },
            { "UnexploredCells", unexploredCells },
            { "ExploredCells", exploredCells },
            { "VisibleCells", visibleCells },
            { "Enemies", enemies },
            { "PickableObjects", pickableObjects },
            { "PlayerLastPosition", player.GlobalPosition },
        };
        
        //合并数据
        mapDataForSave.Merge((_mapGenerator as ISavable).GetDataForSave());
        
        return mapDataForSave;
    }
    /// <summary>
    /// 是否生成地图数据
    /// </summary>
    /// <returns></returns>
    private bool IsMapGenerated()
    {
        if (_saveLoadManager.LoadedData == null ||
            _saveLoadManager.LoadedData.Count == 0 ||
            !_saveLoadManager.LoadedData.ContainsKey("Maps")) return false;
        var maps = _saveLoadManager.LoadedData["Maps"].AsGodotArray<Godot.Collections.Dictionary<string, Variant>>();
        for (int i = 0; i < maps.Count; i++)
        {
            if (maps[i]["SceneName"].AsString() == GetTree().CurrentScene.Name) return true;
        }
        return false;
    }
}